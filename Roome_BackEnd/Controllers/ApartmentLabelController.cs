using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.DAL;
using Roome_BackEnd.BL;
using Roome_BackEnd.Services;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentLabelController : ControllerBase
    {
        private readonly DBserviceApartmentLabel _dbService;

        public ApartmentLabelController()
        {
            _dbService = new DBserviceApartmentLabel();
        }

        // GET: api/ApartmentLabel/{apartmentId}
        [HttpGet("{apartmentId}")]
        public ActionResult<List<ApartmentLabel>> GetLabels(int apartmentId)
        {
            var labels = ApartmentLabel.GetAll(apartmentId);
            return Ok(labels);
        }

        // GET: api/ApartmentLabel/images/{apartmentId}
        [HttpGet("images/{apartmentId}")]
        public ActionResult<List<string>> GetImageUrls(int apartmentId)
        {
            var imageService = new DBserviceUploadImage();
            var imageUrls = imageService.GetImageUrlsForApartment(apartmentId);
            return Ok(imageUrls);
        }

        // POST: api/ApartmentLabel
        [HttpPost]
        public IActionResult AddLabel([FromBody] ApartmentLabel label)
        {
            if (string.IsNullOrWhiteSpace(label.Label) || label.ApartmentId <= 0)
                return BadRequest("Invalid label data.");

            ApartmentLabel.Add(label.ApartmentId, label.Label);
            return Ok("Label added successfully.");
        }

        // PUT: api/ApartmentLabel/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateLabel(int id, [FromBody] string newLabel)
        {
            if (string.IsNullOrWhiteSpace(newLabel))
                return BadRequest("Label must not be empty.");

            ApartmentLabel.Update(id, newLabel);
            return Ok("Label updated successfully.");
        }

        // DELETE: api/ApartmentLabel/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteLabel(int id)
        {
            ApartmentLabel.Delete(id);
            return Ok("Label deleted successfully.");
        }

        // POST: api/ApartmentLabel/detect/{apartmentId}
        [HttpPost("detect/{apartmentId}")]
        public async Task<IActionResult> DetectLabels(int apartmentId)
        {
            var imageService = new DBserviceUploadImage();
            var visionService = new VisionService();

            var imageUrls = imageService.GetImageUrlsForApartment(apartmentId);
            var allLabels = new HashSet<string>();
            string baseUrl = "https://roomebackend20250414140006.azurewebsites.net";

            foreach (var url in imageUrls)
            {
                try
                {
                    string fullUrl = url.StartsWith("http") ? url : $"{baseUrl}{url}";
                    var labels = await visionService.DetectFurnitureLabelsFromUrlAsync(fullUrl);

                    var furnitureKeywords = new List<string>
                    {
                        "couch", "sofa", "armchair", "chair", "bench",
                        "table", "coffee table", "dining table", "desk", "nightstand",
                        "bed", "bunk bed", "mattress", "dresser", "wardrobe", "closet",
                        "tv", "television", "tv stand", "entertainment unit",
                        "lamp", "chandelier", "light fixture",
                        "bookshelf", "bookcase", "shelf", "cabinet", "drawer",
                        "mirror", "rug", "carpet", "curtain", "blinds",
                        "balcony", "patio furniture", "outdoor chair", "outdoor table",
                        "bar stool", "vanity", "ottoman", "bean bag",
                        "recliner", "sideboard", "console table", "shoe rack",    "air conditioner", "ac", "bed", "shower", "washing machine", "dryer",
                        "swimming pool", "pool", "garden", "yard", "balcony", "terrace",
                        "elevator", "parking", "garage", "dishwasher", "microwave", "oven",
                        "fridge", "refrigerator", "stove", "security camera", "intercom", "jacuzzi"
                    };

                    foreach (var label in labels)
                    {
                        if (furnitureKeywords.Contains(label.ToLower()))
                        {
                            allLabels.Add(label);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error analyzing image: {url}, {ex.Message}");
                }
            }

            foreach (var label in allLabels)
            {
                ApartmentLabel.Add(apartmentId, label);
            }

            return Ok(new { apartmentId, labels = allLabels });
        }
    }
}