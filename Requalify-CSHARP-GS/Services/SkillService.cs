using Microsoft.EntityFrameworkCore;
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

        public SkillService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Skill> CreateAsync(CreateSkillRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new SkillNotFoundException("The field Name is required.");

            if (string.IsNullOrWhiteSpace(request.Level))
                throw new SkillNotFoundException("The field Level is required.");

            if (string.IsNullOrWhiteSpace(request.Category))
                throw new SkillNotFoundException("The field Category is required.");

            if (request.ProficiencyPercentage < 0 || request.ProficiencyPercentage > 100)
                throw new SkillNotFoundException("Proficiency must be between 0 and 100.");

            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (userExists == null)
                throw new SkillNotFoundException("The provided UserId does not exist.");

            var entity = request.ToEntity();

            _context.Skills.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<Skill>> GetAllAsync()
        {
            var skills = await _context.Skills.ToListAsync();

            if (!skills.Any())
                throw new CourseNotFoundException("No skills records found.");

            return skills;
        }

        public async Task<Skill> GetByIdAsync(int id)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
                throw new SkillNotFoundException("Skill not found.");

            return skill;
        }

        public async Task<IEnumerable<Skill>> GetByUserIdAsync(int userId)
        {
            var skills = await _context.Skills
                .Where(s => s.UserId == userId)
                .ToListAsync();

            if (!skills.Any())
                throw new SkillNotFoundException("No skills found for this user.");

            return skills;
        }

        public async Task<Skill> UpdateAsync(int id, UpdateSkillRequest request)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
                throw new SkillNotFoundException("Skill not found.");

            request.UpdateEntity(skill);

            await _context.SaveChangesAsync();

            return skill;
        }

        public async Task DeleteAsync(int id)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
                throw new SkillNotFoundException("Skill not found.");

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
        }
    }
}
