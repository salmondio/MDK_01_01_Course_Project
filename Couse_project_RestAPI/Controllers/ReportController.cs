using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
using Couse_project_RestAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        [Authorize(Roles = "Admin,Owner")]
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
        [Authorize(Roles = "Admin,Owner")]
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
                    .Where(r => r.Id_student == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
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
                    return NotFound($"Не существует жалоб, отпарвленных студентом с Id = {User.FindFirst(ClaimTypes.NameIdentifier).Value}");

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
                    .Where(r => r.Id == id && r.Id_student == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
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
                    return NotFound($"Не существует жалобы с Id = {id} и отправителем с Id = {User.FindFirst(ClaimTypes.NameIdentifier).Value}");

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
        [ProducesResponseType(typeof(ReportCreateDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<ReportCreateDTO>> Add([FromBody] ReportCreateDTO report)
        {
            try
            {
                User? teacher = await _context.Users.FindAsync(report.Id_teacher);
                // Если такого препода нет
                if (teacher == null ||
                    teacher.Id_role != 5)
                    return BadRequest($"Преподавателя с указанными Id не существует.");

                // Создаем объект полноценной модели жалобы
                Report newReport = new Report()
                {
                    Id_student = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                    Id_teacher = report.Id_teacher,
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
        /// Метод позволяет студенту отправить жалобу на преподавателя
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        [HttpPost("Owner/Add")]
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Report>> OwnerAdd([FromBody] Report report)
        {
            try
            {
                if (await _context.Reports.AnyAsync(r => r.Id == report.Id))
                    return BadRequest($"Жалоба с Id = {report.Id} уже существует");

                User? student = await _context.Users.FindAsync(report.Id_student);
                User? teacher = await _context.Users.FindAsync(report.Id_teacher);
                // Если такого студента или препода нет
                if (student == null || teacher == null ||
                    student.Id_role != 4 ||
                    teacher.Id_role != 5)
                    return BadRequest($"Студента или преподавателя с указанными Id не существует.");

                // Добавляем и сохраняем жалобу в БД
                await _context.Reports.AddAsync(report);
                await _context.SaveChangesAsync();

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Метод позволяет обновить статус жалобы администратору
        /// </summary>
        /// <param name="statusId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("UpdateStatus/{id}")]
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner,Admin,Moderator")]
        public async Task<ActionResult<Report>> Update([FromBody]int statusId, int id)
        {
            try
            {
                // Ищем жалобу по Id
                Report? updatedReport = await _context.Reports.FindAsync(id);

                // Если такой жалобы нет
                if (updatedReport == null)
                    return NotFound($"Жалобы с id = {id} не существует");

                // Обновляем статус и сохраняем изменения
                updatedReport.Id_status = statusId;
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
                if (report.Id_student != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
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


        /// <summary>
        /// Метод позволяет обновить статус жалобы администратору
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        [HttpPut("Owner/Update")]
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Report>> Update([FromBody] Report report)
        {
            try
            {
                // Ищем жалобу по Id
                Report updatedReport = await _context.Reports.FindAsync(report.Id);

                // Если такой жалобы нет
                if (updatedReport == null)
                    return NotFound($"Жалобы с id = {report.Id} не существует");

                User? student = await _context.Users.FindAsync(report.Id_student);
                User? teacher = await _context.Users.FindAsync(report.Id_teacher);
                // Если такого студента или препода нет
                if (student == null || teacher == null ||
                    student.Id_role != 4 ||
                    teacher.Id_role != 5)
                    return BadRequest($"Студента или преподавателя с указанными Id не существует.");

                // Обновляем статус и сохраняем изменения
                updatedReport.Id_student = report.Id_student;
                updatedReport.Id_teacher = report.Id_teacher;
                updatedReport.Id_status = report.Id_status;
                updatedReport.Id_inspector = report.Id_inspector;
                updatedReport.Date_time = DateTime.Now;
                updatedReport.Text = report.Text;
                updatedReport.Is_active = report.Is_active;
                await _context.SaveChangesAsync();

                return Ok(updatedReport);
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
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Report>> Delete(int id)
        {
            try
            {
                // Ищем жалобу по Id
                Report deletedReport = await _context.Reports.FindAsync(id);

                // Если такой жалобы нет
                if (deletedReport == null)
                    return NotFound($"Жалобы с id = {id} не существует");

                // Обновляем статус и сохраняем изменения
                _context.Reports.Remove(deletedReport);
                await _context.SaveChangesAsync();

                return Ok(deletedReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
