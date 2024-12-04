using Cms.BL;
using Cms.Core.Common;
using Cms.Model;
using CMS.Core.Database;
using CMS.Core.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Dictionary.API
{
    /// <summary>
    /// Tầng Controller cấu hình layout
    /// </summary>
    /// @author nktiem 24/11/2024
    //[Authorize]
    public class LayoutController : BaseController<Layout>
    {
        #region Field

        private ILayoutBL _layoutBL;

        #endregion

        #region Contructor

        public LayoutController(ILayoutBL layoutBL) : base(layoutBL)
        {
            _layoutBL = layoutBL;
        }

        #endregion

        #region Method

        /// <summary>
        /// Lấy cấu hình layout theo Tag
        /// </summary>
        /// <param name="tag"></param>
        /// @author nktiem 24.11.2024
        [HttpGet("get-layout-by-tag/{tag}")]
        public virtual async Task<ServiceResponse> GetLayoutByTag([FromRoute] string tag)
        {
            var result = await _layoutBL.GetLayoutByTag(tag);
            return new ServiceResponse()
            {
                Data = result,
            };
        }

        /// <summary>
        /// Cập nhật cấu hình layout theo Tag
        /// </summary>
        /// <param name="tag"></param>
        /// @author nktiem 24.11.2024
        [HttpPost("update-layout-by-tag/{tag}")]
        public virtual async Task<ServiceResponse> UpdateLayoutByTag([FromRoute] string tag, [FromBody] Layout layout)
        {
            await _layoutBL.UpdateLayoutByTag(tag, layout);
            return new ServiceResponse()
            {
                Data = true,
            };
        }

        #endregion
    }
}
