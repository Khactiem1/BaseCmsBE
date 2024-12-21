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
    /// BL với Role
    /// </summary>
    /// @author nktiem 24/11/2024
    public class RoleBL : BaseBL<Role>, IRoleBL
    {
        #region Field

        private IRoleDL _roleDL;

        #endregion

        #region Contructor

        public RoleBL(IRoleDL roleDL, IDatabaseService databaseService) : base(roleDL, databaseService)
        {
            _roleDL = roleDL;
        }

        #endregion

        /// <summary>
        /// Lấy cấu hình permission theo user_id
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public async Task<object> GetPermissionByUser(Guid user_id)
        {
            var sql = $"select * from \"SYS\".sys_view_get_permission where user_id = @v_user_id;";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "v_user_id", user_id }
            };
            return await _databaseService.QueryUsingCommandText<object>(sql, param);
        }
    }
}