using Requalify.DTOs.Requests;
using Requalify.DTOs.Responses;
using Requalify.Model;

namespace Requalify.Mappers
{
    public static class SkillMapper
    {
        public static Skill ToEntity(this CreateSkillRequest request)
        {
            return new Skill
            {
                Name = request.Name,
                Level = request.Level,
                Category = request.Category,
                ProficiencyPercentage = request.ProficiencyPercentage,
                Description = request.Description,
                UserId = request.UserId
            };
        }

        public static void UpdateEntity(this UpdateSkillRequest request, Skill entity)
        {
            entity.Name = request.Name;
            entity.Level = request.Level;
            entity.Category = request.Category;
            entity.ProficiencyPercentage = request.ProficiencyPercentage;
            entity.Description = request.Description;
        }

        public static SkillResponse ToResponse(this Skill entity)
        {
            return new SkillResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Level = entity.Level,
                Category = entity.Category,
                ProficiencyPercentage = entity.ProficiencyPercentage,
                Description = entity.Description
            };
        }
    }
}
