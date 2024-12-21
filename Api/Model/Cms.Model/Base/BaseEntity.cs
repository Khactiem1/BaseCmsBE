using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cms.Model
{
    /// <summary>
    /// BaseEntity model
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Người tạo
        /// </summary>
        public Guid? created_by { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? created_date { get; set; }

        /// <summary>
        /// Người sửa
        /// </summary>
        public Guid? modified_by { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        public DateTime? modified_date { get; set; }

        /// <summary>
        /// Người xoá
        /// </summary>
        public Guid? deleted_by { get; set; }

        /// <summary>
        /// Ngày xoá
        /// </summary>
        public DateTime? deleted_date { get; set; }

        /// <summary>
        /// Đã xoá hay chưa
        /// </summary>
        [NotMapped]
        public bool? is_deleted { get; set; } = false;

        /// <summary>
        /// Có xoá cứng trong database hay không
        /// </summary>
        [NotMapped]
        public bool? is_delete_in_database { get; set; } = false;

        /// <summary>
        /// Ngừng sử dụng
        /// </summary>
        public bool? is_active { get; set; } = true;

        /// <summary>
        /// Mode thêm hay sửa hay xoá (Mặc định là None ko làm gì)
        /// </summary>
        [NotMapped]
        public EntityState? State { get; set; } = EntityState.None;

        /// <summary>
        /// Danh sách các detail được cấu hình ở master
        /// </summary>
        [NotMapped]
        [IgnoreDataMember]
        [JsonIgnore]
        public List<ModelDetailConfig> ModelDetailConfig { get; set; } = new List<ModelDetailConfig>();
    }
}
