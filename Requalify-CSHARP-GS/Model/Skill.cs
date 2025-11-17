using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Requalify.Model
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int ProficiencyPercentage { get; set; }
        public string Description { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
