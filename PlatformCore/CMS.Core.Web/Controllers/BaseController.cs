using System;
using System.Collections.Generic;
using System.Text;
using Cms.BL;
using Cms.Core.Common.Extension;
using Cms.Model;
using Cms.Model.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CMS.Core.Web
{
    /// <summary>
    /// Base Controller dành cho tất cả cả Controller của app
    /// </summary>
    /// @author nktiem 24/11/2024
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase
    {
        #region Field

        private IBaseBL<T> _baseBL;

        #endregion

        #region Contructor

        public BaseController(IBaseBL<T> baseBL)
        {
            _baseBL = baseBL;
        }

        #endregion

        #region Method

        #region API GET

        /// <summary>
        /// Hàm lấy ra bản ghi theo ID
        /// </summary>
        /// <param name="recordID"></param>
        /// <returns>Thông tin chi tiết một bản ghi</returns>
        /// @author nktiem 24.11.2024
        [HttpGet("{recordID}")]
        public virtual async Task<ServiceResponse> GetRecordByID([FromRoute] Guid recordID)
        {
            var result = await _baseBL.GetMasterDetail(recordID);
            return new ServiceResponse()
            {
                Data = result,
            };
        }

        #endregion

        #region API POST

        /// <summary>
        /// Đầu API Save bản ghi
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        [HttpPost]
        public virtual async Task<ServiceResponse> InsertDataAsync([FromBody] T record)
        {
            var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(record), typeof(T));
            baseModel.State = EntityState.Insert;
            return await _baseBL.SaveDataAsync(new List<BaseEntity>() { baseModel });
        }

        /// <summary>
        /// Thực hiện Paging data
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        [HttpPost("datapaging")]
        public virtual async Task<ServiceResponse> GetDataPaging([FromBody] PagingRequest pagingRequest)
        {
            var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new object()), typeof(T));
            var result = new ServiceResponse();
            result.Data = await _baseBL.GetDataPaging(pagingRequest, baseModel.GetType().GetViewNameOnly());
            return result;
        }

        /// <summary> 
        /// API xoá hàng loạt bản ghi
        /// <summary>
        /// <param name="listRecordID">Danh sách ID bản ghi muốn xoá</param>
        /// <return><return>
        /// @author nktiem 24.11.2024
        [HttpPost("delete_multiple")]
        public virtual async Task<ServiceResponse> DeleteMultiple([FromBody] List<Guid> listRecordID)
        {
            var baseEntitys = new List<BaseEntity>();
            foreach (var recordID in listRecordID)
            {
                var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new object()), typeof(T));
                baseModel.SetValue(baseModel.GetType().GetPrimaryKeyFieldName(), recordID);
                baseModel.State = EntityState.Delete;
                baseEntitys.Add(baseModel);
            }
            return await _baseBL.SaveDataAsync(baseEntitys);
        }

        /// <summary> 
        /// Toggle Kích hoạt bản ghi (inactive)
        /// <summary>
        /// <param name="dataInactives"></param>
        /// <return><return>
        /// @author nktiem 24.11.2024
        [HttpPost("update_inactive")]
        public virtual async Task<ServiceResponse> ToggleInactive([FromBody] ModelToggleInactive dataInactives)
        {
            var baseEntitys = new List<BaseEntity>();
            foreach (var recordID in dataInactives.ListID)
            {
                var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new object()), typeof(T));
                baseModel.SetValue(baseModel.GetType().GetPrimaryKeyFieldName(), recordID);
                baseModel.State = EntityState.Inactive;
                baseEntitys.Add(baseModel);
            }
            return await _baseBL.SaveDataAsync(baseEntitys, dataInactives: dataInactives);
        }

        #endregion

        #region API PUT

        /// <summary> 
        /// Đầu API sửa bản ghi
        /// <summary>
        /// <param name="recordID">ID bản ghi muốn cập nhật</param>
        /// <param name="record">Kiểu dữ liệu bản ghi cập nhật</param>
        /// @author nktiem 24.11.2024
        [HttpPut("{recordID}")]
        public virtual async Task<ServiceResponse> UpdateRecord([FromRoute] Guid recordID, [FromBody] T record)
        {
            var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(record), typeof(T));
            baseModel.State = EntityState.Update;
            return await _baseBL.SaveDataAsync(new List<BaseEntity>() { baseModel });
        }

        #endregion

        #region API DELETE

        /// <summary>
        /// API xoá một bản ghi theo ID
        /// <summary>
        /// <param name="recordID">ID bản ghi</param>
        /// @author nktiem 24.11.2024
        [HttpDelete("{recordID}")]
        public virtual async Task<ServiceResponse> DeleteRecord([FromRoute] Guid recordID)
        {
            var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new object()), typeof(T));
            baseModel.SetValue(baseModel.GetType().GetPrimaryKeyFieldName(), recordID);
            baseModel.State = EntityState.Delete;
            return await _baseBL.SaveDataAsync(new List<BaseEntity>() { baseModel });
        }

        #endregion

        #endregion
    }
}
