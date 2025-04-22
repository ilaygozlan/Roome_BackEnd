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

        [HttpPost("uploadApartmentImage/{apartmentId}")]
        public async Task<IActionResult> Post([FromForm] List<IFormFile> files, [FromRoute] int apartmentId)
        {
            List<string> imageLinks = new List<string>();

            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploadedFiles");

         
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName); // למניעת כפילויות
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

            return Ok(uploadedImages);
        }
    }
    }





