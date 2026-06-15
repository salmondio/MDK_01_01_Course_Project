using System.Text.Json.Serialization;

namespace Course_project_wpf.Models.DTO
{
    public class EvaluationDTO
    {
        [JsonPropertyName("presentation")]
        public byte Presentation { get; set; }

        [JsonPropertyName("attitude")]
        public byte Attitude { get; set; }

        [JsonPropertyName("responsiveness")]
        public byte Responsiveness { get; set; }

        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        [JsonPropertyName("date_time")]
        public DateTime? Date_time { get; set; }

        [JsonPropertyName("teacher")]
        public UserDTO? Teacher { get; set; }
    }
}