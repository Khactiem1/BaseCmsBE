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
    /// Tầng Controller Menu
    /// </summary>
    /// @author nktiem 24/11/2024
    [Authorize]
    public class MenuController : BaseController<Menu>
    {
        #region Field

        private IMenuBL _menuBL;

        #endregion

        #region Contructor

        public MenuController(IMenuBL menuBL) : base(menuBL)
        {
            _menuBL = menuBL;
        }

        #endregion

        #region Method

        #endregion
    }
}
