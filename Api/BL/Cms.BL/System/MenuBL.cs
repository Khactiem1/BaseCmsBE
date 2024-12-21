using Cms.Core.Common;
using Cms.Core.Common.Extension;
using Cms.DL;
using Cms.Model;
using CMS.Core.Database;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Cms.BL
{
    /// <summary>
    /// BL với Menu
    /// </summary>
    /// @author nktiem 24/11/2024
    public class MenuBL : BaseBL<Menu>, IMenuBL
    {
        #region Field

        private IMenuDL _menuDL;

        #endregion

        #region Contructor

        public MenuBL(IMenuDL menuDL, IDatabaseService databaseService) : base(menuDL, databaseService)
        {
            _menuDL = menuDL;
        }

        #endregion
    }
}