using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.BL
{
    public class Roomate
    {
        public int? UserID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Job { get; set; }
        public string Description { get; set; }
        public string BirthDate { get; set; }

       [JsonConstructor]
       public Roomate(int? userId, string name, string gender, string job, string description, string birthDate)
        {
            UserID = userId;
            Name = name;
            Gender = gender;
            Job = job;
            Description = description;
            BirthDate = birthDate;
        }
       /* public List<Roomate> GetRoomatesInApartment(){

        } */   

        public static bool AddRoommates(string roomates, int apartmentId){
          DBserviceRoomates dBservice = new DBserviceRoomates();
          return dBservice.AddNewRoommates(roomates, apartmentId);
        }
        public static bool DeleteRoommate(string roomateName, int apartmentId){
          DBserviceRoomates dBservice = new DBserviceRoomates();
          return dBservice.DeleteRoommate(roomateName, apartmentId);
        }
    }
}