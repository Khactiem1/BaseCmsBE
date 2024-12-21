using Cms.Core.Common;
using CMS.Core.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configPathDev = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Config");
if (Directory.Exists(configPathDev))
{
    builder.Configuration.Sources.Clear();
    builder.Configuration
        .SetBasePath(configPathDev)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
}

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Giữ nguyên tên thuộc tính
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterAppServices(builder.Configuration);
builder.Services.InitConfigGlobal(builder.Configuration);
builder.Services.ProcessCross();

builder.Services.AddAuthenticationService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware();
app.ProcessCross();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();