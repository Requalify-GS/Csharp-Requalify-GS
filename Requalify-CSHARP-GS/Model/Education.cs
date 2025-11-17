namespace Requalify.Model
{
    public class Education
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string Instituion { get; set; } = string.Empty;
        public DateTime CompletionDate {  get; set; }
        public string Certificate { get; set; } = string.Empty;
        public User User { get; set; }
    }
}
