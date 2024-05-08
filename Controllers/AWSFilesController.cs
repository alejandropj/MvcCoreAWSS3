using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MvcCoreAWSS3.Services;

namespace MvcCoreAWSS3.Controllers
{
    public class AWSFilesController : Controller
    {
        private ServiceStorageS3 service;
        private string url;

        public AWSFilesController(ServiceStorageS3 service, IConfiguration config)
        {
            this.service = service;
            this.url = config.GetValue<string>("AWS:BucketURL");
        }
        public async Task< IActionResult> Index()
        {
            List<string> filesS3 = 
                await this.service.GetVersionsFileAsync();
            ViewData["DATOS"] = url;
            return View(filesS3);
        }  
        public async Task< IActionResult> UploadFile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile
            (IFormFile file)
        {
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadFileAsync
                    (file.FileName, stream);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteFile(string fileName)
        {
            await this.service.DeleteFileAsync(fileName);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetPrivateFile(string fileName)
        {
            Stream stream = await this.service.GetFileAsync(fileName);
            return File(stream,"image/png");
        }
    }
}
