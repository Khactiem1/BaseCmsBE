using Cms.Core.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cms.Model
{
    /// <summary>
    /// Model Role
    /// </summary>
    [ConfigTable("role", "role_code", "", "SYS")]
    public class Role : BaseEntity
    {
        /// <summary>
        /// ID PK
        /// </summary>
        [Key]
        public Guid role_id { get; set; }

        /// <summary>
        /// Mã quyền
        /// </summary>
        public string? role_code { get; set; }

        /// <summary>
        /// Tên quyền
        /// </summary>
        public string? role_name { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// Mã quyền
        /// </summary>
        public bool? is_administrator { get; set; } = false;

        /// <summary>
        /// Chi tiết quyền
        /// </summary>
        [NotMapped]
        public List<RoleDetail> RoleDetail { get; set; } = new List<RoleDetail>();

        /// <summary>
        /// Cấu hình master detail
        /// </summary>
        public Role() 
        {
            this.ModelDetailConfig.Add(new ModelDetailConfig("role_detail", "role_id", "RoleDetail", typeof(RoleDetail)));
        }
    }
}
