namespace Couse_project_RestAPI.Models.DTO
{
    public class EvaluationDTO
    {
        public byte Presentation { get; set; }
        public byte Attitude { get; set; }
        public byte Responsiveness { get; set; }
        public DateOnly Date { get; set; }
        public DateTime? Date_time { get; set; }
        public UserDTO? Teacher { get; set; }
    }
}
