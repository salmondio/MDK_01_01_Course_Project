using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    public class TeacherDiscipline
    {
        public int Id { get; set; }
        public int Id_teacher { get; set; }
        public int Id_discipline { get; set; }
    }
}
