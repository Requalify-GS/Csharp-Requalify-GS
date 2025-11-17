namespace Requalify.DTOs.Requests
{
    public class UpdateSkillRequest
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public int ProficiencyPercentage { get; set; }
        public string Description { get; set; }
    }

}
