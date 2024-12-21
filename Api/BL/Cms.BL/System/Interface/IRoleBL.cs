using Cms.Model;

namespace Cms.BL
{
    /// <summary>
    /// interface BL với Role
    /// </summary>
    /// @author nktiem 24/11/2024
    public interface IRoleBL : IBaseBL<Role>
    {
        /// <summary>
        /// Lấy cấu hình permission theo user_id
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public Task<object> GetPermissionByUser(Guid user_id);
    }
}
