using Microsoft.OpenApi.Any;
using Microsoft.VisualBasic;
using Roome_BackEnd.DAL;


namespace Roome_BackEnd.BL
{
    public class ApartmantImages{
        private int id;
        private string imageUrl="";
        private int apartmentId;
public ApartmantImages(int id, string imageUrl, int apartmentId)
        {
            Id = id;
            ImageUrl = imageUrl;
            ApartmentId = apartmentId;
        }

        public int Id { get => id; set => id = value; }
        public string ImageUrl { get => imageUrl; set => imageUrl = value; }
        public int ApartmentId { get => apartmentId; set => apartmentId = value; }
    }
}