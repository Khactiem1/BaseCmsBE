using Cms.Core.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cms.Model
{
    /// <summary>
    /// Model user_join_role
    /// </summary>
    [ConfigTable("user_join_role", "", "", "SYS")]
    public class UserJoinRole : BaseEntity
    {
        /// <summary>
        /// ID PK
        /// </summary>
        [Key]
        public Guid user_join_role_id { get; set; }

        /// <summary>
        /// FK
        /// </summary>
        public Guid role_id { get; set; }

        /// <summary>
        /// ID user
        /// </summary>
        public Guid user_id { get; set; }
    }
}
