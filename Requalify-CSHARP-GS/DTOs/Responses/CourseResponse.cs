using Requalify.Hateoas;

namespace Requalify.DTOs.Responses
{
    public class CourseResponse : ResourceResponse  
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public string Url { get; set; }
    }

}
