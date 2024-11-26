using Cms.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace Cms.BL
{
    /// <summary>
    /// Base interface tầng BL
    /// </summary>
    /// @author nktiem 24/11/2024
    public interface IBaseBL<T>
    {
        /// <summary>
        /// Thực hiện Paging data
        /// </summary>
        /// <param name="pagingRequest"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public Task<PagingResult> GetDataPaging(PagingRequest pagingRequest, string viewName);

        /// <summary>
        /// Lấy ra chi tiết một bản ghi theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public Task<dynamic> GetMasterDetail(Guid id, Type? modelType = null);

        /// <summary>
        /// Thực hiện lưu dữ liệu
        /// </summary>
        /// <param name="baseEntitys">đối tượng cần lưu</param>
        /// <param name="dataInactives">Model này phục vụ toggle inactive bản ghi</param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public Task<ServiceResponse> SaveDataAsync(List<BaseEntity> baseEntitys, ModelToggleInactive dataInactives = null);

        /// <summary>
        /// Xử lý dữ liệu sau khi cất còn transaction
        /// </summary>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public Task AfterSaveAsyncWithTransaction(List<BaseEntity> baseEntitys, IDbTransaction transaction);

        /// <summary>
        /// Xử lý dữ liệu sau khi cất ko còn transaction
        /// </summary>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public Task AfterSaveAsync(List<BaseEntity> baseEntitys, ServiceResponse serviceResponse);

        /// <summary>
        /// Hàm xử lý dữ liệu trước khi cất
        /// </summary>
        /// <param name="baseEntitys"></param>
        /// <returns></returns>
        /// @author nktiem 24.11.2024
        public Task BeforeSaveDataAsync(List<BaseEntity> baseEntitys);
    }
}