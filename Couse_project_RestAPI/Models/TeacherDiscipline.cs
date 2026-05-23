using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    [Table("Teacher_discipline")]
    public class TeacherDiscipline
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Id преподавателя
        /// </summary>
        [Required]
        public int Id_teacher { get; set; }
        /// <summary>
        /// Id дисциплины, которой обучает преподаватель
        /// </summary>
        [Required]
        public int Id_discipline { get; set; }


        [ForeignKey(nameof(Id_teacher))]
        public virtual User? Teacher { get; set; }

        [ForeignKey(nameof(Id_discipline))]
        public virtual User? Discipline { get; set; }
    }
}
