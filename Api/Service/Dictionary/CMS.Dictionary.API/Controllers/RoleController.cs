using Cms.BL;
using Cms.Core.Common;
using Cms.Model;
using CMS.Core.Database;
using CMS.Core.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Dictionary.API
{
    /// <summary>
    /// Tầng Controller Role
    /// </summary>
    /// @author nktiem 24/11/2024
    [Authorize]
    public class RoleController : BaseController<Role>
    {
        #region Field

        private IRoleBL _roleBL;

        #endregion

        #region Contructor

        public RoleController(IRoleBL roleBL) : base(roleBL)
        {
            _roleBL = roleBL;
        }

        #endregion

        #region Method

        /// <summary>
        /// Lấy cấu hình permission theo user_id
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        [HttpGet("get_permission/{user_id}")]
        public async Task<ServiceResponse> GetPermissionByUser([FromRoute] Guid user_id)
        {
            User user = GetCurrentUser();
            object result = null;
            if (user?.user_id == user_id)
            {
                result = await _roleBL.GetPermissionByUser(user_id);
            }
            return new ServiceResponse()
            {
                Data = result,
            };
        }

        #endregion
    }
}
