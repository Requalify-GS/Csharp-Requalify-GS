using Requalify.Hateoas;

namespace Requalify.DTOs.Responses
{
    public class EducationResponse : ResourceResponse
    {
        public int Id { get; set; }
        public string Degree { get; set; }
        public string Instituion { get; set; }
        public DateTime CompletionDate { get; set; }
        public string Certificate { get; set; }
    }

}
