using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Model
{
    /// <summary>
    /// Enum state save record
    /// </summary>
    public enum EntityState : int
    {
        /// <summary>
        /// Không làm gì cả
        /// </summary>
        None = 0,

        /// <summary>
        /// Thêm
        /// </summary>
        Insert = 1,

        /// <summary>
        /// Sửa
        /// </summary>
        Update = 2,

        /// <summary>
        /// Xoá
        /// </summary>
        Delete = 3,

        /// <summary>
        /// Togle active bản ghi
        /// </summary>
        Inactive = 4,
    }

    /// <summary>
    /// EnumValidateResult
    /// </summary>
    public enum EnumValidateResult : int
    {
        /// <summary>
        /// Duplicate Dữ liệu
        /// </summary>
        Duplicate = 1,

        /// <summary>
        /// Duplicate với dữ liệu đã bị xoá
        /// </summary>
        DuplicateDelete = 2,
    }
}
