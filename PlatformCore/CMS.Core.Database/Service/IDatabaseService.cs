using System;
using System.Data;

namespace CMS.Core.Database
{
    public interface IDatabaseService
    {
        /// <summary>
        /// Trả về cnn IDbConnection theo connectionString
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối</param>
        /// <returns></returns>
        public IDbConnection GetConnection(string connectionString = "");

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
        public Task<T> ExecuteScalarUsingCommandText<T>(string commandText, Dictionary<string, object> dicParam = null, IDbTransaction transaction = null, IDbConnection connection = null);

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
        public Task<List<T>> QueryUsingCommandText<T>(string commandText, Dictionary<string, object> dicParam = null, IDbTransaction transaction = null, IDbConnection connection = null);

        /// <summary>
        /// Lấy ra chi tiết 1 bản ghi theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<T> GetByID<T>(Type modelType, Guid id);
    }
}
