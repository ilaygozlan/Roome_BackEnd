using Roome_BackEnd.DAL;
using System;
using System.Collections.Generic;

namespace Roome_BackEnd.BL
{
    public class ApartmentLabel
    {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public string Label { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Add label to database
        public static void Add(int apartmentId, string label)
        {
            DBserviceApartmentLabel db = new DBserviceApartmentLabel();
            db.AddLabel(apartmentId, label);
        }

        // Update existing label by ID
        public static void Update(int id, string newLabel)
        {
            DBserviceApartmentLabel db = new DBserviceApartmentLabel();
            db.UpdateLabel(id, newLabel);
        }

        // Delete label by ID
        public static void Delete(int id)
        {
            DBserviceApartmentLabel db = new DBserviceApartmentLabel();
            db.DeleteLabel(id);
        }

        // Get all labels by apartment ID
        public static List<ApartmentLabel> GetAll(int apartmentId)
        {
            DBserviceApartmentLabel db = new DBserviceApartmentLabel();
            return db.GetLabels(apartmentId);
        }
    }
}
