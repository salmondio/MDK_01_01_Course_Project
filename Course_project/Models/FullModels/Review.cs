using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Course_project_wpf.Models.FullModels
{
    public class Review
    {
        public int Id { get; set; }
        public int Id_student { get; set; }
        public int Id_teacher { get; set; }
        public int Id_status { get; set; }
        public int? Id_inspector { get; set; }
        public DateTime Date_time { get; set; }
        public string Text { get; set; }
        public bool Is_active { get; set; }
    }
}
