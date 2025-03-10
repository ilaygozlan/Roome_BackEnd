using Roome_BackEnd.DAL;

namespace Roome_BackEnd.BL
{
    public class SubletApartment : AbstractApartment
    {
        private bool canCancelWithoutPenalty;
        private bool isWholeProperty;

        public SubletApartment() : base(0, 0, 0, "", false, false, false, 0, DateTime.Now, DateTime.Now, true, 0, 0, 0, "")
        {
            CanCancelWithoutPenalty = false;
            IsWholeProperty = false;
        }

        public SubletApartment(
                int id, int price, int amountOfRooms, string location, bool allowPet, bool allowSmoking,
                bool gardenBalcony, int parkingSpace, DateTime entryDate, DateTime exitDate, bool isActive,
                int propertyTypeID, int userID, int floor, string description, bool canCancelWithoutPenalty, bool isWholeProperty)
                : base(id, price, amountOfRooms, location, allowPet, allowSmoking, gardenBalcony, parkingSpace,
                      entryDate, exitDate, isActive, propertyTypeID, userID, floor, description)
        {
            CanCancelWithoutPenalty = canCancelWithoutPenalty;
            IsWholeProperty = isWholeProperty;
        }

        public bool CanCancelWithoutPenalty { get => canCancelWithoutPenalty; set => canCancelWithoutPenalty = value; }
        public bool IsWholeProperty { get => isWholeProperty; set => isWholeProperty = value; }

        //--------------------------------------------------------------------------------------------------
        // This method adds a new Sublet Apartment
        //--------------------------------------------------------------------------------------------------
        public override int AddApartment()
        {
            DBserviceApartment dbService = new DBserviceApartment();
            return dbService.AddNewSubletApartment(this);
        }

        //--------------------------------------------------------------------------------------------------
        // This method retrieves all Sublet Apartments
        //--------------------------------------------------------------------------------------------------
        public List<SubletApartment> GetSubletApartments()
        {
            DBserviceApartment dbService = new DBserviceApartment();
            return dbService.GetAllSubletApartments();
        }

        public override bool deleteApartment()
        {
            return true; 
        }
    }
}
