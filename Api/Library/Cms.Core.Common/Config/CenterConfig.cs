using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Core.Common
{
    /// <summary>
    /// Config chung toàn hệ thống được lấy từ appsettings
    /// </summary>
    public class CenterConfig
    {
        /// <summary>
        /// Cấu hình các ConnectionStrings tới database
        /// </summary>
        public ConnectionStrings connectionStrings { get; set; }
    }
}
