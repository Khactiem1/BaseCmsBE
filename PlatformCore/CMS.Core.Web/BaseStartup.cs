using Cms.BL;
using Cms.Core.Common;
using Cms.DL;
using CMS.Core.Database;
using CMS.Core.Database.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

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
            services.AddScoped<ILayoutBL, LayoutBL>();

            #endregion


            #region DL

            services.AddScoped<IEmployeeDL, EmployeeDL>();
            services.AddScoped<ILayoutDL, LayoutDL>();

            #endregion

            services.AddScoped<IDatabaseService, DatabaseService>();
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
