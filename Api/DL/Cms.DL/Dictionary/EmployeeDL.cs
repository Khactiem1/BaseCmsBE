using Cms.Model;
using CMS.Core.Database;

namespace Cms.DL
{
    /// <summary>
    /// DL với nhân viên
    /// </summary>
    /// @author nktiem 24/11/2024
    public class EmployeeDL : BaseDL<Employee>, IEmployeeDL
    {
        public EmployeeDL(IDatabaseService databaseService) : base(databaseService)
        {

        }
    }
}
