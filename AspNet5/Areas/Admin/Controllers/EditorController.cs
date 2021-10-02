using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet5.Areas.Admin.Controllers
{
    public class EditorController : AdminBaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string editorContent)
        {
            return View(model: editorContent);
        }

        /// <summary>
        /// TinyMCE Default image upload button handler.
        /// This handler is called by the function "UploadImage" registered for the "images_upload_handler".
        /// Uploads the given image to the uploads path with a new random name (Guid).
        /// </summary>
        /// <returns>Returns only the source (src) part of the img tag as URI string</returns>
        [HttpPost]
        [Route("/tinymce/image")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Upload([FromServices] IWebHostEnvironment env, IFormFile img)
        {
            try
            {
                FileInfo imageFileInfo = new FileInfo(Path.Combine(env.WebRootPath, "uploads", img.FileName));
                FileInfo imageNewFileInfo = new FileInfo(Path.Combine(imageFileInfo.DirectoryName, $"{Guid.NewGuid().ToString()}{imageFileInfo.Extension}"));

                using (var fileStream = new FileStream(imageNewFileInfo.FullName, FileMode.Create))
                {
                    await img.CopyToAsync(fileStream);
                }

                return Json(new { location = $"{Url.Content("~/uploads")}/{imageNewFileInfo.Name}" });
            }
            catch (Exception ex)
            {
                return Json(new { location = $"{ex.Message}" });
            }
        }

        /// <summary>
        /// TinyMCE Default image upload button handler.
        /// This handler is called by the function "UploadImageBase64" registered for the "images_upload_handler".
        /// Converts the given image to the uploads path with a new random name (Guid).
        /// </summary>
        /// <returns>Converts the given image to base64 data and returns image as data</returns>
        [HttpPost]
        [Route("/tinymce/image-base64")]
        [ValidateAntiForgeryToken]
        public JsonResult UploadBase64(IFormFile img)
        {
            try
            {
                String ImgBase64Src = "data:image/png;base64, 0";

                using (Stream sr = img.OpenReadStream())
                using (BinaryReader br = new BinaryReader(sr))
                {
                    Byte[] imgBytes = br.ReadBytes((Int32)img.Length);
                    String imgBase64 = Convert.ToBase64String(imgBytes, 0, imgBytes.Length);
                    ImgBase64Src = $"data:image/png;base64, {imgBase64}";
                }

                return Json(new { imgData = ImgBase64Src });
            }
            catch (Exception ex)
            {
                return Json(new { imgData = ex.Message });
            }
        }

        private Tuple<int, int> GetImageDimensions(string Base64Image)
        {
            string Base64DataPart = System.Text.RegularExpressions.Regex.Match(Base64Image, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

            byte[] imgBytes = Convert.FromBase64String(Base64DataPart);

            using (var ms = new MemoryStream(imgBytes))
            {
                //System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                //return new Tuple<int, int>(img.Width, img.Height);
                return new Tuple<int, int>(200, 200);
            }
        }
    }
}
