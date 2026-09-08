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
                ext = id[dotPosition..];
            }
            string contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
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
