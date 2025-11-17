namespace Requalify.DTOs.Requests
{
    public class CreateEducationRequest
    {
        public string Degree { get; set; }
        public string Instituion { get; set; }
        public DateTime CompletionDate { get; set; }
        public string Certificate { get; set; }
        public int UserId { get; set; }
    }

}
