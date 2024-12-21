using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Cms.BL;
using Cms.Core.Common.Extension;
using Cms.Model;
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
        /// Hàm lấy ra thông tin người đang đăng nhập gọi api
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-current-user")]
        public User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new User
                {
                    user_id = Guid.Parse(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value),
                    user_name = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                    user_full_name = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                };
            }
            return null;
        }

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
        /// Khôi phục dữ liệu đã bị xoá theo mã
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("restore_data_delete")]
        public async Task<ServiceResponse> RestoreDataDelete([FromBody] T record)
        {
            User user = GetCurrentUser();
            var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(record), typeof(T));
            baseModel.modified_by = user.user_id;
            return await _baseBL.RestoreDataDelete(baseModel);
        }

        /// <summary>
        /// Export data ra file excel
        /// </summary>
        /// <param name="formData">Trường muốn filter và sắp xếp</param>
        /// <returns>file Excel chứa dữ liệu danh sách </returns>
        /// CreatedBy: Nguyễn Khắc Tiềm (6/10/2022)
        [HttpPost("export_data")]
        public virtual async Task<IActionResult> ExportData([FromBody] ExportDataParam param)
        {
            Stream stream = await _baseBL.ExportData(param);
            return File(stream, "application/octet-stream", $"{param.SheetName}.xlsx");
        }

        /// <summary>
        /// Đầu API Save bản ghi
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        [HttpPost]
        public virtual async Task<ServiceResponse> InsertDataAsync([FromBody] T record)
        {
            User user = GetCurrentUser();
            var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(record), typeof(T));
            baseModel.State = EntityState.Insert;
            baseModel.created_by = user.user_id;
            baseModel.modified_by = user.user_id;
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
            var result = new ServiceResponse();
            result.Data = await _baseBL.GetDataPaging(pagingRequest, typeof(T).GetViewNameOnly());
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
            User user = GetCurrentUser();
            foreach (var recordID in listRecordID)
            {
                var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new object()), typeof(T));
                baseModel.SetValue(baseModel.GetType().GetPrimaryKeyFieldName(), recordID);
                baseModel.State = EntityState.Delete;
                baseModel.deleted_by = user.user_id;
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
            User user = GetCurrentUser();
            foreach (var recordID in dataInactives.ListID)
            {
                var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new object()), typeof(T));
                baseModel.SetValue(baseModel.GetType().GetPrimaryKeyFieldName(), recordID);
                baseModel.State = EntityState.Inactive;
                baseModel.modified_by = user.user_id;
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
            User user = GetCurrentUser();
            var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(record), typeof(T));
            baseModel.State = EntityState.Update;
            baseModel.modified_by = user.user_id;
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
            User user = GetCurrentUser();
            var baseModel = (BaseEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new object()), typeof(T));
            baseModel.SetValue(baseModel.GetType().GetPrimaryKeyFieldName(), recordID);
            baseModel.State = EntityState.Delete;
            baseModel.deleted_by = user.user_id;
            return await _baseBL.SaveDataAsync(new List<BaseEntity>() { baseModel });
        }

        #endregion

        #endregion
    }
}
