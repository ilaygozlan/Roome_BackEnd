using System;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.BL
{
    public class OpenHouse
    {
        public int OpenHouseId { get; set; }
        public int ApartmentId { get; set; }
        public DateTime Date { get; set; }
        public int AmountOfPeoples { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        // Constructor
        public OpenHouse( int openHouseId, int apartmentId, DateTime date, int amountOfPeoples, string startTime, string endTime )
        {
            OpenHouseId = openHouseId;
            ApartmentId = apartmentId;
            Date = date;
            AmountOfPeoples = amountOfPeoples;
            StartTime = startTime;
            EndTime = endTime;
        }

        public static int CreateAnOpenHouse(OpenHouse openHouse, int userId){
            DBservicesOpenHouse dbService = new DBservicesOpenHouse();
            return dbService.CreateAnOpenHouse(openHouse, userId);
        }
    }
}
