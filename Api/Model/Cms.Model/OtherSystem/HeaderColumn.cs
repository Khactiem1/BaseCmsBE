using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Model
{
    public class HeaderColumn
    {
        /// <summary>
        /// Cột dữ liệu cần lấy
        /// </summary>
        public string? DataField { get; set; }

        /// <summary>
        /// Kiểu format
        /// </summary>
        public FormatType? FormatType { get; set; }

        /// <summary>
        /// Tên cột
        /// </summary>
        public string? Header { get; set; }

        /// <summary>
        /// Độ rộng
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Enum EnumResources
        /// </summary>
        public List<EnumResource>? EnumResources { get; set; } = null;
    }

    /// <summary>
    /// kiểu enum phía client
    /// </summary>
    public class EnumResource
    {
        /// <summary>
        /// Giá trị
        /// </summary>
        public object enumValue { get; set; } = "";

        /// <summary>
        /// Key
        /// </summary>
        public string enumKey { get; set; } = "";

        /// <summary>
        /// Text hiển thị
        /// </summary>
        public string enumText { get; set; } = "";
    }
}
