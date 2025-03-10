using Microsoft.OpenApi.Any;
using Microsoft.VisualBasic;
using Roome_BackEnd.DAL;

/*
namespace Roome_BackEnd.BL
{
    public class ApartmentService{
        private readonly DBserviceApartment dbService;
        public ApartmentService(DBserviceApartment dBservice){
            _dbService= dBservice;
        }
        public List<AbstractApartment> GetAllApartments(){
            List<RentalApartment> rentals = _dbService.GetAllRentals();
            List<SharedApartment> sharedApartments = _dbService.GetAllSharedApartments();
              List<AbstractApartment> allApartments = new List<AbstractApartment>();
        allApartments.AddRange(rentals);
        allApartments.AddRange(sharedApartments);

        return allApartments;
        }
    }
}
*/