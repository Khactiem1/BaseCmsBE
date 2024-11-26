using Cms.Core.Common;
using Cms.DL;
using Cms.Model;
using CMS.Core.Database;
using System;
using System.Threading.Tasks;

namespace Cms.BL
{
    /// <summary>
    /// BL với nhân viên
    /// </summary>
    /// @author nktiem 24/11/2024
    public class EmployeeBL : BaseBL<Employee>, IEmployeeBL
    {
        #region Field

        private IEmployeeDL _employeeDL;

        #endregion

        #region Contructor

        public EmployeeBL(IEmployeeDL employeeDL, IDatabaseService databaseService) : base(employeeDL, databaseService)
        {
            _employeeDL = employeeDL;
        }

        #endregion
    }
}
