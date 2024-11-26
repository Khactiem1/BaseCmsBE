using Cms.Core.Common;
using Cms.Core.Common.Extension;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core.Database.Service
{
    public class DatabaseService : IDatabaseService
    {
        /// <summary>
        /// Trả về cnn IDbConnection theo connectionString
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối</param>
        /// <returns></returns>
        public IDbConnection GetConnection(string connectionString = "")
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConfigUtil.ConfigGlobal.connectionStrings.DBDefault;
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("DEV: connectionString IsNullOrEmpty");
            }
            return new NpgsqlConnection(connectionString);
        }

        /// <summary>
        /// Thực thi một commandText trả về một giá trị
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="dicParam"></param>
        /// <param name="transaction"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// @author nktiem 25.11.2024
        public async Task<T> ExecuteScalarUsingCommandText<T>(string commandText, Dictionary<string, object> dicParam = null, IDbTransaction transaction = null, IDbConnection connection = null)
        {
            T result = default(T);
            var con = transaction != null ? transaction.Connection : connection;
            if(con != null)
            {
                var cd = new CommandDefinition(commandText, commandType: CommandType.Text, parameters: dicParam, transaction: transaction);
                result = await con.ExecuteScalarAsync<T>(cd);
            }
            else
            {
                IDbConnection cnn = GetConnection();
                try
                {
                    var cd = new CommandDefinition(commandText, commandType: CommandType.Text, parameters: dicParam, transaction: transaction);
                    result = await cnn.ExecuteScalarAsync<T>(cd);
                }
                finally
                {
                    if (cnn.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    cnn.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// Thực thi một commandText trả về một danh sách
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="dicParam"></param>
        /// <param name="transaction"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// @author nktiem 25.11.2024
        public async Task<List<T>> QueryUsingCommandText<T>(string commandText, Dictionary<string, object> dicParam = null, IDbTransaction transaction = null, IDbConnection connection = null)
        {
            List<T> result;
            var con = transaction != null ? transaction.Connection : connection;
            if (con != null)
            {
                var cd = new CommandDefinition(commandText, commandType: CommandType.Text, parameters: dicParam, transaction: transaction);
                result = (await con.QueryAsync<T>(cd)).ToList();
            }
            else
            {
                IDbConnection cnn = GetConnection();
                try
                {
                    var cd = new CommandDefinition(commandText, commandType: CommandType.Text, parameters: dicParam, transaction: transaction);
                    result = (await cnn.QueryAsync<T>(cd)).ToList();
                }
                finally
                {
                    if (cnn.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    cnn.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// Lấy ra chi tiết 1 bản ghi theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// @author nktiem 25.11.2024
        public async Task<T> GetByID<T>(Type modelType, Guid id)
        {
            string commandText = string.Empty;
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            GenerateSelectByID(ref commandText, ref dicParam, modelType, id);
            var data = await QueryUsingCommandText<T>(commandText, dicParam);
            return data.FirstOrDefault();
        }

        /// <summary>
        /// Gen ra câu truy vấn lấy 1 bản ghi theo ID
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="dicParam"></param>
        /// <param name="modelType"></param>
        /// <param name="id"></param>
        /// <param name="columns"></param>
        /// @author nktiem 25.11.2024
        public void GenerateSelectByID(ref string commandText, ref Dictionary<string, object> dicParam, Type modelType, Guid id, string columns = "*")
        {
            commandText = $"SELECT {columns} FROM {modelType.GetTableNameOnly()} WHERE is_deleted = false AND {modelType.GetPrimaryKeyFieldName()} = @IDValue;";
            dicParam.Add("@IDValue", id);
        }
    }
}
