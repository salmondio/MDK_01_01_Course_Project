using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    [Table("Evaluation")]
    public class Evaluation
    {
        [Key]
        public int Id { get; set; }
        public int Id_student { get; set; }
        public int Id_teacher { get; set; }
        public sbyte Presentation { get; set; }
        public sbyte Attitude { get; set; }
        public sbyte Responsiveness { get; set; }
    }
}
