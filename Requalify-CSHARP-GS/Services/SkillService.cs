using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Requalify.Connection;
using Requalify.DTOs.Requests;
using Requalify.Exceptions;
using Requalify.Mappers;
using Requalify.Model;
using Requalify.Services.Abstractions;

namespace Requalify.Services
{
    public class SkillService : ISkillService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SkillService> _logger;
        private readonly ActivitySource _activitySource;

        public SkillService(AppDbContext context, ILogger<SkillService> logger, ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
        }

        public async Task<Skill> CreateAsync(CreateSkillRequest request)
        {
            using var activity = _activitySource.StartActivity("SkillService.CreateSkill", ActivityKind.Internal);
            activity?.SetTag("skill.userId", request.UserId);
            activity?.SetTag("skill.name", request.Name);

            _logger.LogInformation("Creating new skill for UserId {userId}", request.UserId);

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: Name"));
                throw new SkillNotFoundException("The field Name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Level))
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: Level"));
                throw new SkillNotFoundException("The field Level is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Category))
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: Category"));
                throw new SkillNotFoundException("The field Category is required.");
            }

            if (request.ProficiencyPercentage < 0 || request.ProficiencyPercentage > 100)
            {
                activity?.AddEvent(new ActivityEvent("Invalid Proficiency Percentage"));
                throw new SkillNotFoundException("Proficiency must be between 0 and 100.");
            }

            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (userExists == null)
            {
                activity?.AddEvent(new ActivityEvent("UserId does not exist"));
                throw new SkillNotFoundException("The provided UserId does not exist.");
            }

            var entity = request.ToEntity();
            _context.Skills.Add(entity);
            await _context.SaveChangesAsync();

            activity?.SetTag("skill.id", entity.Id);
            activity?.AddEvent(new ActivityEvent("Skill created successfully"));

            _logger.LogInformation("Skill created successfully with ID {id}", entity.Id);

            return entity;
        }

        public async Task<IEnumerable<Skill>> GetAllAsync()
        {
            using var activity = _activitySource.StartActivity("SkillService.GetAllSkills", ActivityKind.Internal);

            _logger.LogInformation("Retrieving all skills");

            var skills = await _context.Skills.ToListAsync();

            if (!skills.Any())
            {
                activity?.AddEvent(new ActivityEvent("No skill records found"));
                throw new CourseNotFoundException("No skills records found.");
            }

            activity?.SetTag("skills.count", skills.Count());
            activity?.AddEvent(new ActivityEvent("Skills retrieved successfully"));

            _logger.LogInformation("Returned {count} skills", skills.Count());
            return skills;
        }

        public async Task<Skill> GetByIdAsync(int id)
        {
            using var activity = _activitySource.StartActivity("SkillService.GetById", ActivityKind.Internal);
            activity?.SetTag("skill.id", id);

            _logger.LogInformation("Retrieving skill with ID {id}", id);

            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
            {
                activity?.AddEvent(new ActivityEvent("Skill not found"));
                throw new SkillNotFoundException("Skill not found.");
            }

            activity?.AddEvent(new ActivityEvent("Skill found successfully"));

            return skill;
        }

        public async Task<IEnumerable<Skill>> GetByUserIdAsync(int userId)
        {
            using var activity = _activitySource.StartActivity("SkillService.GetByUserId", ActivityKind.Internal);
            activity?.SetTag("skill.userId", userId);

            _logger.LogInformation("Retrieving skills for UserId {userId}", userId);

            var skills = await _context.Skills
                .Where(s => s.UserId == userId)
                .ToListAsync();

            if (!skills.Any())
            {
                activity?.AddEvent(new ActivityEvent("No skills found for given user"));
                throw new SkillNotFoundException("No skills found for this user.");
            }

            activity?.SetTag("skills.count", skills.Count());
            activity?.AddEvent(new ActivityEvent("Skills retrieved successfully"));

            return skills;
        }

        public async Task<Skill> UpdateAsync(int id, UpdateSkillRequest request)
        {
            using var activity = _activitySource.StartActivity("SkillService.UpdateSkill", ActivityKind.Internal);
            activity?.SetTag("skill.id", id);

            _logger.LogInformation("Updating skill with ID {id}", id);

            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
            {
                activity?.AddEvent(new ActivityEvent("Skill to update not found"));
                throw new SkillNotFoundException("Skill not found.");
            }

            request.UpdateEntity(skill);
            await _context.SaveChangesAsync();

            activity?.AddEvent(new ActivityEvent("Skill updated successfully"));

            _logger.LogInformation("Skill with ID {id} updated successfully", id);

            return skill;
        }

        public async Task DeleteAsync(int id)
        {
            using var activity = _activitySource.StartActivity("SkillService.DeleteSkill", ActivityKind.Internal);
            activity?.SetTag("skill.id", id);

            _logger.LogInformation("Deleting skill with ID {id}", id);

            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
            {
                activity?.AddEvent(new ActivityEvent("Skill to delete not found"));
                throw new SkillNotFoundException("Skill not found.");
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            activity?.AddEvent(new ActivityEvent("Skill deleted successfully"));

            _logger.LogInformation("Skill with ID {id} deleted successfully", id);
        }
    }
}
