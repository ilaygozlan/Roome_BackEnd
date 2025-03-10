using Microsoft.OpenApi.Any;
using Microsoft.VisualBasic;
using Roome_BackEnd.DAL;


namespace Roome_BackEnd.BL
{
    public class RentalApartment: AbstractApartment{
        private int contractLength;
        private bool extensionPossible;
          public RentalApartment(int id, int price, int amountOfRooms, string location, bool allowPet, bool allowSmoking,
            bool gardenBalcony, int parkingSpace, DateTime entryDate, DateTime exitDate, bool isActive, int propertyTypeID,
            int userID, int floor, string description, int contractLength, bool extensionPossible)
            : base(id, price, amountOfRooms, location, allowPet, allowSmoking, gardenBalcony, parkingSpace,
                   entryDate, exitDate, isActive, propertyTypeID, userID, floor, description)
        {
            ContractLength=contractLength;
            ExtensionPossible=extensionPossible;}
        public int ContractLength { get => contractLength; set => contractLength = value; }
        public bool ExtensionPossible { get => extensionPossible; set => extensionPossible = value; }
        public int NumberOfRommates { get; internal set; }
        public List<RentalApartment> GetApartment(){
          DBserviceApartment dbService = new DBserviceApartment();
          return dbService.GetAllRentalApartments();
          
}
        public RentalApartment() : base(0, 0, 0, "", false, false, false, 0, DateTime.Now, DateTime.Now, true, 0, 0, 0, "")
        {
        ContractLength = 0;
        ExtensionPossible = false;
      }

        public override int AddApartment()
         {
      DBserviceApartment dbService = new DBserviceApartment();
    return dbService.AddNewRentalApartment(this);
}
        public override bool deleteApartment(){
          if(this.Id<=0){
            throw new ArgumentException("Invalid apartment Id to delete");
          }
          DBserviceApartment dbservice = new DBserviceApartment();
          return dbservice.SoftDeleteRentalApartment(this.Id);
        }

        public static implicit operator RentalApartment(SharedApartment v)
        {
            throw new NotImplementedException();
        }
    }
    }

