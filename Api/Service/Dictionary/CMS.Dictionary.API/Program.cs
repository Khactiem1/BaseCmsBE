using CMS.Core.Web;

var builder = WebApplication.CreateBuilder(args);

// Xóa cấu hình mặc định nếu cần
builder.Configuration.Sources.Clear();

// Thêm file cấu hình từ đường dẫn tùy chỉnh
builder.Configuration
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Config"))
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
