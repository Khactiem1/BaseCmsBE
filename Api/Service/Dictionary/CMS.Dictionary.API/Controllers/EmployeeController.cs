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
    /// Tầng Controller nhân viên
    /// </summary>
    /// @author nktiem 24/11/2024
    //[Authorize]
    public class EmployeeController : BaseController<Employee>
    {
        #region Field

        private IEmployeeBL _employeeBL;

        #endregion

        #region Contructor

        public EmployeeController(IEmployeeBL employeeBL) : base(employeeBL)
        {
            _employeeBL = employeeBL;
        }

        #endregion

        #region Method

        #endregion
    }
}
