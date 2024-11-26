using Cms.Core.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Cms.Model
{
    /// <summary>
    /// Model nhân viên
    /// </summary>
    [ConfigTable("employee", "employee_code")]
    public class Employee : BaseEntity
    {
        /// <summary>
        /// ID PK
        /// </summary>
        [Key]
        public Guid employee_id { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        public string? employee_code { get; set; }

        /// <summary>
        /// Tên nhân viên
        /// </summary>
        public string? employee_name { get; set; }

        /// <summary>
        /// ngày sinh
        /// </summary>
        public DateTime? date_of_birth { get; set; }

        /// <summary>
        /// giới tính
        /// </summary>
        public Gender? gender { get; set; }

        /// <summary>
        /// chứng minh thư
        /// </summary>
        public string? identity_card { get; set; }

        /// <summary>
        /// chi nhánh ngân hàng
        /// </summary>
        public string? branch_bank { get; set; }

        /// <summary>
        /// chức danh
        /// </summary>
        public string? employee_title { get; set; }

        /// <summary>
        /// số tài khoản
        /// </summary>
        public string? bank_account { get; set; }

        /// <summary>
        /// tên ngân hàng
        /// </summary>
        public string? name_bank { get; set; }

        /// <summary>
        /// ngày cấp cmnd
        /// </summary>
        public DateTime? day_for_identity { get; set; }

        /// <summary>
        /// địa chỉ cấp cmnd
        /// </summary>
        public string? grant_address_identity { get; set; }

        /// <summary>
        /// số điện thoại
        /// </summary>
        public string? phone_number { get; set; }

        /// <summary>
        /// số điện thoại cố định
        /// </summary>
        public string? landline_phone { get; set; }

        /// <summary>
        /// điạ chỉ email
        /// </summary>
        public string? employee_email { get; set; }

        /// <summary>
        /// địa chỉ nhân viên
        /// </summary>
        public string? employee_address { get; set; }

        /// <summary>
        /// Có là khách hàng
        /// </summary>
        public bool? is_customer { get; set; } = false;

        /// <summary>
        /// Có là nhà cung cấp
        /// </summary>
        public bool? is_vendor { get; set; } = false;

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string? password { get; set; }
    }
}
