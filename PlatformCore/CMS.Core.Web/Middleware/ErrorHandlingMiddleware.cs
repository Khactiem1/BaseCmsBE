using Cms.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core.Web
{
    /// <summary>
    /// Middleware bắt exception toàn ứng dụng
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = new ServiceResponse
            {
                Success = false,
                ErrorCode = 500,
                Data = null,
                DevMessage = exception.Message.ToString(),
                UserMessage = "Có lỗi xảy ra, hãy liên hệ bộ phận hỗ trợ kỹ thuật",
            };
            var resultJson = JsonConvert.SerializeObject(result);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(resultJson);
        }
    }
}
