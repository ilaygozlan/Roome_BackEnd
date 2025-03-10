using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Dynamic;
using Roome_BackEnd.BL;


namespace Roome_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
      

        // POST add new user to DB
        // POST add new user to DB
        [HttpPost("AddNewUser")]
        public ActionResult<int> PostAddNewUser([FromBody] User newUser)
        {
            int result = newUser.AddUser(newUser);

            if (result == 0)
            {
                return Conflict("Email already exists or user could not be added.");
            }

            return Ok(result);
        }


        // GET user by email
       [HttpGet("GetUserByEmail/{email}")]
    public ActionResult<User> GETUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            string decodedEmail = Uri.UnescapeDataString(email.Trim());
            Console.WriteLine($"🔍 Received request for user: '{decodedEmail}'");

            User user = new User().GetUser(decodedEmail);

            if (user == null)
            {
                Console.WriteLine("❌ User not found.");
                return NotFound("User not found.");
            }

            Console.WriteLine($"✅ User found: {user.FullName}");
            return Ok(user);
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
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            return BadRequest("Email is required.");
        }

        int result = user.UpdateUserDetailsByEmail(user);

        if (result == 0)
        {
            Console.WriteLine("❌ No changes made or user not found.");
            return NotFound("No changes made or user not found.");
        }

        Console.WriteLine($"✅ User details updated successfully. Rows affected: {result}");
        return Ok(result);
    }

}


}
