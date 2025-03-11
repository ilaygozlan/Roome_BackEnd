using Microsoft.OpenApi.Any;
using Microsoft.VisualBasic;
using Roome_BackEnd.DAL;


namespace Roome_BackEnd.BL
{
    public abstract class  AbstractApartment{
      
        
        protected AbstractApartment(int id,int price,int amountOfRooms,string location,bool allowPet,bool allowSmoking,bool gardenBalcony, int parkingSpace,DateTime entryDate, DateTime exitDate,
        bool isActive,int propertyTypeID,int userID,int floor,string description=""){
             Id = id;
            Price = price;
            AmountOfRooms = amountOfRooms;
            Location = location;
            AllowPet = allowPet;
            AllowSmoking = allowSmoking;
            GardenBalcony = gardenBalcony;
            ParkingSpace = parkingSpace;
            EntryDate = entryDate;
            ExitDate = exitDate;
            IsActive = isActive;
            PropertyTypeID = propertyTypeID;
            UserID = userID;
            Floor = floor;
            Description = description;
        }
        
        public int Id { get; set; }
        public int Price { get; set; }
        public int AmountOfRooms { get; set; }
        public string Location { get; set; }
        public bool AllowPet { get; set; }
        public bool AllowSmoking { get; set; }
        public bool GardenBalcony { get; set; }
        public int ParkingSpace { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime ExitDate { get; set; }
        public bool IsActive { get; set; }
        public int PropertyTypeID { get; set; }
        public int UserID { get; set; }
        public int Floor { get; set; }
        public string Description { get; set; }
        public abstract int AddApartment();
       
    }
    
}