using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_project_wpf.Models.FullModels
{
    public class Evaluation
    {
        public int Id_student { get; set; }
        public int Id_teacher { get; set; }
        public DateTime Date_time { get; set; }
        [Range(1, 9)]
        public byte Presentation { get; set; }
        [Range(1, 9)]
        public byte Attitude { get; set; }
        [Range(1, 9)]
        public byte Responsiveness { get; set; }
    }
}
