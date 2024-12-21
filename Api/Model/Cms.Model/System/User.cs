using Cms.Core.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cms.Model
{
    /// <summary>
    /// Model user
    /// </summary>
    [ConfigTable("users", "user_name", "", "SYS")]
    public class User : BaseEntity
    {
        /// <summary>
        /// ID PK
        /// </summary>
        [Key]
        public Guid user_id { get; set; }

        /// <summary>
        /// Mã người dùng
        /// </summary>
        public string? user_name { get; set; }

        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string? user_full_name { get; set; }

        /// <summary>
        /// Avartar
        /// </summary>
        public string? avartar { get; set; }

        /// <summary>
        /// ngày sinh
        /// </summary>
        public DateTime? date_of_birth { get; set; }

        /// <summary>
        /// giới tính
        /// </summary>
        public Gender? gender { get; set; }

        /// <summary>
        /// số điện thoại
        /// </summary>
        public string? phone_number { get; set; }

        /// <summary>
        /// điạ chỉ email
        /// </summary>
        public string? email { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [NotMapped]
        public string? password { get; set; }

        /// <summary>
        /// access_token
        /// </summary>
        [NotMapped]
        public string? access_token { get; set; }

        /// <summary>
        /// Chi tiết quyền
        /// </summary>
        [NotMapped]
        public List<UserJoinRole>? UserJoinRoleDetail { get; set; } = new List<UserJoinRole>();

        /// <summary>
        /// Cấu hình master detail
        /// </summary>
        public User()
        {
            this.ModelDetailConfig.Add(new ModelDetailConfig("user_join_role", "user_id", "UserJoinRoleDetail", typeof(UserJoinRole)));
        }
    }
}
