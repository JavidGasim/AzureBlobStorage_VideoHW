using System.Diagnostics;
using AzureBlobStorage_VideoHW.Models;
using AzureBlobStorage_VideoHW.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobStorage_VideoHW.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlobStorageService _blobStorageService;
        public HomeController(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty");
            }

            using var stream = file.OpenReadStream();
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var videoUrl = await _blobStorageService.UploadAsync(stream, fileName);
            return RedirectToAction("Index");
        }

        [HttpGet("download/{filename}")]
        public async Task<IActionResult> DownloadVideo(string filename)
        {
            var stream = await _blobStorageService.DownloadAsync(filename);
            return File(stream, "video/mp4", filename);
        }

        public async Task<IActionResult> Index()
        {
            var videoUrls = await _blobStorageService.ListFilesAsync();
            return View(videoUrls);
        }
    }
}
