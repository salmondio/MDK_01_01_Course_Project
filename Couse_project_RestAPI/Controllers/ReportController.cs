using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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



        [HttpGet("List")]
        [ProducesResponseType(typeof(IEnumerable<Report>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Report>>> List()
        {
            try
            {
                IEnumerable<Report> reportList = await _context.Reports.ToArrayAsync();
                return Ok(reportList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Report>> GetReport(int id)
        {
            try
            {
                Report report = await _context.Reports.FindAsync(id);

                if (report == null)
                    return NotFound($"Жалобы с id = {id} не существует");

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("Add")]
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Report>> Add([FromBody] Report report)
        {
            try
            {
                await _context.Reports.AddAsync(report);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetReport), new {id = report.Id}, report);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("Update")]
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Report>> Update([FromBody] Report report)
        {
            try
            {
                Report updatedReport = await _context.Reports.FindAsync(report.Id);

                if (updatedReport == null)
                    return NotFound($"Жалобы с id = {updatedReport.Id} не существует");

                updatedReport.Id_status = report.Id_status;
                updatedReport.Text = report.Text;

                await _context.SaveChangesAsync();

                return Ok(updatedReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("Delete")]
        [ProducesResponseType(typeof(Report), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Report>> Delete(int id)
        {
            try
            {
                Report report = await _context.Reports.FindAsync(id);

                if (report == null)
                    return NotFound($"Жалобы с id = {id} не существует");

                _context.Reports.Remove(report);
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
