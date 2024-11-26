using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Model
{
    public class ModelToggleInactive
    {
        /// <summary>
        /// Có Inactive cho con ko (Dùng cho danh mục hình cây)
        /// </summary>
        public bool ForceChild { get; set; }

        /// <summary>
        /// Kích hoạt hay ngưng kích hoạt
        /// </summary>
        public bool Inactive { get; set; }

        /// <summary>
        /// ID bản ghi cần Toggle Inactive
        /// </summary>
        public List<Guid> ListID { get; set; } = new List<Guid>();
    }
}
