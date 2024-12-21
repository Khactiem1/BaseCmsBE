using Cms.Core.Common.Extension;
using Cms.DL;
using Cms.Model;
using CMS.Core.Database;
using CMS.Core.Database.Service;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Transactions;
using System.Collections;

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
        protected IDatabaseService _databaseService;

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
        /// Thực hiện xuất excel dữ liệu
        /// </summary>
        /// <returns></returns>
        public async Task<Stream> ExportData(ExportDataParam param)
        {
            var pagingResult = await GetDataPaging(param.Param, typeof(T).GetViewNameOnly());
            List<object> records = pagingResult.PageData;
            if (records == null || records.Count == 0 || param.HeaderColumns.Count == 0) return null;
            // lấy các thuộc tính của nhân viên
            var properties = typeof(T).GetProperties();
            int indexCol = param.HeaderColumns.Count;
            string column = ExcelUtil.GetColumnName(indexCol + 1);
            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream ?? new MemoryStream()))
            {
                var sheet = package.Workbook.Worksheets.Add(param.Title);
                // style header customize tên header của file excel
                ExcelUtil.SetUpExportHeaderData(ref sheet, column, param.Title, records.Count(), param.HeaderColumns);
                int index = 4;
                int indexRow = 0;
                //string convertDate = "M/d/yyyy hh:mm:ss tt";
                // duyệt các nhân viên thêm dữ liệu vào excel (phần body)
                foreach (var recordItem in records)
                {
                    int indexBody = 2;
                    sheet.Cells[index, 1].Value = indexRow + 1;
                    sheet.Cells[index, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    index++;
                    foreach (var header in param.HeaderColumns)
                    {
                        ExcelUtil.SetCellValue(ref sheet, indexRow, indexBody, recordItem, header);
                        indexBody++;
                    }
                    indexRow++;
                }
                package.Save();
                package.Stream.Position = 0;
                return package.Stream;
            }
        }

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
            string columnKey = typeof(T).GetPrimaryKeyFieldName();
            Dictionary<string, object> param = new Dictionary<string, object>();

            if (whereParameter != null)
            {
                string whereString = whereParameter.GetWhereClause();
                if (!string.IsNullOrEmpty(whereString))
                {
                    commandText.Append($" WHERE {whereString} AND (is_deleted = false OR is_deleted is null)");
                    commandTextCount.Append($" WHERE {whereString} AND (is_deleted = false OR is_deleted is null)");
                }
                else
                {
                    commandText.Append($" WHERE (is_deleted = false OR is_deleted is null)");
                    commandTextCount.Append($" WHERE (is_deleted = false OR is_deleted is null)");
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
                commandText.Append($" WHERE (is_deleted = false OR is_deleted is null)");
                commandTextCount.Append($" WHERE (is_deleted = false OR is_deleted is null)");
            }
            if (!string.IsNullOrEmpty(sort))
            {
                commandText.Append($" ORDER BY {sort}");
            }
            else
            {
                commandText.Append($" ORDER BY modified_date DESC");
            }
            if (!string.IsNullOrEmpty(columnKey)) // Bổ sung mặc định sort theo ID nữa
            {
                commandText.Append($", {columnKey} DESC");
            }
            bool isProcessLimitOffset = ProcessLimitOffset(ref commandText, pagingRequest);
            if (!isProcessLimitOffset)
            {
                commandText.Append($";");
            }

            // Paging dữ liệu
            result.PageData = await _databaseService.QueryUsingCommandText<object>(commandText.ToString(), param);
            if (isProcessLimitOffset)
            {
                result.Count = await _databaseService.ExecuteScalarUsingCommandText<int>(commandTextCount.ToString(), param);
            }
            else
            {
                result.Count = result.PageData.Count;
            }

            return result;
        }

        /// <summary>
        /// Xử lý sql phân trang
        /// </summary>
        public bool ProcessLimitOffset(ref StringBuilder commandText, PagingRequest pagingRequest)
        {
            if (pagingRequest.PageIndex == 0 || pagingRequest.PageSize == 0)
            {
                return false;
            }
            commandText.Append($" LIMIT {pagingRequest.PageSize} OFFSET {(pagingRequest.PageIndex - 1) * pagingRequest.PageSize};");
            return true;
        }

        /// <summary>
        /// Lấy ra chi tiết một bản ghi theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public async Task<dynamic> GetMasterDetail(Guid id, Type? modelType = null)
        {
            if (modelType == null)
            {
                modelType = typeof(T);
            }
            // Get master trước
            var baseEntity = (BaseEntity)Activator.CreateInstance(modelType);
            object data = await _databaseService.GetByID<T>(modelType, id);

            // Get detail
            if (data != null)
            {
                baseEntity = (BaseEntity)data;
                if (baseEntity.ModelDetailConfig != null && baseEntity.ModelDetailConfig.Count > 0)
                {
                    var foreignKeyValue = baseEntity.GetValue(baseEntity.GetType().GetPrimaryKeyFieldName());
                    foreach (ModelDetailConfig detailConfig in baseEntity.ModelDetailConfig.Where(c => !string.IsNullOrWhiteSpace(c.PropertyNameOnMaster)))
                    {
                        string commandText = $"SELECT * FROM {detailConfig.ModelType.GetViewNameOnly()} WHERE is_deleted = false AND {modelType.GetPrimaryKeyFieldName()} = @v_IDValue;";
                        Dictionary<string, object> param = new Dictionary<string, object>()
                        {
                            { "v_IDValue", foreignKeyValue },
                        };
                        IList lstDetail = await _databaseService.QueryUsingCommandText<object>(commandText, param);
                        if (lstDetail != null && lstDetail.Count > 0)
                        {
                            Type genericListType = typeof(List<>).MakeGenericType(detailConfig.ModelType);
                            IList list = (IList)Activator.CreateInstance(genericListType);
                            var deserializedList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(lstDetail), genericListType);
                            // Gán giá trị vào danh sách
                            foreach (var item in (IList)deserializedList)
                            {
                                list.Add(item);
                            }
                            baseEntity.SetValue(detailConfig.PropertyNameOnMaster, list);
                        }
                    }
                }
            }
            await AfterGetFormData(baseEntity);
            return baseEntity;
        }

        /// <summary>
        /// Xử lý sau khi get data ra
        /// </summary>
        /// <returns></returns>
        public virtual async Task AfterGetFormData(BaseEntity baseEntity)
        {

        }

        /// <summary>
        /// Khôi phục dữ liệu đã bị xoá theo mã
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceResponse> RestoreDataDelete(BaseEntity baseEntity)
        {
            string fieldUnique = baseEntity.GetType().GetFieldUnique();
            string tableName = baseEntity.GetType().GetTableNameOnly();
            string columnKey = baseEntity.GetType().GetPrimaryKeyFieldName();
            Dictionary<string, object> param = new Dictionary<string, object>();
            string command = $"UPDATE {tableName} SET is_deleted = false, deleted_date = @D_deleted_date, modified_by = @D_modified_by WHERE {fieldUnique} = @D_{fieldUnique}; select {columnKey} from {tableName} WHERE {fieldUnique} = @D_{fieldUnique};";
            param.Add($"@D_deleted_date", null);
            param.Add($"@D_modified_by", baseEntity.modified_by);
            param.Add($"@D_{fieldUnique}", baseEntity.GetValue(fieldUnique));
            var result = await _databaseService.QueryUsingCommandText<object>(command, param);
            return new ServiceResponse()
            {
                Data = result?.FirstOrDefault(),
            };
        }

        /// <summary>
        /// Xử lý validate trước khi cất dữ liệu
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<ValidateResult>> ValidateBeforeSave(List<BaseEntity> baseEntitys)
        {
            List<ValidateResult> validateResults = new List<ValidateResult>();
            var validateCheckDuplicate = await CheckDuplidateData(baseEntitys);
            if (validateCheckDuplicate != null && validateCheckDuplicate.Count > 0)
            {
                validateResults.AddRange(validateCheckDuplicate);
            }
            var validateCustom = await ValidateBeforeSaveCustom(baseEntitys);
            if(validateCustom != null && validateCustom.Count > 0)
            {
                validateResults.AddRange(validateCustom);
            }
            return validateResults;
        }

        /// <summary>
        /// Xử lý check trùng dữ liệu
        /// </summary>
        /// <returns></returns>
        public async Task<List<ValidateResult>> CheckDuplidateData(List<BaseEntity> baseEntitys)
        {
            List<ValidateResult> validateResults = new List<ValidateResult>();
            StringBuilder commandCheckDuplicate = new StringBuilder();
            StringBuilder commandCheckDuplicateDel = new StringBuilder();
            Dictionary<string, object> param = new Dictionary<string, object>();
            string fieldUnique = baseEntitys[0].GetType().GetFieldUnique();
            if (!string.IsNullOrEmpty(fieldUnique))
            {
                string tableName = baseEntitys[0].GetType().GetTableNameOnly();
                string columnKey = baseEntitys[0].GetType().GetPrimaryKeyFieldName();
                commandCheckDuplicate.Append($"select true from {tableName} where ");
                commandCheckDuplicateDel.Append($"select true from {tableName} where ");
                int count = 0;
                int countDel = 0;
                foreach (var baseEntity in baseEntitys)
                {
                    if(baseEntity.State == EntityState.Insert)
                    {
                        if(count > 0) commandCheckDuplicate.Append("OR ");
                        if (countDel > 0) commandCheckDuplicateDel.Append("OR ");
                        commandCheckDuplicate.Append($"({fieldUnique} = @v_{fieldUnique}{count} and (is_deleted = false OR is_deleted is null)) ");
                        commandCheckDuplicateDel.Append($"({fieldUnique} = @v_{fieldUnique}{countDel}_del and is_deleted = true) ");
                        param.Add($"v_{fieldUnique}{count}", baseEntity.GetValue(fieldUnique));
                        param.Add($"v_{fieldUnique}{countDel}_del", baseEntity.GetValue(fieldUnique));
                        count++;
                        countDel++;
                    }
                    else if (baseEntity.State == EntityState.Update)
                    {
                        if (count > 0)
                        {
                            commandCheckDuplicate.Append("OR ");
                        }
                        commandCheckDuplicate.Append($"({fieldUnique} = @v_{fieldUnique}{count} and {columnKey} <> @v_{columnKey}{count} and (is_deleted = false OR is_deleted is null)) ");
                        param.Add($"v_{fieldUnique}{count}", baseEntity.GetValue(fieldUnique));
                        param.Add($"v_{columnKey}{count}", baseEntity.GetValue(columnKey));
                        count++;
                    }
                }
                if (count > 0)
                {
                    var result = await _databaseService.ExecuteScalarUsingCommandText<bool>(commandCheckDuplicate.ToString(), param);
                    if (result)
                    {
                        validateResults.Add(new ValidateResult()
                        {
                            ErrorMessage = "i18nCommon.DuplicateData",
                            Code = EnumValidateResult.Duplicate
                        });
                    }
                }
                if (countDel > 0)
                {
                    var result = await _databaseService.ExecuteScalarUsingCommandText<bool>(commandCheckDuplicateDel.ToString(), param);
                    if (result)
                    {
                        validateResults.Add(new ValidateResult()
                        {
                            ErrorMessage = "i18nCommon.DuplicateDataDel",
                            Code = EnumValidateResult.DuplicateDelete
                        });
                    }
                }
            }
            return validateResults;
        }

        /// <summary>
        /// Xử lý validate trước khi cất dữ liệu dành cho các service override lại xử lý custom validate
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<ValidateResult>> ValidateBeforeSaveCustom(List<BaseEntity> baseEntitys)
        {
            return null;
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
                // Thực hiện validate dữ liệu
                var validateResults = await ValidateBeforeSave(baseEntitys);
                if (validateResults.Count > 0)
                {
                    serviceResponse.Data = validateResults;
                    serviceResponse.Success = false;
                    return serviceResponse;
                }

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

            // Save detail
            foreach (var entity in baseEntitys)
            {
                if (entity.ModelDetailConfig != null && entity.ModelDetailConfig.Count > 0)
                {
                    var foreignKeyValue = entity.GetValue(entity.GetType().GetPrimaryKeyFieldName());
                    foreach (ModelDetailConfig detailConfig in entity.ModelDetailConfig.Where(c => !string.IsNullOrWhiteSpace(c.PropertyNameOnMaster)))
                    {
                        IList lstDetailObject = entity.GetValue<IList>(detailConfig.PropertyNameOnMaster);
                        if (lstDetailObject != null && lstDetailObject.Count > 0)
                        {
                            List<BaseEntity> lstDetailSave = new List<BaseEntity>();
                            foreach (BaseEntity detail in lstDetailObject)
                            {
                                if (detail.State == EntityState.Insert)
                                {
                                    detail.created_date = DateTime.Now;
                                }
                                detail.modified_date = DateTime.Now;
                                if (detail.State == EntityState.Insert || detail.State == EntityState.Update || detail.State == EntityState.Delete)
                                {
                                    // Gán lại khoá chính cho detail
                                    detail.SetValue(detailConfig.ForeignKeyName, foreignKeyValue);
                                    lstDetailSave.Add(detail);
                                }
                            }
                            if(lstDetailSave.Count > 0)
                            {
                                await DoSaveAsync(lstDetailSave, transaction);
                            }
                        }
                    }
                }
            }
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
                    sql.AppendLine($"UPDATE {tableName} SET is_active = {valueInactive}, modified_date = @I_modified_date{count}, modified_by = @I_modified_by{count} WHERE {columnKey} = @I_{columnKey}{count};");
                    param.Add($"@I_modified_date{count}", DateTime.Now);
                    param.Add($"@I_modified_by{count}", baseModel.modified_by);
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
                        sql.AppendLine($"UPDATE {tableName} SET is_deleted = true, deleted_date = @D_deleted_date{count}, deleted_by = @D_deleted_by{count} WHERE {columnKey} = @D_{columnKey}{count};");
                        param.Add($"@D_deleted_date{count}", DateTime.Now);
                        param.Add($"@D_deleted_by{count}", baseModel.deleted_by);
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
                    if(baseModel.State == EntityState.Insert)
                    {
                        if(baseModel.is_active == null)
                        {
                            baseModel.is_active = true;
                        }
                    }
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
                IEnumerable<string> columnsUpdate = columns.FindAll(x => x != columnKey && !columnSkipsOnUpdate.Contains(x))
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
                if (baseEntity.State == EntityState.Insert)
                {
                    baseEntity.created_date = DateTime.Now;
                }
                baseEntity.modified_date = DateTime.Now;
            }
        }

        #endregion
    }
}
