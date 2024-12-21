using Cms.Core.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cms.Model
{
    /// <summary>
    /// Model Menu
    /// </summary>
    [ConfigTable("menu", "menu_code", "", "SYS")]
    public class Menu : BaseEntity
    {
        /// <summary>
        /// ID PK
        /// </summary>
        [Key]
        public Guid menu_id { get; set; }

        /// <summary>
        /// ID cha
        /// </summary>
        public Guid? parent_id { get; set; }

        /// <summary>
        /// Mã menu
        /// </summary>
        public string? menu_code { get; set; }

        /// <summary>
        /// Tên menu
        /// </summary>
        public string? menu_name { get; set; }

        /// <summary>
        /// icon
        /// </summary>
        public string? icon { get; set; }

        /// <summary>
        /// thứ tự sắp xếp
        /// </summary>
        public int? sort { get; set; }

        /// <summary>
        /// url truy cập
        /// </summary>
        public string? url { get; set; }
    }
}
