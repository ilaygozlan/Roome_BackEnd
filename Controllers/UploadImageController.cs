using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.DAL;
using System.Diagnostics;
using Roome_BackEnd.Models;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadImageController : ControllerBase
    {/*
        [HttpPost("addLabel")]
        public IActionResult AddLabel([FromBody] LabelDTO label)
        {
            try
            {
                DBserviceUploadImage db = new();
                db.InsertLabel(label.ImageId, label.ImageFile, label.PredictedLabel);
                return Ok(new { message = "Label added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to add label: {ex.Message}");
            }
        }
*/
        [HttpDelete("deleteLabel")]
        public IActionResult DeleteLabel([FromQuery] int apartmentId, [FromQuery] string imagePath, [FromQuery] string predictedLabel)
        {
            try
            {
                DBserviceUploadImage db = new();
                db.DeleteLabel(apartmentId, imagePath, predictedLabel);
                return Ok(new { message = "Label deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to delete label: {ex.Message}");
            }
        }

        [HttpPost("detectLabelsForApartment/{apartmentId}")]
        public IActionResult DetectLabelsForApartment(int apartmentId)
        {
            try
            {
                DBserviceUploadImage dbService = new();

                // Get all image paths for this apartment from DB
                List<string> imagePaths = dbService.GetImagesByApartmentId(apartmentId);

                // Detect and insert labels into DB
                dbService.DetectAndSaveLabels(apartmentId, imagePaths);

                return Ok(new { message = "Labels detected and saved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during label detection: {ex.Message}");
            }
        }

        [HttpPost("uploadApartmentImage/{apartmentId}")]
        public async Task<IActionResult> Post([FromForm] List<IFormFile> files, [FromRoute] int apartmentId)
        {
            List<string> imageLinks = new();

            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploadedFiles");
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            DBserviceUploadImage dbService = new();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
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
                return NotFound("No files uploaded");

            dbService.UploadImages(string.Join(",", imageLinks), apartmentId);
            dbService.DetectAndSaveLabels(apartmentId, imageLinks);

            return Ok(new
            {
                ApartmentId = apartmentId,
                UploadedImages = imageLinks
            });
        }

        [HttpGet("GetLabelSummaryByApartment/{apartmentId}")]
        public IActionResult GetLabelSummaryByApartment(int apartmentId)
        {
            DBserviceUploadImage dbService = new();
            var summary = dbService.GetLabelSummaryByApartmentId(apartmentId);

            return Ok(summary ?? new List<Dictionary<string, object>>());
        }

        [HttpPost("detectLabelsFromForm")]
        public async Task<IActionResult> DetectLabelsFromForm([FromForm] List<IFormFile> files)
        {
            List<string> tempPaths = new();
            try
            {
                string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tempDetect");
                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder);

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
                        string fullPath = Path.Combine(tempFolder, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
Console.WriteLine($"Image saved at: {fullPath}");

                        tempPaths.Add(fullPath);
                    }
                }
                Console.WriteLine(tempPaths);

                if (tempPaths.Count == 0)
                    return BadRequest("No files provided.");

                DBserviceUploadImage dbService = new();
                List<string> detectedLabels = new();
                detectedLabels.AddRange(dbService.DetectLabelsWithoutSaving(tempPaths));


                return Ok(new { DetectedLabels = detectedLabels });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Temporary detection failed: {ex.Message}");
            }
        }
    }
}
