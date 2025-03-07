using System.Net;
using System;
using Microsoft.VisualBasic;
using Roome_BackEnd.DAL;


namespace Roome_BackEnd.BL
{
    public class User
    {
        string email="";
        string fullName="";
        string phoneNumber="";
        Char gender;
       DateTime birthDate;
        string profilePicture="";
        bool ownPet;
        bool smoke;

        public User(){}
        public User(string email, string fullName, string phoneNumber, char gender, DateTime birthDate, string profilePicture, bool ownPet, bool smoke)
        {
            Email = email;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Gender = gender;
            BirthDate = birthDate;
            ProfilePicture = profilePicture;
            OwnPet = ownPet;
            Smoke = smoke;
        }

        public string Email { get => email; set => email = value; }
        public string FullName { get => fullName; set => fullName = value; }
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        public char Gender { get => gender; set => gender = value; }
        public DateTime BirthDate { get => birthDate; set => birthDate = value; }
        public string ProfilePicture { get => profilePicture; set => profilePicture = value; }
        public bool OwnPet { get => ownPet; set => ownPet = value; }
        public bool Smoke { get => smoke; set => smoke = value; }

        public int AddUser(User NewUser)
        {
           DBserviceUser dBserviecesuser = new DBserviceUser();

            int numEffected = dBserviecesuser.AddNewUser(NewUser);
            return numEffected;
        }
       
    }

}
