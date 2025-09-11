using Microsoft.AspNetCore.Mvc;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;
using Newtonsoft.Json;
using System;

namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoommatePreferencesController : ControllerBase
    {
        private readonly DBserviceRoommatePreferences dbService = new DBserviceRoommatePreferences();

        // POST: api/RoommatePreferences/Add
        [HttpPost("Add")]
        public ActionResult<int> AddRoommatePreferences([FromBody] RoommatePreferences preferences)
        {
            try
            {
                int result = dbService.InsertRoommatePreferences(preferences);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding roommate preferences: {ex.Message}");
            }
        }

        // PUT: api/RoommatePreferences/Update
        [HttpPut("Update")]
        public ActionResult<int> UpdateRoommatePreferences([FromBody] RoommatePreferences preferences)
        {
            try
            {
                int result = dbService.UpdateRoommatePreferences(preferences);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating roommate preferences: {ex.Message}");
            }
        }

        // DELETE: api/RoommatePreferences/Delete/{userId}
        [HttpDelete("Delete/{userId}")]
        public ActionResult<int> DeleteRoommatePreferences(int userId)
        {
            try
            {
                int result = dbService.DeleteRoommatePreferences(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting roommate preferences: {ex.Message}");
            }
        }


        // GET: api/RoommatePreferences/GetByUserId/{userId}
        [HttpGet("GetByUserId/{userId}")]
        public ActionResult<RoommatePreferences> GetRoommatePreferencesByUserId(int userId)
        {
            try
            {
                var preferences = dbService.GetRoommatePreferencesByUserId(userId);
                if (preferences == null)
                    return NotFound("Preferences not found for this user.");
                return Ok(preferences);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving preferences: {ex.Message}");
            }
        }

        // GET: api/RoommatePreferences/GetAll
        [HttpGet("GetAll")]
        public ActionResult<List<RoommatePreferences>> GetAllRoommatePreferences()
        {
            try
            {
                var preferencesList = dbService.GetAllRoommatePreferences();
                return Ok(preferencesList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving all preferences: {ex.Message}");
            }
        }
        [HttpPost("Upsert")]
        public IActionResult UpsertRoommatePreferences([FromBody] RoommatePreferences preferences)
        {
            try
            {
                DBserviceRoommatePreferences db = new DBserviceRoommatePreferences();


                var existingPref = db.GetRoommatePreferencesByUserId(preferences.UserId);

                if (existingPref != null)
                {
                    preferences.PreferenceId = existingPref.PreferenceId;
                    db.UpdateRoommatePreferences(preferences);
                }
                else
                {
                    db.InsertRoommatePreferences(preferences);
                }

                return Ok(new { message = "Roommate preferences saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetMatches/{userId}/{k}")]
        public IActionResult GetMatches(int userId, int k)
        {
            try
            {
                var db = new DBserviceRoommatePreferences();
                var userPref = db.GetRoommatePreferencesByUserId(userId);
                var allPrefs = db.GetAllRoommatePreferences();

                var matcher = new RoommateMatchingService();
                var matches = matcher.GetBestMatches(userPref, allPrefs, k);

                var userIds = matches.Select(p => p.UserId).ToList();
                return Ok(userIds);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
