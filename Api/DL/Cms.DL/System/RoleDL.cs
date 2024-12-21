using Cms.Model;
using CMS.Core.Database;

namespace Cms.DL
{
    /// <summary>
    /// DL với Role
    /// </summary>
    /// @author nktiem 24/11/2024
    public class RoleDL : BaseDL<Role>, IRoleDL
    {
        public RoleDL(IDatabaseService databaseService) : base(databaseService)
        {

        }
    }
}
