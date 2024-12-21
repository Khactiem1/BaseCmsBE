using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Model
{
    /// <summary>
    /// Response lỗi trả về khi validate xử lý dữ liệu
    /// </summary>
    public class ValidateResult
    {
        /// <summary>
        /// ID bản ghi lỗi
        /// </summary>
        public object ID { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public EnumValidateResult Code { get; set; }

        /// <summary>
        /// Nội dung lỗi
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
