using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cms.Model
{
    /// <summary>
    /// Cấu hình detail cho master
    /// </summary>
    public class ModelDetailConfig
    {
        /// <summary>
        /// Tên bảng detail
        /// </summary>
        public string DetailTableName { get; set; }

        /// <summary>
        /// Cột khoá chính của master
        /// </summary>
        public string ForeignKeyName { get; set; }

        /// <summary>
        /// Tên cột chứa dữ liệu danh sách detail
        /// </summary>
        public string PropertyNameOnMaster { get; set; }

        /// <summary>
        /// Kiểu dữ liệu của detail
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        /// Có xoá detail khi xoá master hay không
        /// </summary>
        public bool CasCaseOnDeleteMasterModel { get; set; } = true;

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        public ModelDetailConfig(string detailTableName, string foreignKeyName, string propertyNameOnMaster, Type modelType, bool casCaseOnDeleteMasterModel = true)
        {
            this.DetailTableName = detailTableName;
            this.ForeignKeyName = foreignKeyName;
            this.PropertyNameOnMaster = propertyNameOnMaster;
            this.ModelType = modelType;
            this.CasCaseOnDeleteMasterModel= casCaseOnDeleteMasterModel;
        }
    }
}
