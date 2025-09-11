namespace Roome_BackEnd.BL
{
    public class RoommatePreferences
    {
        public int PreferenceId { get; set; }
        public int UserId { get; set; }
        public string PreferredGender { get; set; }
        public int PreferredMinAge { get; set; }
        public int PreferredMaxAge { get; set; }
        public bool AllowSmoking { get; set; }
        public bool AllowPets { get; set; }
        public string CleanlinessLevel { get; set; }
        public string SleepSchedule { get; set; }
        public string SocialLevel { get; set; }
        public string WorkHours { get; set; }
        public bool WorkFromHome { get; set; }
        public bool HasPet { get; set; }
        public string PetType { get; set; }
        public string RelationshipStatus { get; set; }
        public string SocialStyle { get; set; }
        public bool OpenToFriendship { get; set; }
        public string Notes { get; set; }
    }
}
