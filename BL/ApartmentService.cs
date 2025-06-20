using Roome_BackEnd.DAL;

namespace Roome_BackEnd.BL
{
    public class ApartmentService
    {
        public static bool editApartment(AbstractApartment apartment)
        {
            DBserviceApartment _dbService = new DBserviceApartment();
            return _dbService.EditApartment(apartment);
        }

        public static AbstractApartment? GetApartmentById(int apartmentId)
        {
            DBserviceApartment _dbService = new();
            DBserviceUploadImage _imageService = new();

            var apt = _dbService.GetApartmentById(apartmentId);

            if (apt != null)
            {
                var labels = _imageService.GetLabelSummaryByApartmentId(apartmentId);
                apt.DetectedLabels = labels
                    .Select(l => l["Label"]?.ToString())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList()!;
            }

            return apt;
        }

        public static string ToggleApartmentActiveStatus(int apartmentId)
        {
            DBserviceApartment _dbService = new DBserviceApartment();
            if (apartmentId <= 0)
                throw new ArgumentException("Invalid Apartment ID.");

            bool success = _dbService.ToggleApartmentActiveStatus(apartmentId);
            return success ? "Apartment status updated successfully." : "Apartment not found or no changes made.";
        }

        public static List<Dictionary<string, object>> GetAllActiveApartments(int userId)
        {
            DBserviceApartment _dbService = new();
            DBserviceUploadImage labelService = new();

            List<Dictionary<string, object>> apartments = _dbService.GetAllActiveApartments(userId);

            foreach (var apartment in apartments)
            {
                if (apartment.TryGetValue("ApartmentID", out object? aptIdObj) && aptIdObj is int aptId)
                {
                    // Step 1: Try to get existing labels from DB
                    List<Dictionary<string, string>> labels = labelService.GetLabelsByApartmentId(aptId);

                    // Step 2: If no labels found, re-run detection
                    if (labels.Count == 0 && apartment.TryGetValue("ImageUrls", out object? urlsObj) && urlsObj is string imageUrls)
                    {
                        List<string> imagePaths = imageUrls
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .ToList();

                        // 🔁 Run detection and save to DB
                        labelService.DetectAndSaveLabels(aptId, imagePaths);

                        // 🔄 Fetch again after detection
                        labels = labelService.GetLabelsByApartmentId(aptId);
                    }

                    // Attach to apartment
                    apartment["ImageLabels"] = labels;
                }
            }

            return apartments;
        }

        public static List<dynamic> GetAllApartments()
        {
            List<dynamic> allApartments = new();
            DBserviceApartment _dbService = new();
            List<RentalApartment> allRental = _dbService.GetAllRentalApartments();
            List<SharedApartment> allShared = _dbService.GetAllSharedApartments();
            List<SubletApartment> allSublet = _dbService.GetAllSubletApartments();

            if (allRental != null)
                allApartments.AddRange(allRental);
            if (allShared != null)
                allApartments.AddRange(allShared);
            if (allSublet != null)
                allApartments.AddRange(allSublet);

            return allApartments;
        }

        internal static bool EditApartment(AbstractApartment updatedApartment)
        {
            throw new NotImplementedException();
        }
    }
}
