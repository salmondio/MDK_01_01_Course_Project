using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Course_project_wpf.Models.FullModels
{
    public class TeacherDiscipline
    {
        public int Id { get; set; }
        public int Id_teacher { get; set; }
        public int Id_discipline { get; set; }
    }
}
