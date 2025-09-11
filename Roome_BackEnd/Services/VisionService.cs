using Google.Cloud.Vision.V1;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roome_BackEnd.Services
{
    public class VisionService
    {
        // Analyze a single image from a public URL
        public async Task<List<string>> DetectFurnitureLabelsFromUrlAsync(string imageUrl)
        {
            var client = await ImageAnnotatorClient.CreateAsync();
            var image = Image.FetchFromUri(imageUrl); 
            var response = await client.DetectLabelsAsync(image);

            return response
                .Where(label => label.Score >= 0.6)
                .Select(label => label.Description.ToLower())
                .ToList();
        }
    }
}
