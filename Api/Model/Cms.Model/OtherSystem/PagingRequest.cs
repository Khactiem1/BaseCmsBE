using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Model
{
    public class PagingRequest
    {
        /// <summary>
        /// Số lượng bản ghi trên một trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Trang số bao nhiêu
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Cột cần lấy
        /// </summary>
        public string? Columns { get; set; }

        /// <summary>
        /// String filter của Grid
        /// </summary>
        public string? Filter { get; set; }

        /// <summary>
        /// String Sort của Grid
        /// </summary>
        public string? Sort { get; set; }

        /// <summary>
        /// Tham số truyền lên thêm API
        /// </summary>
        public Dictionary<string, object> CustomParam { get; set; } = new Dictionary<string, object>();
    }
}
