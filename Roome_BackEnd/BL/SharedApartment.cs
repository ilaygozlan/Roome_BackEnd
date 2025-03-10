using Microsoft.OpenApi.Any;
using Microsoft.VisualBasic;
using Roome_BackEnd.DAL;


namespace Roome_BackEnd.BL
{
    public class SharedApartment: AbstractApartment{
        private int numberOfRommates;
        public SharedApartment(int id, int price, int amountOfRooms, string location, bool allowPet, bool allowSmoking,
            bool gardenBalcony, int parkingSpace, DateTime entryDate, DateTime exitDate, bool isActive, int propertyTypeID,
            int userID, int floor, string description, int numberOfRommates)
            : base(id, price, amountOfRooms, location, allowPet, allowSmoking, gardenBalcony, parkingSpace,
                   entryDate, exitDate, isActive, propertyTypeID, userID, floor, description)
        {
            NumberOfRommates=numberOfRommates;
            }
    public int NumberOfRommates { get => numberOfRommates; set => numberOfRommates = value; }
    public override int AddApartment()
         {
      DBserviceApartment dbService = new DBserviceApartment();
    return dbService.AddNewSharedApartment(this);
}
        public override bool deleteApartment(){
            return true;
        }
    
    }}