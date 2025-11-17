namespace Requalify.DTOs.Requests
{
    public class CreateCourseRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public string Url { get; set; }
        public int UserId { get; set; }
    }

}
