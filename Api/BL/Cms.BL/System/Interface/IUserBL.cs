using Cms.Model;

namespace Cms.BL
{
    /// <summary>
    /// interface BL với User
    /// </summary>
    /// @author nktiem 24/11/2024
    public interface IUserBL : IBaseBL<User>
    {
        /// <summary>
        /// Hàm thực hiện Login
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<ServiceResponse> Authentication(User user);

        /// <summary>
        /// Reset mật khẩu về mặc định
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public Task ResetPassword(List<User> users);
    }
}
