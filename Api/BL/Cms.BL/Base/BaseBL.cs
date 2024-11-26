using Cms.Core.Common.Extension;
using Cms.DL;
using Cms.Model;
using Cms.Model.Enum;
using CMS.Core.Database;
using CMS.Core.Database.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace Cms.BL
{
    /// <summary>
    /// Base BL tầng BL
    /// </summary>
    /// @author nktiem 24/11/2024
    public class BaseBL<T> : IBaseBL<T>
    {
        #region Field

        private IBaseDL<T> _baseDL;
        private IDatabaseService _databaseService;

        #endregion

        #region Contructor

        public BaseBL(IBaseDL<T> baseDL, IDatabaseService databaseService)
        {
            _baseDL = baseDL;
            _databaseService = databaseService;
        }

        #endregion

        #region Method

        /// <summary>
        /// Thực hiện Paging data
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public async Task<PagingResult> GetDataPaging(PagingRequest pagingRequest, string viewName)
        {
            // Xử lý các tham số param
            PagingResult result = new PagingResult();
            WhereParameter whereParameter = ParamService.ParseFilter(pagingRequest.Filter);
            string sort = ParamService.ParseSort(pagingRequest.Sort);
            string columns = ParamService.ParseColumn(pagingRequest.Columns);
            StringBuilder commandText = new StringBuilder($"SELECT {columns} FROM {viewName}");
            StringBuilder commandTextCount = new StringBuilder($"SELECT COUNT(1) FROM {viewName}");
            Dictionary<string, object> param = new Dictionary<string, object>();
            if (whereParameter != null)
            {
                string whereString = whereParameter.GetWhereClause();
                if (!string.IsNullOrEmpty(whereString))
                {
                    commandText.Append($" WHERE {whereString} AND is_deleted = false");
                    commandTextCount.Append($" WHERE {whereString} AND is_deleted = false");
                }
                else
                {
                    commandText.Append($" WHERE is_deleted = false");
                    commandTextCount.Append($" WHERE is_deleted = false");
                }
                if (whereParameter.WhereValues != null)
                {
                    foreach (var item in whereParameter.WhereValues)
                    {
                        param.AddOrUpdate(item.Key, item.Value);
                    }
                }
            }
            else
            {
                commandText.Append($" WHERE is_deleted = false");
                commandTextCount.Append($" WHERE is_deleted = false");
            }
            if (!string.IsNullOrEmpty(sort))
            {
                commandText.Append($" ORDER BY {sort}");
            }
            ProcessLimitOffset(ref commandText, pagingRequest);

            // Paging dữ liệu
            result.PageData = await _databaseService.QueryUsingCommandText<object>(commandText.ToString(), param);
            result.Count = await _databaseService.ExecuteScalarUsingCommandText<int>(commandTextCount.ToString(), param);

            return result;
        }

        /// <summary>
        /// Xử lý sql phân trang
        /// </summary>
        public void ProcessLimitOffset(ref StringBuilder commandText, PagingRequest pagingRequest)
        {
            if (pagingRequest.PageIndex == 0 || pagingRequest.PageSize == 0)
            {
                return;
            }
            commandText.Append($" LIMIT {pagingRequest.PageSize} OFFSET {(pagingRequest.PageIndex - 1) * pagingRequest.PageSize};");
        }

        /// <summary>
        /// Lấy ra chi tiết một bản ghi theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public async Task<dynamic> GetMasterDetail(Guid id, Type? modelType = null)
        {
            if(modelType == null)
            {
                modelType = typeof(T);
            }
            return await _databaseService.GetByID<T>(modelType, id);
        }

        /// <summary>
        /// Thực hiện lưu dữ liệu
        /// </summary>
        /// <param name="baseEntitys">đối tượng cần lưu</param>
        /// <param name="dataInactives">Model này phục vụ toggle inactive bản ghi</param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public async Task<ServiceResponse> SaveDataAsync(List<BaseEntity> baseEntitys, ModelToggleInactive dataInactives = null)
        {
            ServiceResponse serviceResponse = new ServiceResponse();
            IDbConnection connection = null;
            IDbTransaction transaction = null;

            try
            {
                // Thực hiện validate dữ liệu: TODO

                // Xử lý dữ liệu trước khi cất
                await BeforeSaveDataAsync(baseEntitys);

                connection = _databaseService.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();

                // Cất dữ liệu
                var result = await DoSaveAsync(baseEntitys, transaction, dataInactives: dataInactives);

                // Xử lý sau khi save thành công còn transaction
                if (result)
                {
                    await AfterSaveAsyncWithTransaction(baseEntitys, transaction);
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                    serviceResponse.Success = false;
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw;
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            if (serviceResponse.Success)
            {
                // Xử lý sau khi save thành công không còn transaction
                await AfterSaveAsync(baseEntitys, serviceResponse);
                if (baseEntitys.Count == 1)
                {
                    serviceResponse.Data = baseEntitys[0];
                }
                else
                {
                    serviceResponse.Data = baseEntitys;
                }
            }
            return serviceResponse;
        }

        /// <summary>
        /// Thực thi cất dữ liệu
        /// </summary>
        /// <param name="baseEntitys"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<bool> DoSaveAsync(List<BaseEntity> baseEntitys, IDbTransaction transaction, ModelToggleInactive dataInactives = null)
        {
            var param = new Dictionary<string, object>();
            var command = BuildQueryStringUpdate(baseEntitys, ref param, dataInactives: dataInactives);
            await _databaseService.ExecuteScalarUsingCommandText<int>(command, param, transaction: transaction);
            return true;
        }

        /// <summary>
        /// Build command cất dữ liệu
        /// </summary>
        /// <param name="baseEntitys"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public string BuildQueryStringUpdate(List<BaseEntity> baseEntitys, ref Dictionary<string, object> param, ModelToggleInactive dataInactives = null)
        {
            StringBuilder sql = new StringBuilder();
            string tableName = baseEntitys[0].GetType().GetTableNameOnly();
            string columnKey = baseEntitys[0].GetType().GetPrimaryKeyFieldName();

            //Lấy thông tin columns
            var columns = baseEntitys[0].GetType().GetColumnTable();

            var columnSkips = new string[] { };
            columns = columns.FindAll(x => !columnSkips.Contains(x));
            //Build câu lênh query string để cất dữ liệu

            var modelInserts = baseEntitys.FindAll(x => x.State == EntityState.Insert || x.State == EntityState.Update);
            var modelDeletes = baseEntitys.FindAll(x => x.State == EntityState.Delete);
            var modelInactives = baseEntitys.FindAll(x => x.State == EntityState.Inactive);

            param = new Dictionary<string, object>();
            int count = 0;

            if (modelInactives.Count > 0 && dataInactives != null)
            {
                string valueInactive = dataInactives.Inactive ? "true" : "false";
                foreach (var baseModel in modelInactives)
                {
                    sql.AppendLine($"UPDATE {tableName} SET inactive = {valueInactive}, modified_date = @I_modified_date{count} WHERE {columnKey} = @I_{columnKey}{count};");
                    param.Add($"@I_modified_date{count}", DateTime.Now);
                    param.Add($"@I_{columnKey}{count}", baseModel.GetValue(columnKey));
                    count++;
                }
            }

            count = 0;
            if (modelDeletes.Count > 0)
            {
                foreach (var baseModel in modelDeletes)
                {
                    if(baseModel.is_delete_in_database == true)
                    {
                        sql.AppendLine($"delete from {tableName} WHERE {columnKey} = @D_{columnKey}{count};");
                    }
                    else
                    {
                        sql.AppendLine($"UPDATE {tableName} SET is_deleted = true, deleted_date = @D_deleted_date{count} WHERE {columnKey} = @D_{columnKey}{count};");
                        param.Add($"@D_deleted_date{count}", DateTime.Now);
                    }
                    //Add param
                    param.Add($"@D_{columnKey}{count}", baseModel.GetValue(columnKey));
                    count++;
                }
            }

            count = 0;
            bool isFirst = true;
            sql.AppendLine("SELECT 0;");
            columns.Add(columnKey);
            columns = columns.Distinct().ToList();
            if (modelInserts?.Count > 0)
            {
                sql.AppendLine($"INSERT INTO {tableName} ( {string.Join(",", columns)}) VALUES");
                foreach (var baseModel in modelInserts)
                {
                    if (string.IsNullOrEmpty(baseModel.GetValue(columnKey) + "") || baseModel.GetValue(columnKey) == null || (baseModel.GetValue(columnKey) + "") == "00000000-0000-0000-0000-000000000000")
                    {
                        baseModel.SetValue(columnKey, Guid.NewGuid());
                    }
                    //Tạo query
                    if (isFirst)
                    {
                        sql.AppendLine($"({"@" + string.Join($"_{count},@", columns)}_{count})");
                        isFirst = false;
                    }
                    else
                    {
                        sql.AppendLine($",({"@" + string.Join($"_{count},@", columns)}_{count})");
                    }
                    //Add param
                    foreach (var col in columns)
                    {
                        param.Add($"@{col}_{count}", baseModel.GetValue(col));
                    }
                    count += 1;
                }

                var columnSkipsOnUpdate = new string[] { "created_by", "created_date" };
                IEnumerable<string> columnsUpdate = null;
                columnsUpdate = columns.FindAll(x => x != columnKey && !columnSkipsOnUpdate.Contains(x))
                        .Select(x => { return $"{x} = EXCLUDED.{x}"; });
                sql.AppendLine($"ON CONFLICT ({columnKey}) DO UPDATE SET {string.Join(", ", columnsUpdate)};");
                sql.AppendLine("SELECT 0;");
            }
            return sql.ToString();
        }

        /// <summary>
        /// Xử lý dữ liệu sau khi cất còn transaction
        /// </summary>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public virtual async Task AfterSaveAsyncWithTransaction(List<BaseEntity> baseEntitys, IDbTransaction transaction)
        {

        }

        /// <summary>
        /// Xử lý dữ liệu sau khi cất ko còn transaction
        /// </summary>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public virtual async Task AfterSaveAsync(List<BaseEntity> baseEntitys, ServiceResponse serviceResponse)
        {

        }

        /// <summary>
        /// Hàm xử lý dữ liệu trước khi cất
        /// </summary>
        /// <param name="baseEntitys"></param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public virtual async Task BeforeSaveDataAsync(List<BaseEntity> baseEntitys)
        {
            foreach(var baseEntity in baseEntitys)
            {
                baseEntity.created_date = DateTime.Now;
                baseEntity.modified_date = DateTime.Now;
            }
        }

        #endregion
    }
}
