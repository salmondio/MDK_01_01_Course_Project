using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Course_project_wpf.Models.DTO
{
    public class EvaluationCreateDTO
    {
        [JsonPropertyName("presentation")]
        [Range(1, 9)]
        public byte Presentation { get; set; }

        [JsonPropertyName("attitude")]
        [Range(1, 9)]
        public byte Attitude { get; set; }

        [JsonPropertyName("responsiveness")]
        [Range(1, 9)]
        public byte Responsiveness { get; set; }

        [JsonPropertyName("id_teacher")]
        public int Id_teacher { get; set; }
    }
}