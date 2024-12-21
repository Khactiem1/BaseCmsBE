using Cms.Model;
using CMS.Core.Database;

namespace Cms.DL
{
    /// <summary>
    /// DL với User
    /// </summary>
    /// @author nktiem 24/11/2024
    public class UserDL : BaseDL<User>, IUserDL
    {
        public UserDL(IDatabaseService databaseService) : base(databaseService)
        {

        }
    }
}
