using System.ComponentModel.DataAnnotations;

namespace Couse_project_RestAPI.Models.DTO
{
    public class ReportCreateDTO
    {
        [Required]
        public int Id_teacher { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
