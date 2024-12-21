using Cms.Core.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cms.Model
{
    /// <summary>
    /// Model RoleDetail
    /// </summary>
    [ConfigTable("role_detail", "", "", "SYS")]
    public class RoleDetail : BaseEntity
    {
        /// <summary>
        /// ID PK
        /// </summary>
        [Key]
        public Guid role_detail_id { get; set; }

        /// <summary>
        /// FK
        /// </summary>
        public Guid role_id { get; set; }

        /// <summary>
        /// Mã quyền sub_system_code
        /// </summary>
        public string? sub_system_code { get; set; }

        /// <summary>
        /// List quyền list_permission
        /// </summary>
        public string? list_permission { get; set; }
    }
}
