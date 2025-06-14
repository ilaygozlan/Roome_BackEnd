using System;
using System.Collections.Generic;
using System.Linq;

namespace Roome_BackEnd.BL
{
    public class RoommateMatchingService
    {
        // ממיר את ה-preferences לווקטור מספרי
        public static RoommatePreferenceVector ConvertToVector(RoommatePreferences pref)
        {
            return new RoommatePreferenceVector
            {
                PreferredGender = pref.PreferredGender == "M" ? 1 :
                                  pref.PreferredGender == "F" ? 2 : 0,
                PreferredMinAge = pref.PreferredMinAge,
                PreferredMaxAge = pref.PreferredMaxAge,
                AllowSmoking = pref.AllowSmoking ? 1 : 0,
                AllowPets = pref.AllowPets ? 1 : 0,
                CleanlinessLevel = pref.CleanlinessLevel == "low" ? 1 :
                                    pref.CleanlinessLevel == "medium" ? 2 :
                                    pref.CleanlinessLevel == "high" ? 3 : 0,
                SleepSchedule = pref.SleepSchedule == "early" ? 1 :
                                pref.SleepSchedule == "late" ? 2 : 0,
                SocialLevel = pref.SocialLevel == "low" ? 1 :
                              pref.SocialLevel == "medium" ? 2 :
                              pref.SocialLevel == "high" ? 3 : 0,
                WorkHours = pref.WorkHours == "9H" ? 1 :
                            pref.WorkHours == "12H" ? 2 :
                            pref.WorkHours == "24H" ? 3 : 0,
                WorkFromHome = pref.WorkFromHome ? 1 : 0,
                RelationshipStatus = pref.RelationshipStatus == "single" ? 1 :
                                      pref.RelationshipStatus == "in relationship" ? 2 :
                                      pref.RelationshipStatus == "married" ? 3 : 0,
                SocialStyle = pref.SocialStyle == "love to host" ? 1 :
                              pref.SocialStyle == "love to be at home" ? 2 :
                              pref.SocialStyle == "flexible" ? 3 : 0,
                OpenToFriendship = pref.OpenToFriendship ? 1 : 0
            };
        }

        // מחשב דמיון (מרחק) בין שני משתמשים
        public static double CalculateSimilarity(RoommatePreferenceVector v1, RoommatePreferenceVector v2)
        {
            double wSmoking = 3;
            double wPets = 2;
            double wCleanliness = 4;
            double wSleep = 2;
            double wSocial = 1;
            double wWorkHours = 1;
            double wRelationship = 1;
            double wFriendship = 1;
            double wWorkFromHome = 1;
            double wAge = 0.5;
            double wGender = 1;
            double wSocialStyle = 1;

            double sum = 0;

            sum += wGender * Math.Pow(v1.PreferredGender - v2.PreferredGender, 2);
            sum += wAge * Math.Pow(v1.PreferredMinAge - v2.PreferredMinAge, 2);
            sum += wAge * Math.Pow(v1.PreferredMaxAge - v2.PreferredMaxAge, 2);
            sum += wSmoking * Math.Pow(v1.AllowSmoking - v2.AllowSmoking, 2);
            sum += wPets * Math.Pow(v1.AllowPets - v2.AllowPets, 2);
            sum += wCleanliness * Math.Pow(v1.CleanlinessLevel - v2.CleanlinessLevel, 2);
            sum += wSleep * Math.Pow(v1.SleepSchedule - v2.SleepSchedule, 2);
            sum += wSocial * Math.Pow(v1.SocialLevel - v2.SocialLevel, 2);
            sum += wWorkHours * Math.Pow(v1.WorkHours - v2.WorkHours, 2);
            sum += wWorkFromHome * Math.Pow(v1.WorkFromHome - v2.WorkFromHome, 2);
            sum += wRelationship * Math.Pow(v1.RelationshipStatus - v2.RelationshipStatus, 2);
            sum += wSocialStyle * Math.Pow(v1.SocialStyle - v2.SocialStyle, 2);
            sum += wFriendship * Math.Pow(v1.OpenToFriendship - v2.OpenToFriendship, 2);

            return Math.Sqrt(sum);
        }

        // הפונקציה המרכזית: מקבלת K ומחזירה את ה-K הכי מתאימים
        public List<RoommatePreferences> GetBestMatches(RoommatePreferences userPref, List<RoommatePreferences> allUsers, int k)
        {
            var userVector = ConvertToVector(userPref);

            var neighbors = allUsers
                .Where(other => other.UserId != userPref.UserId)
                .Select(other => new 
                { 
                    Preference = other, 
                    Distance = CalculateSimilarity(userVector, ConvertToVector(other)) 
                })
                .OrderBy(x => x.Distance)
                .Take(k)
                .Select(x => x.Preference)
                .ToList();

            return neighbors;
        }
    }

    public class RoommatePreferenceVector
    {
        public int PreferredGender;
        public int PreferredMinAge;
        public int PreferredMaxAge;
        public int AllowSmoking;
        public int AllowPets;
        public int CleanlinessLevel;
        public int SleepSchedule;
        public int SocialLevel;
        public int WorkHours;
        public int WorkFromHome;
        public int RelationshipStatus;
        public int SocialStyle;
        public int OpenToFriendship;
    }
}
