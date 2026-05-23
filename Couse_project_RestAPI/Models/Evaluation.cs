using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    [Table("Evaluation")]
    public class Evaluation
    {
        [Key]
        [Column(Order = 0)]
        public int Id_student { get; set; }

        [Key]
        [Column(Order = 1)]
        public int Id_teacher { get; set; }

        [Required]
        [Range(1, 9)]
        public byte Presentation { get; set; }

        [Required]
        [Range(1, 9)]
        public byte Attitude { get; set; }

        [Required]
        [Range(1, 9)]
        public byte Responsiveness { get; set; }


        [NotMapped]
        public double AverageScore => (Presentation + Attitude + Responsiveness) / 3.0;
    }
}
