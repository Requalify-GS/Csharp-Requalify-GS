using Microsoft.EntityFrameworkCore;
using Requalify.Connection;
using Requalify.DTOs.Requests;
using Requalify.Exceptions;
using Requalify.Mappers;
using Requalify.Model;
using Requalify.Services.Abstractions;

namespace Requalify.Services
{
    public class EducationService : IEducationService
    {
        private readonly AppDbContext _context;

        public EducationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Education> CreateAsync(CreateEducationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Degree))
                throw new EducationNotFoundException("The field Degree is required.");

            if (string.IsNullOrWhiteSpace(request.Instituion))
                throw new EducationNotFoundException("The field Institution is required.");

            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (userExists == null)
                throw new EducationNotFoundException("The provided UserId does not exist.");

            var entity = request.ToEntity();

            _context.Educations.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<Education> GetByIdAsync(int id)
        {
            var education = await _context.Educations.FirstOrDefaultAsync(e => e.Id == id);

            if (education == null)
                throw new EducationNotFoundException("Education record not found.");

            return education;
        }

        public async Task<IEnumerable<Education>> GetAllAsync()
        {
            var educations = await _context.Educations.ToListAsync();

            if (!educations.Any())
                throw new EducationNotFoundException("No education records found.");

            return educations;
        }

        public async Task<IEnumerable<Education>> GetByUserIdAsync(int userId)
        {
            var educations = await _context.Educations
                .Where(e => e.UserId == userId)
                .ToListAsync();

            if (!educations.Any())
                throw new EducationNotFoundException("No education records found for this user.");

            return educations;
        }

        public async Task<Education> UpdateAsync(int id, UpdateEducationRequest request)
        {
            var education = await _context.Educations.FirstOrDefaultAsync(e => e.Id == id);

            if (education == null)
                throw new EducationNotFoundException("Education record not found.");

            request.UpdateEntity(education);

            await _context.SaveChangesAsync();

            return education;
        }

        public async Task DeleteAsync(int id)
        {
            var education = await _context.Educations.FirstOrDefaultAsync(e => e.Id == id);

            if (education == null)
                throw new EducationNotFoundException("Education record not found.");

            _context.Educations.Remove(education);
            await _context.SaveChangesAsync();
        }
    }
}
