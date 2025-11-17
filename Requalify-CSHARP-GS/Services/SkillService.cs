using Microsoft.EntityFrameworkCore;
using Requalify.Connection;
using Requalify.Services.Abstractions;
using Requalify.DTOs.Requests;
using Requalify.Mappers;
using Requalify.Model;
using Requalify.Exceptions;

namespace Requalify.Services
{
    public class SkillService : ISkillService
    {
        private readonly AppDbContext _context;

        public SkillService(AppDbContext context)
        {
            _context = context;
        }

        // CREATE
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

            // Validate the UserId
            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
                throw new SkillNotFoundException("The provided UserId does not exist.");

            var entity = request.ToEntity();

            _context.Skills.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        // GET SKILL BY ID
        public async Task<Skill> GetByIdAsync(int id)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
                throw new SkillNotFoundException("Skill not found.");

            return skill;
        }

        // GET SKILLS BY USER ID
        public async Task<IEnumerable<Skill>> GetByUserIdAsync(int userId)
        {
            var skills = await _context.Skills
                .Where(s => s.UserId == userId)
                .ToListAsync();

            if (!skills.Any())
                throw new SkillNotFoundException("No skills found for this user.");

            return skills;
        }

        // UPDATE
        public async Task<Skill> UpdateAsync(int id, UpdateSkillRequest request)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
                throw new SkillNotFoundException("Skill not found.");

            request.UpdateEntity(skill);

            await _context.SaveChangesAsync();

            return skill;
        }

        // DELETE
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
