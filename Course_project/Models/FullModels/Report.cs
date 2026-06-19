using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Course_project_wpf.Models.FullModels
{
    public class Report
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("id_student")]
        public int Id_student { get; set; }
        [JsonPropertyName("id_teacher")]
        public int Id_teacher { get; set; }
        [JsonPropertyName("id_status")]
        public int Id_status { get; set; }
        [JsonPropertyName("id_inspector")]
        public int? Id_inspector { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime Date_time { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("is_active")]
        public bool Is_active { get; set; }
    }
}
