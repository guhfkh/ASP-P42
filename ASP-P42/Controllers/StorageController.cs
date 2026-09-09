using ASP_P42.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P42.Controllers
{
    public class StorageController(IStorageService storageService) : Controller
    {
        private readonly IStorageService _storageService = storageService;

        [HttpGet]
        public IActionResult Image(string id)
        {
            int dotPosition = id.LastIndexOf(".");
            string ext = "";
            if (dotPosition > 0)
            {
                ext = id[dotPosition..].ToLower();
            }
            string contentType = ext switch 
            { 
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                ".ico" => "image/x-icon",
                ".tif" or ".tiff" => "image/tiff",
                ".avif" => "image/avif",
                _ => "application/octet-stream", 
            };

            try
            {
                return File(_storageService.Load(id), contentType);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
