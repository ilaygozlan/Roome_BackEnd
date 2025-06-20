using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Roome_BackEnd.DAL
{
    public class DBserviceUploadImage
    {
        private readonly IConfigurationRoot configuration;

        // Initialize configuration from appsettings.json
        public DBserviceUploadImage()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        // Establish connection to SQL Server
        public SqlConnection connect()
        {
            string? cStr = configuration.GetConnectionString("myProjDB");
            if (string.IsNullOrEmpty(cStr))
                throw new Exception("Connection string 'myProjDB' not found in appsettings.json");

            SqlConnection con = new(cStr);
            con.Open();
            return con;
        }

        public List<string> GetImagesByApartmentId(int apartmentId)
        {
            List<string> images = new();
            string query = "SELECT ImagePath FROM ApartmentImages WHERE ApartmentId = @ApartmentId";

            using SqlConnection con = connect();
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string? path = reader["ImagePath"].ToString();
                if (!string.IsNullOrWhiteSpace(path))
                    images.Add(path);
            }

            return images;
        }

        public void DeleteLabel(int apartmentId, string imagePath, string label)
        {
            string query = @"DELETE FROM ApartmentImageLabels 
                             WHERE ApartmentId = @ApartmentId 
                               AND ImagePath = @ImagePath 
                               AND PredictedLabel = @Label";

            using SqlConnection con = connect();
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);
            cmd.Parameters.AddWithValue("@ImagePath", imagePath);
            cmd.Parameters.AddWithValue("@Label", label);
            cmd.ExecuteNonQuery();
        }

        // Upload images to the database using stored procedure
        public object UploadImages(string imagesLinks, int apartmentId)
        {
            using SqlConnection con = connect();
            using SqlCommand cmd = CreateCommandWithStoredProcedureUploadImages("UploadApartmentImages", con, imagesLinks, apartmentId);
            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected >= 0
                    ? new { message = "Images uploaded successfully!", urls = imagesLinks }
                    : new { message = "Error" };
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload images", ex);
            }
        }

        // Helper for stored procedure
        private SqlCommand CreateCommandWithStoredProcedureUploadImages(string spName, SqlConnection con, string imagesLinks, int apartmentId)
        {
            SqlCommand cmd = new()
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add(new SqlParameter("@RowsAffected", SqlDbType.Int) { Direction = ParameterDirection.Output });
            cmd.Parameters.AddWithValue("@ApartmentID", apartmentId);
            cmd.Parameters.AddWithValue("@ImageUrls", imagesLinks);

            return cmd;
        }

        // Detect labels using Python script and save to DB
        public void DetectAndSaveLabels(int apartmentId, List<string> imagePaths)
        {
            foreach (var imgPath in imagePaths)
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), imgPath.TrimStart('/').Replace("/", "\\"));

                if (!File.Exists(fullPath)) continue;

                string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "AIModels", "predict.py");
                string outputFileName = $"output_{Guid.NewGuid()}.txt";
                string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "AIModels", outputFileName);

                var psi = new ProcessStartInfo
                {
                    FileName = @"C:\\Users\\ofri9\\tf-env\\Scripts\\python.exe",
                    Arguments = $"\"{scriptPath}\" \"{fullPath}\" \"{outputPath}\"",
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine("Python error: " + error);
                    continue;
                }

                if (!File.Exists(outputPath)) continue;

                string lastMeaningfulLine = File.ReadAllText(outputPath, Encoding.UTF8).Trim();
                try { File.Delete(outputPath); } catch { }

                if (string.IsNullOrEmpty(lastMeaningfulLine)) continue;

                string[] labels = lastMeaningfulLine.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var label in labels)
                {
                    InsertLabel(apartmentId, imgPath, label.Trim());
                }
            }
        }

        // NEW: Detect labels temporarily (without saving to DB)
public List<string> DetectLabelsWithoutSaving(List<string> imagePaths)
{
    List<string> detectedLabels = new();

    foreach (var fullPath in imagePaths)
    {
        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"File does not exist: {fullPath}");
            continue;
        }

        string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "AIModels", "predict.py");
        string outputFileName = $"output_{Guid.NewGuid()}.txt";
        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "AIModels", outputFileName);

        Console.WriteLine($"Running prediction:");
        Console.WriteLine($"Script Path: {scriptPath}");
        Console.WriteLine($"Full Path: {fullPath}");
        Console.WriteLine($"Output Path: {outputPath}");

        var psi = new ProcessStartInfo
        {
            FileName = @"C:\Users\ofri9\tf-env\Scripts\python.exe",
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        // שימוש ב-ArgumentList - הכי בטוח למניעת בעיות רווחים בנתיב
        psi.ArgumentList.Add(scriptPath);
        psi.ArgumentList.Add(fullPath);
        psi.ArgumentList.Add(outputPath);

        using var process = Process.Start(psi);
        string stdOutput = process.StandardOutput.ReadToEnd();
        string stdError = process.StandardError.ReadToEnd();
        process.WaitForExit();

        Console.WriteLine($"Python stdout: {stdOutput}");
        Console.WriteLine($"Python stderr: {stdError}");

        if (!string.IsNullOrWhiteSpace(stdError))
        {
            Console.WriteLine("Temporary detection error: " + stdError);
            continue;
        }

        if (!File.Exists(outputPath))
        {
            Console.WriteLine("Output file not found.");
            continue;
        }

        string content = File.ReadAllText(outputPath, Encoding.UTF8).Trim();
        try { File.Delete(outputPath); } catch { }

        if (!string.IsNullOrEmpty(content))
        {
            detectedLabels.AddRange(content.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()));
        }
        else
        {
            Console.WriteLine("No labels detected.");
        }
    }

    return detectedLabels;
}
        // Insert label if not already present
        public void InsertLabel(int apartmentId, string imagePath, string predictedLabel)
        {
            if (string.IsNullOrWhiteSpace(predictedLabel)) return;
            if (LabelAlreadyExists(apartmentId, imagePath, predictedLabel)) return;

            if (predictedLabel.Length > 255)
                predictedLabel = predictedLabel[..255];

            string query = "INSERT INTO ApartmentImageLabels (ApartmentId, ImagePath, PredictedLabel) " +
                           "VALUES (@ApartmentId, @ImagePath, @PredictedLabel)";

            using SqlConnection con = connect();
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);
            cmd.Parameters.AddWithValue("@ImagePath", imagePath);
            cmd.Parameters.AddWithValue("@PredictedLabel", predictedLabel);
            cmd.ExecuteNonQuery();
        }

        private bool LabelAlreadyExists(int apartmentId, string imagePath, string label)
        {
            string query = @"SELECT COUNT(*) FROM ApartmentImageLabels 
                             WHERE ApartmentId = @ApartmentId 
                             AND LOWER(ImagePath) = LOWER(@ImagePath) 
                             AND PredictedLabel = @Label";

            using SqlConnection con = connect();
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);
            cmd.Parameters.AddWithValue("@ImagePath", imagePath.Trim().ToLower());
            cmd.Parameters.AddWithValue("@Label", label.Trim());

            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public List<Dictionary<string, string>> GetLabelsByApartmentId(int apartmentId)
        {
            var results = new List<Dictionary<string, string>>();
            string query = "SELECT ImagePath, PredictedLabel FROM ApartmentImageLabels WHERE ApartmentId = @ApartmentId";

            using SqlConnection con = connect();
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                results.Add(new Dictionary<string, string>
                {
                    { "ImagePath", reader["ImagePath"].ToString() ?? "" },
                    { "PredictedLabel", reader["PredictedLabel"].ToString() ?? "" }
                });
            }

            return results;
        }

        public List<Dictionary<string, object>> GetLabelSummaryByApartmentId(int apartmentId)
        {
            var summary = new List<Dictionary<string, object>>();
            string query = @"
                SELECT PredictedLabel, COUNT(*) AS Count
                FROM ApartmentImageLabels
                WHERE ApartmentId = @ApartmentId
                GROUP BY PredictedLabel
                ORDER BY Count DESC";

            using SqlConnection con = connect();
            using SqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                summary.Add(new Dictionary<string, object>
                {
                    { "Label", reader["PredictedLabel"].ToString() },
                    { "Count", Convert.ToInt32(reader["Count"]) }
                });
            }

            return summary;
        }
    }
}
