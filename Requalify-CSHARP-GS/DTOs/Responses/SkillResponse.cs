using Requalify.Hateoas;

namespace Requalify.DTOs.Responses
{
    public class SkillResponse : ResourceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public int ProficiencyPercentage { get; set; }
        public string Description { get; set; }
    }

}
