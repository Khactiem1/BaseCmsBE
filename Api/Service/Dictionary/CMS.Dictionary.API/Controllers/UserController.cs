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
    /// Tầng Controller User
    /// </summary>
    /// @author nktiem 24/11/2024
    [Authorize]
    public class UserController : BaseController<User>
    {
        #region Field

        private IUserBL _userBL;

        #endregion

        #region Contructor

        public UserController(IUserBL userBL) : base(userBL)
        {
            _userBL = userBL;
        }

        #endregion

        #region Method

        /// <summary>
        /// API Login
        /// </summary>
        /// @author nktiem 24.11.2024
        [AllowAnonymous]
        [HttpPost("Login")]
        public virtual async Task<ServiceResponse> Authentication([FromBody] User user)
        {
            return await _userBL.Authentication(user);
        }

        /// <summary>
        /// API Reset mật khẩu về mặc định
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("reset_password")]
        public virtual async Task<ServiceResponse> ResetPassword([FromBody] List<User> users)
        {
            await _userBL.ResetPassword(users);
            return new ServiceResponse()
            {
                Data = true,
            };
        }

        #endregion
    }
}
