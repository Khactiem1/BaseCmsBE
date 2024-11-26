using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Model
{
    /// <summary>
    /// Kiểu ServiceResponse trả về của một request
    /// </summary>
    public class ServiceResponse
    {
        #region Field

        /// <summary>
        /// trả về true hoặc false (thành công hoặc thất bại)
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Mã lỗi đi kèm
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Dữ liệu đi kèm
        /// </summary>
        public dynamic? Data { get; set; }

        /// <summary>
        /// DevMessage
        /// </summary>
        public string? DevMessage { get; set; }

        /// <summary>
        /// UserMessage
        /// </summary>
        public string? UserMessage { get; set; }

        /// <summary>
        /// Serve Time
        /// </summary>
        public DateTime ServeTime { get; set; } = DateTime.Now;

        #endregion
    }
}
