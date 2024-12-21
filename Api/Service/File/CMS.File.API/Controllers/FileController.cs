using Cms.BL;
using Cms.Core.Common;
using Cms.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace CMS.File.API
{
    /// <summary>
    /// Tầng Controller File
    /// </summary>
    /// @author nktiem 24/11/2024
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        #region Field

        private IFileBL _fileBL;

        #endregion

        #region Contructor

        public FileController(IFileBL fileBL)
        {
            _fileBL = fileBL;
        }

        #endregion

        #region Method

        /// <summary>
        /// Upload file lên serve
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<ServiceResponse> UploadFile(IFormFile file, [FromQuery] string path)
        {
            if (file == null || file.Length == 0)
                return new ServiceResponse()
                {
                    Success = false,
                    Data = "File is empty or not provided.",
                };
            if (path.Contains("..") || Path.IsPathFullyQualified(path))
            {
                return new ServiceResponse()
                {
                    Success = false,
                    Data = "Invalid path.",
                };
            }
            var maxFileSize = ConfigUtil.ConfigGlobal.FileSettings.MaxFileSize;
            if (file.Length > maxFileSize)
            {
                return new ServiceResponse()
                {
                    Success = false,
                    Data = $"File size exceeds the maximum limit of {maxFileSize / (1024 * 1024)}MB.",
                };
            }
            // Kết hợp đường dẫn gốc và path
            var targetDirectory = Path.Combine($"{ConfigUtil.ConfigGlobal.FileSettings.PathRoot}{path}");

            // Tạo thư mục nếu chưa tồn tại
            Directory.CreateDirectory(targetDirectory);

            // Lấy phần đuôi file (extension)
            string fileExtension = Path.GetExtension(file.FileName);

            // Tạo tên file với GUID để tránh trùng lặp
            string fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(targetDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new ServiceResponse()
            {
                Data = $"{path}/{fileName}",
            };
        }

        /// <summary>
        /// Lấy file từ serve
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("view")]
        [AllowAnonymous]
        public IActionResult ViewFile([FromQuery] string fileName)
        {
            var filePath = Path.Combine($"{ConfigUtil.ConfigGlobal.FileSettings.PathRoot}{fileName}");

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found."); // Trả về thông báo lỗi

            // Lấy Content-Type dựa trên loại file
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream"; // Content-Type mặc định nếu không xác định được
            }

            // Trả về file trực tiếp
            return PhysicalFile(filePath, contentType);
        }

        #endregion
    }
}
