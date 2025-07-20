using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadImageCpntroller : ControllerBase
    {
        [HttpPost("uploadApartmentImage/{apartmentId}")]
        public async Task<IActionResult> Post([FromForm] List<IFormFile> files, [FromRoute] int apartmentId)
        {
            List<string> imageLinks = new();

            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "uploadedFiles");

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
                    string fullPath = Path.Combine(rootPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    string relativePath = "/uploadedFiles/" + fileName;
                    imageLinks.Add(relativePath);
                }
            }

            if (imageLinks.Count == 0)
            {
                return NotFound("No files uploaded");
            }

            string imageUrlsCsv = string.Join(",", imageLinks);
            object uploadedImages = ApartmantImages.UploadImages(imageUrlsCsv, apartmentId);

            return Ok(imageUrlsCsv);
        }
        [HttpDelete("deleteApartmentImage")]
        public IActionResult DeleteImage([FromQuery] string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest("Image URL is required");
            }

            DBserviceUploadImage db = new();
            bool success = db.DeleteImageByUrl(imageUrl);

            if (success)
            {
                return Ok(new { message = "Image deleted successfully" });
            }
            else
            {
                return NotFound("Image not found or could not be deleted");
            }


        }
        [HttpPost("uploadImageProfile")]
        public async Task<IActionResult> Post([FromForm] List<IFormFile> files)
        {

            List<string> imageLinks = new List<string>();

            string path = System.IO.Directory.GetCurrentDirectory();

            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(path, "uploadedFiles/" + formFile.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    imageLinks.Add(formFile.FileName);
                }
            }

            if (files.Count == 0) { return NotFound(); }

            // Return status code  
            return Ok(imageLinks);

        }
    }
}
