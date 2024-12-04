using Cms.Model;
using CMS.Core.Database;

namespace Cms.DL
{
    /// <summary>
    /// DL với layout
    /// </summary>
    /// @author nktiem 24/11/2024
    public class LayoutDL : BaseDL<Layout>, ILayoutDL
    {
        public LayoutDL(IDatabaseService databaseService) : base(databaseService)
        {

        }
    }
}
