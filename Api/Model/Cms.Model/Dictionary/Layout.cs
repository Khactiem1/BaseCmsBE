using Cms.Core.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Cms.Model
{
    /// <summary>
    /// Model cấu hình layout
    /// </summary>
    [ConfigTable("layout", "layout_id")]
    public class Layout : BaseEntity
    {
        /// <summary>
        /// ID PK
        /// </summary>
        [Key]
        public Guid layout_id { get; set; }

        /// <summary>
        /// Layout Tag
        /// </summary>
        public string? layout_tag { get; set; }

        /// <summary>
        /// Tên layout
        /// </summary>
        public string? layout_name { get; set; }

        /// <summary>
        /// Cấu hình columns
        /// </summary>
        public string? config { get; set; }
    }
}
