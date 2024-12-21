using Cms.BL;
using Cms.Core.Common;
using Cms.DL;
using CMS.Core.Database;
using CMS.Core.Database.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace CMS.Core.Web
{
    /// <summary>
    /// BaseStartup của app
    /// </summary>
    /// @author nktiem 24/11/2024
    public static class BaseStartup
    {
        /// <summary>
        /// DependencyInjection Tiêm cho các tầng xử lý phần logic
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void RegisterAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region BL

            services.AddScoped<IEmployeeBL, EmployeeBL>();
            services.AddScoped<IUserBL, UserBL>();
            services.AddScoped<IMenuBL, MenuBL>();
            services.AddScoped<IRoleBL, RoleBL>();
            services.AddScoped<ILayoutBL, LayoutBL>();

            #endregion


            #region DL

            services.AddScoped<IEmployeeDL, EmployeeDL>();
            services.AddScoped<IUserDL, UserDL>();
            services.AddScoped<IMenuDL, MenuDL>();
            services.AddScoped<IRoleDL, RoleDL>();
            services.AddScoped<ILayoutDL, LayoutDL>();

            #endregion

            services.AddScoped<IDatabaseService, DatabaseService>();
            services.AddScoped<IFileBL, FileBL>();
        }

        /// <summary>
        /// Init config
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void InitConfigGlobal(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CenterConfig>(configuration);
            var centerConfig = new CenterConfig();
            new ConfigureFromConfigurationOptions<CenterConfig>(configuration).Configure(centerConfig);
            ConfigUtil.InitConfig(centerConfig);
        }

        /// <summary>
        /// Cấu hình sử dụng Middleware
        /// </summary>
        /// <param name="services"></param>
        public static void UseMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
        }

        /// <summary>
        /// Cấu hình AddAuthentication
        /// </summary>
        /// <param name="services"></param>
        public static void AddAuthenticationService(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = ConfigUtil.ConfigGlobal.JwtSetting.Issuer,
                    ValidAudience = ConfigUtil.ConfigGlobal.JwtSetting.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigUtil.ConfigGlobal.JwtSetting.Key))
                };
            });
        }

        /// <summary>
        /// Xử lý Cross
        /// </summary>
        /// <param name="services"></param>
        public static void ProcessCross(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
        }

        /// <summary>
        /// Xử lý Cross
        /// </summary>
        /// <param name="app"></param>
        public static void ProcessCross(this IApplicationBuilder app)
        {
            app.UseCors("AllowAll");
        }
    }
}
