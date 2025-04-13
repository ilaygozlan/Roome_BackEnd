using System;
using System.Collections.Generic;
using Roome_BackEnd.DAL;

namespace Roome_BackEnd.BL
{
    public class User
    {
        int id;
        string email = "";
        string fullName = "";
        string phoneNumber = "";
        char gender;
        DateTime birthDate;
        string profilePicture = "";
        bool ownPet;
        bool smoke;
        bool isActive;
        string token;

        public User() { }

        public User(int id, string email, string fullName, string phoneNumber, char gender, DateTime birthDate, string profilePicture, bool ownPet, bool smoke, bool isActive, string token)
        {
            ID = id;
            Email = email;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Gender = gender;
            BirthDate = birthDate;
            ProfilePicture = profilePicture;
            OwnPet = ownPet;
            Smoke = smoke;
            IsActive = isActive;
            Token= token;
        }

        public int ID { get => id; set => id = value; }
        public string Email { get => email; set => email = value; }
        public string FullName { get => fullName; set => fullName = value; }
        public string PhoneNumber { get => phoneNumber; set => phoneNumber = value; }
        public char Gender { get => gender; set => gender = value; }
        public DateTime BirthDate { get => birthDate; set => birthDate = value; }
        public string ProfilePicture { get => profilePicture; set => profilePicture = value; }
        public bool OwnPet { get => ownPet; set => ownPet = value; }
        public bool Smoke { get => smoke; set => smoke = value; }
        public bool IsActive { get => isActive; set => isActive = value; }
        public string Token { get => token; set => token = value; }

        public (int userId, bool isNew) AddUser(User NewUser)
        {
            DBserviceUser dBserviecesuser = new DBserviceUser();
            return dBserviecesuser.AddNewUser(NewUser);
        }

        public User GetUser(string useremail)
        {
            DBserviceUser dBserviecesuser = new DBserviceUser();
            return dBserviecesuser.GetUser(useremail);
        }

        public static List<User> GetAllUser()
        {
            DBserviceUser dBserviecesuser = new DBserviceUser();
            return dBserviecesuser.GetAllUser();
        }

        public static int DeactivateUser(string userEmail)
        {
            DBserviceUser dBserviecesuser = new DBserviceUser();
            return dBserviecesuser.DeactivateUser(userEmail);
        }

        public int UpdateUserDetailsByEmail(User user)
        {
            DBserviceUser dBserviecesuser = new DBserviceUser();
            return dBserviecesuser.UpdateUserDetailsByEmail(user);
        }
        public static string AddFriend(int userId1, int userId2)
        {
            DBserviceUser dBserviecesuser = new DBserviceUser();
            return dBserviecesuser.AddFriend(userId1, userId2);
        }

        public static List<User> GetUserFriends(int userId)
            {
                DBserviceUser dBserviecesuser = new DBserviceUser();
                return dBserviecesuser.GetUserFriends(userId);
            }

        public static string RemoveFriend(int userId1, int userId2)
            {
                DBserviceUser dBserviecesuser = new DBserviceUser();
                return dBserviecesuser.RemoveFriend(userId1, userId2);
            }


  

        public static string LikeApartment(int userId, int apartmentId)
        {
            DBserviceUser dBserviceUser = new DBserviceUser();
            return dBserviceUser.UserLikeApartment(userId, apartmentId);
        }

  
   public static string RemoveLikeApartment(int userId, int apartmentId)
   {
       DBserviceUser dBserviceUser = new DBserviceUser();
       return dBserviceUser.RemoveUserLikeApartment(userId, apartmentId);
   }

    public List<dynamic> GetUserLikedApartments(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid user ID.");
            }

            DBserviceUser dbServiceUser = new DBserviceUser();
            return dbServiceUser.GetUserLikedApartments(userId);
        }

        public List<dynamic> GetUserOwnedApartments(int userId)
        {
            DBserviceUser dbService = new DBserviceUser();
            return dbService.GetUserOwnedApartments(userId);
        }

        public static User GetUserById(int userId){
            DBserviceUser db = new DBserviceUser();
            User user = db.GetUserById(userId);
            return user;
        }
        public static int CheckIfUserExists(string email){
          DBserviceUser db = new DBserviceUser();
            return db.CheckIfUserExists(email);
        }
    }

}
