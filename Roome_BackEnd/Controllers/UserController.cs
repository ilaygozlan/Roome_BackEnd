using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Dynamic;
using Roome_BackEnd.BL;
using Roome_BackEnd.DAL;


namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
// GET user by ID
        [HttpGet("GetUserById/{id}")]
        public ActionResult<User> GETUserById(int id)
        {
            var user = BL.User.GetUser(id);
            if(id < 0) 
                return NotFound("No users found.");
            else 
                return Ok(user);  
        }

        // POST add new user to DB
[HttpPost("AddNewUser")]
public ActionResult<object> PostAddNewUser([FromBody] User newUser)
{
    var (userId, isNew) = BL.User.AddUser(newUser);

    var resJson = new
    {
        userId = userId,
        isNewUser = isNew
    };

    return Ok(resJson);
}


       
    // GET: api/User/CheckIfExists?email=example@email.com
    [HttpGet("CheckIfExists")]
    public ActionResult<object> CheckIfUserExists([FromQuery] string email)
    {
        try
        {
            int userId = BL.User.CheckIfUserExists(email);

            return Ok(new
            {
                userId = userId,
                exists = userId != -1
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Server error: " + ex.Message);
        }
    }


        // GET all users
        [HttpGet("GetAllUsers")]
        public ActionResult<List<User>> GETAllUsers()
        {
            var users = BL.User.GetAllUser();

            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }
        //Update User activity
        [HttpPut("DeactivateUser/{userEmail}")]
        public ActionResult<int> PUTDeactivateUser(string userEmail)
        {
            string decodedEmail = Uri.UnescapeDataString(userEmail);
            Console.WriteLine($"Received userEmail: '{decodedEmail}'");

            int result = BL.User.DeactivateUser(decodedEmail);

            if (result < 0)
            {
                Console.WriteLine("Unexpected error occurred.");
                return StatusCode(500, "Unexpected error occurred.");
            }

            if (result == 0)
            {
                Console.WriteLine("User not found or already deactivated.");
                return NotFound("User not found or already deactivated.");
            }

            Console.WriteLine($"User deactivated successfully. Rows affected: {result}");
            return Ok(result);
        }


            //Update User Details
        [HttpPut("UpdateUserDetails")]
            public ActionResult<int> PUTUserDetails([FromBody] User user)
            {
                if (user.ID <= 0)
                {
                    return BadRequest("User ID is required.");
                }

                int result = BL.User.UpdateUserDetailsById(user);

                if (result == 0)
                {
                    return NotFound("No changes made or user not found.");
                }

                Console.WriteLine($"User details updated successfully. Rows affected: {result}");
                return Ok(result);
            }


        //Add friend
        [HttpPost("AddFriend")]
        public ActionResult<string> PostAddFriend([FromBody] FriendRequest request)
        {
            if (request.UserID1 <= 0 || request.UserID2 <= 0)
            {
                return BadRequest("Invalid user IDs.");
            }

            string result = BL.User.AddFriend(request.UserID1, request.UserID2);

            if (result.Contains("already friends"))
            {
                return Conflict(result);
            }

            return Ok(result);
        }
        //class FriendRequest
        public class FriendRequest
        {
            public int UserID1 { get; set; }
            public int UserID2 { get; set; }
        }
        //Get User Friends
        [HttpGet("GetUserFriends/{userId}")]
        public ActionResult<List<User>> GetUserFriends(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            List<User> friends = BL.User.GetUserFriends(userId);

            if (friends == null || friends.Count == 0)
            {
                return NotFound("No friends found for this user.");
            }

            return Ok(friends);
        }
        //Remove Friend
        [HttpDelete("RemoveFriend/{userId1}/{userId2}")]
        public ActionResult<string> RemoveFriend(int userId1, int userId2)
        {
            if (userId1 <= 0 || userId2 <= 0)
            {
                return BadRequest("Invalid user IDs.");
            }

            string result = BL.User.RemoveFriend(userId1, userId2);

            if (result == "These users are not friends")
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        // Like Apartment
        [HttpPost("LikeApartment/{userId}/{apartmentId}")]
        public ActionResult<string> LikeApartment(int userId, int apartmentId)
        {
            if (userId <= 0 || apartmentId <= 0)
            {
                return BadRequest("Invalid User ID or Apartment ID.");
            }

            try
            {
                string result = BL.User.LikeApartment(userId, apartmentId); // Assuming this runs your stored procedure
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        // Remove Like from Apartment
        [HttpDelete("RemoveLikeApartment/{userId}/{apartmentId}")]
        public ActionResult<string> RemoveLikeApartment(int userId, int apartmentId)
        {
            if (userId <= 0 || apartmentId <= 0)
            {
                return BadRequest("Invalid User ID or Apartment ID.");
            }

            try
            {
                string result = BL.User.RemoveLikeApartment(userId, apartmentId); // Assuming it deletes from DB
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //Get User Liked Apartments
        [HttpGet("GetUserLikedApartments/{userId}")]
        public ActionResult<List<dynamic>> GetUserLikedApartments(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            User user = new User();
            List<dynamic> likedApartments = user.GetUserLikedApartments(userId);

            if (likedApartments == null || likedApartments.Count == 0)
            {
                return NotFound("No liked apartments found for this user.");
            }

            return Ok(likedApartments);
        }

        // GET: api/User/GetUserOwnedApartments/{userId}
        [HttpGet("GetUserOwnedApartments/{userId}")]
        public ActionResult<List<dynamic>> GetUserOwnedApartments(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            try
            {
                User user = new User();
                var apartments = user.GetUserOwnedApartments(userId);

                if (apartments == null || apartments.Count == 0)
                {
                    return NotFound("No owned apartments found for this user.");
                }

                return Ok(apartments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Failed to retrieve owned apartments.");
            }
        }

        // GET: api/User/GetPushToken/5
        [HttpGet("GetPushToken/{userId}")]
        public IActionResult GetPushToken(int userId)
        {
            try
            {
                DBserviceUser db = new DBserviceUser();
                string token = db.GetToken(userId);
                if (string.IsNullOrEmpty(token))
                    return NotFound("Push token not found for this user.");

                return Ok(new { pushToken = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/User/PostPushToken/5
[HttpPost("PostPushToken")]
public IActionResult PostPushToken([FromBody] User user)
{
    try
    {
        DBserviceUser db = new DBserviceUser();
        int rowsAffected = db.PostToken(user.ID, user.Token);
        if (rowsAffected > 0)
            return Ok("Push token inserted successfully.");
        else
            return BadRequest("Failed to insert push token.");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}



        // PUT: api/User/UpdatePushToken/5
 [HttpPut("UpdatePushToken/{userId}")]
public IActionResult UpdatePushToken(int userId, [FromBody] User user)
{
    try
    {
        if (string.IsNullOrWhiteSpace(user.Token))
            return BadRequest("Token is required.");

        DBserviceUser db = new DBserviceUser();
        string currentToken = db.GetToken(userId);

        int rowsAffected = string.IsNullOrEmpty(currentToken)
            ? db.PostToken(userId, user.Token)
            : db.UpdateToken(userId, user.Token);

        return rowsAffected > 0 ? Ok("Push token updated successfully.")
                                : BadRequest("Failed to update push token.");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

}}
