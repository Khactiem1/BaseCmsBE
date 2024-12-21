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

        /// <summary>
        /// Cấu hình DefaultPassword khi khôi phục mật khẩu
        /// </summary>
        public string DefaultPassword { get; set; }

        /// <summary>
        /// Cấu hình file FileSettings
        /// </summary>
        public FileSettings FileSettings { get; set; }

        /// <summary>
        /// JwtSetting
        /// </summary>
        public JwtSetting JwtSetting { get; set; }
    }

    /// <summary>
    /// FileSettings
    /// </summary>
    public class FileSettings
    {
        /// <summary>
        /// Cấu hình đường dẫn file root
        /// </summary>
        public string PathRoot { get; set; }

        /// <summary>
        /// Cấu hình đường dẫn file root
        /// </summary>
        public int MaxFileSize { get; set; }
    }

    /// <summary>
    /// JwtSetting
    /// </summary>
    public class JwtSetting
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Issuer
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Audience
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// ExpirationInMinutes
        /// </summary>
        public int? ExpirationInMinutes { get; set; }
    }
}
