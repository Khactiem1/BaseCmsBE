using Cms.Core.Common;
using Cms.Core.Common.Extension;
using Cms.DL;
using Cms.Model;
using CMS.Core.Database;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cms.BL
{
    /// <summary>
    /// BL với User
    /// </summary>
    /// @author nktiem 24/11/2024
    public class UserBL : BaseBL<User>, IUserBL
    {
        #region Field

        private IUserDL _userDL;

        #endregion

        #region Contructor

        public UserBL(IUserDL userDL, IDatabaseService databaseService) : base(userDL, databaseService)
        {
            _userDL = userDL;
        }

        #endregion

        /// <summary>
        /// Xử lý sau khi get data ra
        /// </summary>
        /// <returns></returns>
        public override async Task AfterGetFormData(BaseEntity baseEntity)
        {
            var user = (User)baseEntity;
            user.password = null;
        }

        /// <summary>
        /// Hàm thực hiện Login
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ServiceResponse> Authentication(User user)
        {
            User userGet = null;
            string command = $"select * from {typeof(User).GetTableNameOnly()} where user_name = @v_user_name and password = @v_password and (is_deleted = FALSE OR is_deleted is null) and is_active = TRUE";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                { "v_user_name", user.user_name },
                { "v_password", ExtensionUtility.HashPassword(user.password) }
            };
            userGet = (await _databaseService.QueryUsingCommandText<User>(command, param))?.FirstOrDefault();
            if (userGet != null)
            {
                userGet.password = null;
                userGet.access_token = GenerateAccessToken(userGet);
                return new ServiceResponse()
                {
                    Data = userGet,
                };
            }
            return new ServiceResponse()
            {
                Data = false,
            };
        }

        /// <summary>
        /// Reset mật khẩu về mặc định
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public async Task ResetPassword(List<User> users)
        {
            StringBuilder command = new StringBuilder();
            Dictionary<string, object> param = new Dictionary<string, object>();
            int count = 0;
            foreach (var user in users)
            {
                command.Append($"update {typeof(User).GetTableNameOnly()} set password = @v_password where user_id = @v_user_id{count}; ");
                param.Add($"v_user_id{count}", user.user_id);
                count++;
            }
            param.Add("v_password", ConfigUtil.ConfigGlobal.DefaultPassword);
            await _databaseService.ExecuteScalarUsingCommandText<int>(command.ToString(), param);
        }

        /// <summary>
        /// Hàm xử lý Generate tokken từ người dùng đăng nhập hợp lệ
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigUtil.ConfigGlobal.JwtSetting.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
                new Claim(ClaimTypes.Name, user.user_name),
                new Claim(ClaimTypes.GivenName, user.user_full_name),
                new Claim(ClaimTypes.Role, "")
            };
            var token = new JwtSecurityToken(
                ConfigUtil.ConfigGlobal.JwtSetting.Issuer, 
                ConfigUtil.ConfigGlobal.JwtSetting.Audience, 
                claims, 
                expires: DateTime.UtcNow.AddMinutes(ConfigUtil.ConfigGlobal.JwtSetting.ExpirationInMinutes != null ? (int)ConfigUtil.ConfigGlobal.JwtSetting.ExpirationInMinutes : 5256000), 
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}