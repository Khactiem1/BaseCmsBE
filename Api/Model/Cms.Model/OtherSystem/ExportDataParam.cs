using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Model
{
    /// <summary>
    /// Model xử lý xuất excel
    /// </summary>
    public class ExportDataParam
    {
        /// <summary>
        /// Tên file excel
        /// </summary>
        public string? SheetName { get; set; }

        /// <summary>
        /// Tiêu đề cột
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Param paging
        /// </summary>
        public PagingRequest? Param { get; set; }

        /// <summary>
        /// Các cột xuất excel
        /// </summary>
        public List<HeaderColumn> HeaderColumns { get; set; } = new List<HeaderColumn>();
    }
}
