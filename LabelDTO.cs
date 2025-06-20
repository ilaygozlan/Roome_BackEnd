using Microsoft.AspNetCore.Http;

namespace Roome_BackEnd.Models
{
    public class LabelDTO
    {
        public int ImageId { get; set; }

        // This represents the uploaded file from the form
        public IFormFile ImageFile { get; set; }

        // This is the predicted label (e.g., "sofa", "kitchen")
        public string PredictedLabel { get; set; }
    }
}
