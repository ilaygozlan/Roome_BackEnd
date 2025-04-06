using Roome_BackEnd.DAL;

namespace Roome_BackEnd.BL
{
    public class ApartmentService
    {

        public static bool editApartment(AbstractApartment apartment){
            DBserviceApartment _dbService=new DBserviceApartment();
            return _dbService.EditApartment(apartment);

        }
        public static AbstractApartment? GetApartmentById(int apartmentId){
        DBserviceApartment _dbService=new DBserviceApartment();
        if (apartmentId <= 0)
                throw new ArgumentException("Invalid Apartment ID.");
        return _dbService.GetApartmentById(apartmentId);
        }
      public static string ToggleApartmentActiveStatus(int apartmentId)
        {
            DBserviceApartment _dbService=new DBserviceApartment();
            if (apartmentId <= 0)
                throw new ArgumentException("Invalid Apartment ID.");

            bool success = _dbService.ToggleApartmentActiveStatus(apartmentId);
            return success ? "Apartment status updated successfully." : "Apartment not found or no changes made.";
        }
public static List<Dictionary<string, object>> GetAllActiveApartments(int userId){
    DBserviceApartment _dbService=new DBserviceApartment();
    return _dbService.GetAllActiveApartments(userId);
}
    public static List<dynamic> GetAllApartments(){
        List<dynamic> allApartments = new List<dynamic>();
         DBserviceApartment _dbService=new DBserviceApartment();
         List<RentalApartment> allRental=_dbService.GetAllRentalApartments();
         List<SharedApartment> allShared=_dbService.GetAllSharedApartments();
         List<SubletApartment> allSublet=_dbService.GetAllSubletApartments();
if(allRental!= null){
         allApartments.AddRange(allRental);
}
if(allShared!= null){
         allApartments.AddRange(allShared);
}
if(allSublet!= null){
         allApartments.AddRange(allSublet);
}
         return allApartments;

    }

        internal static bool EditApartment(AbstractApartment updatedApartment)
        {
            throw new NotImplementedException();
        }
    }

}