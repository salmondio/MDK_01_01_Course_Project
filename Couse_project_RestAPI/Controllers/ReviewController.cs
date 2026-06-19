using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
using Couse_project_RestAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Composition;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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


        /// <summary>
        /// Позволяет администратору получить список жалоб
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin/List")]
        [ProducesResponseType(typeof(IEnumerable<Review>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult<IEnumerable<Review>>> List()
        {
            try
            {
                // Получаем список жалоб, и отправляем, если нужны все
                var reviewList = await _context.Reviews.ToListAsync();

                return Ok(reviewList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Позволяет администратору получить конкретную жалобу
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Admin/{id}")]
        [ProducesResponseType(typeof(Review), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult<Review>> GetReviewForAdmin(int id)
        {
            try
            {
                // Ищем жалобу по Id
                Review review = await _context.Reviews.FindAsync(id);

                // Если такой нет
                if (review == null)
                    return NotFound($"Не существует жалобы с Id = {id}");

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Отправляет студенту список жалоб, отправленных им
        /// </summary>
        /// <returns></returns>
        [HttpGet("List")]
        [ProducesResponseType(typeof(IEnumerable<ReviewDTO>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> ListForStudent()
        {
            try
            {
                // Ищем жалобы, отправленные студентом с таким-то Id
                IEnumerable<ReviewDTO> reviewList = await _context.Reviews
                    .Where(r => r.Id_student == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                    .Include(r => r.Status)
                    .Include(r => r.Teacher)
                    .Select(r => new ReviewDTO
                    {
                        Id = r.Id,
                        Date_time = r.Date_time,
                        Text = r.Text,
                        Is_active = r.Is_active,
                        Teacher = new UserDTO
                        {
                            Id = r.Teacher.Id,
                            Name = r.Teacher.Name,
                            Lastname = r.Teacher.Lastname,
                            Surname = r.Teacher.Surname,
                        },
                        Status = r.Status
                    })
                    .ToArrayAsync();
                // Если таких жалоб нет
                if (!reviewList.Any())
                    return NotFound($"Не существует жалоб, отпарвленных студентом с Id = {User.FindFirst(ClaimTypes.NameIdentifier).Value}");

                return Ok(reviewList);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Отправляет студенту список жалоб, отправленных им
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReviewDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ReviewDTO>> GetReviewForStudent(int id)
        {
            try
            {
                // Ищем жалобу, с запрашиваемым Id и с Id студента, сделавшего запрос
                ReviewDTO review = await _context.Reviews
                    .Where(r => r.Id == id && r.Id_student == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                    .Include(r => r.Status)
                    .Include(r => r.Teacher)
                    .Select(r => new ReviewDTO
                    {
                        Id = r.Id,
                        Date_time = r.Date_time,
                        Text = r.Text,
                        Is_active = r.Is_active,
                        Teacher = new UserDTO
                        {
                            Id = r.Teacher.Id,
                            Name = r.Teacher.Name,
                            Lastname = r.Teacher.Lastname,
                            Surname = r.Teacher.Surname,
                        },
                        Status = r.Status
                    })
                    .FirstAsync();
                // Если такой жалобы нет
                if (review == null)
                    return NotFound($"Не существует жалобы с Id = {id} и отправителем с Id = {User.FindFirst(ClaimTypes.NameIdentifier).Value}");

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Метод позволяет студенту отправить жалобу на преподавателя
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        [ProducesResponseType(typeof(ReviewCreateDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ReviewCreateDTO>> Add([FromBody] ReviewCreateDTO review)
        {
            try
            {
                // Проверка на существование получателя
                User? teacher = await _context.Users.FindAsync(review.Id_teacher);
                // Если такого студента или препода нет
                if (teacher == null ||
                    teacher.Id_role != 5)
                    return BadRequest($"Преподавателя с указанными Id не существует.");

                // Создаем объект полноценной модели жалобы
                Review newReview = new Review()
                {
                    Id_student = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                    Id_teacher = review.Id_teacher,
                    Date_time = DateTime.Now,
                    Text = review.Text
                };

                // Добавляем и сохраняем жалобу в БД
                await _context.Reviews.AddAsync(newReview);
                await _context.SaveChangesAsync();

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Метод позволяет студенту отправить жалобу на преподавателя
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPost("Owner/Add")]
        [ProducesResponseType(typeof(Review), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Review>> OwnerAdd([FromBody] Review review)
        {
            try
            {
                // Проверка на существование студента и препода
                User? student = await _context.Users.FindAsync(review.Id_student);
                User? teacher = await _context.Users.FindAsync(review.Id_teacher);
                // Если такого студента или препода нет
                if (student == null || teacher == null ||
                    student.Id_role != 4 ||
                    teacher.Id_role != 5)
                    return BadRequest($"Студента или преподавателя с указанными Id не существует.");

                // Создаем объект полноценной модели жалобы
                if (await _context.Reviews.AnyAsync(r => r.Id == review.Id))
                    return BadRequest($"Отзыв с Id = {review.Id} уже существует");

                // Добавляем и сохраняем жалобу в БД
                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Метод позволяет обновить статус жалобы администратору
        /// </summary>
        /// <param name="status"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("UpdateStatus/{id}")]
        [ProducesResponseType(typeof(Review), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<Review>> UpdateStatus([FromBody] MessageStatus status, int id)
        {
            try
            {
                // Ищем жалобу по Id
                Review updatedReview = await _context.Reviews.FindAsync(id);

                // Если такой жалобы нет
                if (updatedReview == null)
                    return NotFound($"Жалобы с id = {id} не существует");

                // Обновляем статус и сохраняем изменения
                updatedReview.Id_status = status.Id;
                await _context.SaveChangesAsync();

                return Ok(updatedReview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Позволяет студенту отозвать или возобновить жалобу
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("ChangeActive/{id}")]
        [ProducesResponseType(typeof(ReviewDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ReviewDTO>> ChangeActive(int id)
        {
            try
            {
                // Ищем жалобу по Id
                Review review = await _context.Reviews.FindAsync(id);

                // Если таковой нет
                if (review == null)
                    return NotFound($"Жалобы с id = {id} не существует");

                // Если студент пытается отозвать/возобновить чужую жалобу
                if (review.Id_student != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                    return Forbid("Вы не можете отзывать/возобновить жалобы, отправленные не вами");

                // Изменяем жалобу и сохраняем в БД
                review.Is_active = !review.Is_active;
                await _context.SaveChangesAsync();

                return Ok(new ReviewDTO
                {
                    Id = review.Id,
                    Date_time = review.Date_time,
                    Text = review.Text,
                    Is_active = review.Is_active
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Метод позволяет обновить статус жалобы администратору
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPut("Owner/Update")]
        [ProducesResponseType(typeof(Review), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<Review>> OwnerUpdate([FromBody] Review review)
        {
            try
            {
                // Ищем жалобу по Id
                Review updatedReview = await _context.Reviews.FindAsync(review.Id);

                // Если такой жалобы нет
                if (updatedReview == null)
                    return NotFound($"Жалобы с id = {review.Id} не существует");

                // Проверка на существование студента и препода
                User? student = await _context.Users.FindAsync(review.Id_student);
                User? teacher = await _context.Users.FindAsync(review.Id_teacher);
                // Если такого студента или препода нет
                if (student == null || teacher == null ||
                    student.Id_role != 4 ||
                    teacher.Id_role != 5)
                    return BadRequest($"Студента или преподавателя с указанными Id не существует.");

                // Обновляем статус и сохраняем изменения
                updatedReview.Id_student = review.Id_status;
                updatedReview.Id_teacher = review.Id_teacher;
                updatedReview.Id_status = review.Id_status;
                updatedReview.Id_inspector = review.Id_inspector;
                updatedReview.Date_time = DateTime.Now;
                updatedReview.Text = review.Text;
                updatedReview.Is_active = review.Is_active;
                await _context.SaveChangesAsync();

                return Ok(updatedReview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Метод позволяет обновить статус жалобы администратору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Owner/Delete/{id}")]
        [ProducesResponseType(typeof(Review), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Review>> Delete(int id)
        {
            try
            {
                // Ищем жалобу по Id
                Review deletedReview = await _context.Reviews.FindAsync(id);

                // Если такой жалобы нет
                if (deletedReview == null)
                    return NotFound($"Жалобы с id = {id} не существует");

                // Обновляем статус и сохраняем изменения
                _context.Reviews.Remove(deletedReview);
                await _context.SaveChangesAsync();

                return Ok(deletedReview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
