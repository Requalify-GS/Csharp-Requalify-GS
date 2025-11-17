namespace Requalify.DTOs.Requests
{
    public class UpdateEducationRequest
    {
        public string Degree { get; set; }
        public string Instituion { get; set; }
        public DateTime CompletionDate { get; set; }
        public string Certificate { get; set; }
    }

}
