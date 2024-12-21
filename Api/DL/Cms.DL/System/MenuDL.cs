using Cms.Model;
using CMS.Core.Database;

namespace Cms.DL
{
    /// <summary>
    /// DL với Menu
    /// </summary>
    /// @author nktiem 24/11/2024
    public class MenuDL : BaseDL<Menu>, IMenuDL
    {
        public MenuDL(IDatabaseService databaseService) : base(databaseService)
        {

        }
    }
}
