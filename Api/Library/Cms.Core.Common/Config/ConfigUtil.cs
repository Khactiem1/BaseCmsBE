using System;

namespace Cms.Core.Common
{
    /// <summary>
    /// Khởi tạo config chung
    /// </summary>
    /// @author nktiem 24/11/2024
    public class ConfigUtil
    {
        static CenterConfig _centerConfig = null;

        public static CenterConfig ConfigGlobal
        {
            get { return _centerConfig; }
        }

        /// <summary>
        /// Khởi tạo thông tin center config khi start ứng dụng (Chỉ gọi 1 lần duy nhất khi ứng dụng startup)
        /// </summary>
        /// <param name="centerConfig"></param>
        public static void InitConfig(CenterConfig centerConfig)
        {
            if (centerConfig == null)
            {
                throw new ArgumentNullException("DEV: centerConfig can not null.");
            }
            _centerConfig = centerConfig;
        }
    }
}
