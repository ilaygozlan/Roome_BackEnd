using Microsoft.OpenApi.Any;
using Microsoft.VisualBasic;
using Roome_BackEnd.DAL;


namespace Roome_BackEnd.BL
{
    public class SharedApartment: AbstractApartment{
        private int numberOfRoommates;
        public SharedApartment() : base(0, 0, 0, "", false, false, false, 0, DateTime.Now, DateTime.Now, true, 0, 0, 0, "", 1)
        {numberOfRoommates = 0;}
        public SharedApartment(
                int id, int price, int amountOfRooms, string location, bool allowPet, bool allowSmoking,
                bool gardenBalcony, int parkingSpace, DateTime entryDate, DateTime exitDate, bool isActive,
                int propertyTypeID, int userID, int floor, string description,int apartmentType,  int numberOfRoommates) 
                : base(id, price, amountOfRooms, location, allowPet, allowSmoking, gardenBalcony, parkingSpace, 
                      entryDate, exitDate, isActive, propertyTypeID, userID, floor, description, apartmentType)
                      {NumberOfRoommates = numberOfRoommates;}

         public int NumberOfRoommates  { get => numberOfRoommates; set => numberOfRoommates = value; }
        public override int AddApartment(){
          DBserviceApartment dbService = new DBserviceApartment();
          return dbService.AddNewSharedApartment(this);}

        public List<SharedApartment> GetSharedApartments(){
          DBserviceApartment dbService = new DBserviceApartment();
          return dbService.GetAllSharedApartments();}

          
    }
    }