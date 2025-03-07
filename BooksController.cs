using Microsoft.AspNetCore.Mvc;
using serverSide.BL;
using System.Data.SqlClient;
using System.Data;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace serverSide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        // GET - get all books
        [HttpGet]
        public IEnumerable<object> Get()
        {
            return Book.showBooks();
        }
        // GET - get top 5 books by rating 
        [HttpGet("showTop5BooksByRating")]
        public IEnumerable<object> GetTop5Courses()
        {
            return Book.showTop5BooksByrating();
        }

        // GET api/<BooksController>/5
        [HttpGet("getBookReviews")]
        public List<object> Get(int bookId)
        {
            return Book.getBookReviews(bookId);
        }

        // POST add new book to DB
        [HttpPost("AddNewBook")]
        public bool Post([FromBody] Book newBook)
        {
            return newBook.AddNewBook(newBook);
        }

        // PUT api/<BooksController>/5
        [HttpPut("changeBookActivity")]
        public bool Put(int bookId)
        {
            return Book.changeBookActivity(bookId);
        }
        // PUT RateBook
        [HttpPut("RateBook")]
        public IActionResult Put(int bookID, int newRating, int userID, string review)
        {
            int status = Book.RateBook(bookID, newRating, userID, review);

            if (status == 1) { return Ok(true); }

            else if (status == 0) { return NotFound(false); }

            return Unauthorized("user session has ended");
        }

        // DELETE api/<BooksController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }


}
