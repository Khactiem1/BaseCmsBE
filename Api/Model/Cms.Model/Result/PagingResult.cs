using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Model
{
    public class PagingResult
    {
        /// <summary>
        /// Data paging được
        /// </summary>
        public List<object> PageData { get; set; } = new List<object>();

        /// <summary>
        /// Số lượng bản ghi
        /// </summary>
        public int Count { get; set; } = 0;
    }
}
