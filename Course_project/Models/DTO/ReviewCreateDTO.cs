using System.Text.Json.Serialization;

namespace Course_project_wpf.Models.DTO
{
    public class ReviewCreateDTO
    {
        [JsonPropertyName("id_teacher")]
        public int Id_teacher { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}