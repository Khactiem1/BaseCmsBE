using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Core.Common
{
    /// <summary>
    /// Attribute cấu hình table
    /// </summary>
    public class ConfigTableAttribute : Attribute
    {
        /// <summary>
        /// Tên bảng trong database
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// View Paging nếu có
        /// </summary>
        public string? ViewName { get; set; }

        /// <summary>
        /// Field dữ liệu Unique trong bảng
        /// </summary>
        public string? FieldUnique { get; set; }

        /// <summary>
        /// Schema của bảng
        /// </summary>
        public string Schema { get; set; }

        #region Contructor

        public ConfigTableAttribute(string tableName = "", string fieldUnique = "", string viewName = "", string schema = "")
        {
            TableName = tableName;
            ViewName = viewName;
            FieldUnique = fieldUnique;
            Schema = schema;
        }

        #endregion
    }
}
