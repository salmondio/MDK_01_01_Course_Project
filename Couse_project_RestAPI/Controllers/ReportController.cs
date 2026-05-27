using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
using Couse_project_RestAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Couse_project_RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly DbContextMain _context;

        public ReportController(DbContextMain context)
        {
            _context = context;
        }


        /// <summary>
        /// Позволяет администратору получить список жалоб
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin/List")]
        [ProducesResponseType(typeof(IEnumerable<Report>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Report>>> List()
        {
            try
            {
                // Получаем список жалоб, и отправляем, если нужны все
                var reportList = await _context.Reports.ToListAsync();

                return Ok(reportList);
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
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Report>> GetReportForAdmin(int id)
        {
            try
            {
                // Ищем жалобу по Id
                Report report = await _context.Reports.FindAsync(id);

                // Если такой нет
                if (report == null)
                    return NotFound($"Не существует жалобы с Id = {id}");

                return Ok(report);
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
        [ProducesResponseType(typeof(IEnumerable<ReportDTO>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<ReportDTO>>> ListForStudent()
        {
            try
            {
                // Ищем жалобы, отправленные студентом с таким-то Id
                IEnumerable<ReportDTO> reportList = await _context.Reports
                    .Where(r => r.Id_student == int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value))
                    .Include(r => r.Status)
                    .Include(r => r.Teacher)
                    .Select(r => new ReportDTO
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
                if (!reportList.Any())
                    return NotFound($"Не существует жалоб, отпарвленных студентом с Id = {User.FindFirst(JwtRegisteredClaimNames.Sub).Value}");

                return Ok(reportList);

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
        [ProducesResponseType(typeof(ReportDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ReportDTO>> GetReportForStudent(int id)
        {
            try
            {
                // Ищем жалобу, с запрашиваемым Id и с Id студента, сделавшего запрос
                ReportDTO report = await _context.Reports
                    .Where(r => r.Id == id && r.Id_student == int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value))
                    .Include(r => r.Status)
                    .Include(r => r.Teacher)
                    .Select(r => new ReportDTO
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
                if (report == null)
                    return NotFound($"Не существует жалобы с Id = {id} и отправителем с Id = {User.FindFirst(JwtRegisteredClaimNames.Sub).Value}");

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Метод позволяет студенту отправить жалобу на преподавателя
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        [ProducesResponseType(typeof(ReportDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ReportDTO>> Add([FromBody] ReportDTO report)
        {
            try
            {
                // Создаем объект полноценной модели жалобы
                Report newReport = new Report()
                {
                    Id_student = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value),
                    Id_teacher = report.Teacher.Id,
                    Date_time = DateTime.Now,
                    Text = report.Text
                };

                // Добавляем и сохраняем жалобу в БД
                await _context.Reports.AddAsync(newReport);
                await _context.SaveChangesAsync();

                return Ok(report);
            }
            catch(Exception ex)
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
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<Report>> Update([FromBody] MessageStatus status, int id)
        {
            try
            {
                // Ищем жалобу по Id
                Report updatedReport = await _context.Reports.FindAsync(id);

                // Если такой жалобы нет
                if (updatedReport == null)
                    return NotFound($"Жалобы с id = {id} не существует");

                // Обновляем статус и сохраняем изменения
                updatedReport.Id_status = status.Id;
                await _context.SaveChangesAsync();

                return Ok(updatedReport);
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
        [ProducesResponseType(typeof(ReportDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ReportDTO>> ChangeActive(int id)
        {
            try
            {
                // Ищем жалобу по Id
                Report report = await _context.Reports.FindAsync(id);

                // Если таковой нет
                if (report == null)
                    return NotFound($"Жалобы с id = {id} не существует");

                // Если студент пытается отозвать/возобновить чужую жалобу
                if (report.Id_student != int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value))
                    return Forbid("Вы не можете отзывать/возобновить жалобы, отправленные не вами");

                // Изменяем жалобу и сохраняем в БД
                report.Is_active = !report.Is_active;
                await _context.SaveChangesAsync();

                return Ok(new ReportDTO
                {
                    Id = report.Id,
                    Date_time = report.Date_time,
                    Text = report.Text,
                    Is_active = report.Is_active
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
