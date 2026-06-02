using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Course_project_wpf.Models.FullModels
{
    public class Evaluation
    {
        [JsonPropertyName("id_student")]
        public int IdStudent { get; set; }

        [JsonPropertyName("id_teacher")]
        public int IdTeacher { get; set; }

        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("presentation")]
        [Range(1, 9)]
        public int Presentation { get; set; }

        [JsonPropertyName("attitude")]
        [Range(1, 9)]
        public int Attitude { get; set; }

        [JsonPropertyName("responsiveness")]
        [Range(1, 9)]
        public int Responsiveness { get; set; }

        public double Average => (Presentation + Attitude + Responsiveness) / 3;
    }
}
