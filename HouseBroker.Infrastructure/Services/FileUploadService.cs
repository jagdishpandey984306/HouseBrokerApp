using HouseBroker.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Infrastructure.Services
{
    public class FileUploadService : IFileUploadService
    {
        #region MyRegion
        private readonly IHttpContextAccessor _httpContext;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly string _uploadFolder;
        #endregion

        #region Ctor
        public FileUploadService(IHttpContextAccessor httpContext, IHostingEnvironment hostEnvironment)
        {
            _httpContext = httpContext;
            _hostEnvironment = hostEnvironment;
            _uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Uploads");
        }
        #endregion

        #region Methods
        /// <summary>
        /// it return the image url
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string UploadFile(IFormFile file)
        {
            string fullUrl = string.Empty;
            var baseUrl = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}";
            if (file == null || file.Length == 0)
                return fullUrl;

            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid().ToString() + fileExtension;
            var filePath = Path.Combine(_uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var relativePath = $"uploads/{fileName}";
            fullUrl = $"{baseUrl}/{relativePath}";

            return fullUrl;
        }
        #endregion
    }
}
