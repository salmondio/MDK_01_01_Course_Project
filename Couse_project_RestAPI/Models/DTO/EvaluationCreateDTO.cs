using System.ComponentModel.DataAnnotations;

namespace Couse_project_RestAPI.Models.DTO
{
    public class EvaluationCreateDTO
    {
        [Required]
        [Range(1, 9)]
        public byte Presentation { get; set; }
        [Required]
        [Range(1, 9)]
        public byte Attitude { get; set; }
        [Required]
        [Range(1, 9)]
        public byte Responsiveness { get; set; }
        [Required]
        public int Id_teacher { get; set; }
    }
}
