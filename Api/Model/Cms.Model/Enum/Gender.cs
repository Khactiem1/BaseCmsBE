using System;

namespace Cms.Model
{
    /// <summary>
    /// Giới tính 
    /// Enum giới tính của thực thể người, 3 loại nam, nữ, khác
    /// </summary>
    public enum Gender : int
    {
        /// <summary>
        /// nam
        /// </summary>
        Male = 0,

        /// <summary>
        /// nữ
        /// </summary>
        Female = 1,

        /// <summary>
        /// khác
        /// </summary>
        Other = 2,
    }
}
