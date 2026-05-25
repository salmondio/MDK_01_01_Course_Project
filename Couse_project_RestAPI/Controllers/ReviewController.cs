using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Couse_project_RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly DbContextMain _context;

        public ReviewController(DbContextMain context)
        {
            _context = context;
        }



        [HttpGet("List")]
        [ProducesResponseType(typeof(IEnumerable<Review>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Review>>> List()
        {
            try
            {
                IEnumerable<Review> reviewList = await _context.Reviews.ToArrayAsync();
                return Ok(reviewList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Review), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            try
            {
                Review review = await _context.Reviews.FindAsync(id);

                if (review == null)
                    return NotFound($"Отзыва с id = {id} не существует");

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("Add")]
        [ProducesResponseType(typeof(Review), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Review>> Add([FromBody] Review review)
        {
            try
            {
                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("Update")]
        [ProducesResponseType(typeof(Review), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Review>> Update([FromBody] Review review)
        {
            try
            {
                Review updatedReview = await _context.Reviews.FindAsync(review.Id);

                if (updatedReview == null)
                    return NotFound($"Отзыва с id = {updatedReview.Id} не существует");

                updatedReview.Id_status = review.Id_status;
                updatedReview.Text = review.Text;

                await _context.SaveChangesAsync();

                return Ok(updatedReview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("Delete")]
        [ProducesResponseType(typeof(Review), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Review>> Delete(int id)
        {
            try
            {
                Review review = await _context.Reviews.FindAsync(id);

                if (review == null)
                    return NotFound($"Отзыва с id = {id} не существует");

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
