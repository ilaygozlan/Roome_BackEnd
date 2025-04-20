using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;
using Newtonsoft.Json;
using System;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadImageCpntroller : ControllerBase
    {

        // POST api/<uploadController>
        [HttpPost("uploadApartmentImage/{apartmentId}")]
        public async Task<IActionResult> Post([FromForm] List<IFormFile> files, [FromRoute] int apartmentId)
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

            if(files.Count == 0) { return NotFound(); }

            // Return status code 
            string imageUrlsCsv = string.Join(",", imageLinks); // Convert List<string> to CSV string
            object uploadedImages = ApartmantImages.UploadImages(imageUrlsCsv, apartmentId);
            return Ok(uploadedImages);

        }

        [HttpPost("uploadProfileImage")]
        public async Task<IActionResult> UploadSingleImage([FromForm] IFormFile file)
        {
        
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
      
            string rootPath = Directory.GetCurrentDirectory();
            string uploadDir = Path.Combine(rootPath, "wwwroot", "uploadedFiles");


            string filePath = Path.Combine(uploadDir, file.FileName);

            
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

           
            string imageUrl = $"uploadedFiles/{file.FileName}";

         
            return Ok(new { url = imageUrl });
        }


    }
}





