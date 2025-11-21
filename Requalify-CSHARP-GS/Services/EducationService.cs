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
    public class EducationService : IEducationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EducationService> _logger;
        private readonly ActivitySource _activitySource;

        public EducationService(AppDbContext context, ILogger<EducationService> logger, ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
        }

        public async Task<Education> CreateAsync(CreateEducationRequest request)
        {
            using var activity = _activitySource.StartActivity("EducationService.Create", ActivityKind.Internal);
            activity?.SetTag("education.userId", request.UserId);

            _logger.LogInformation("Creating education record for UserId {userId}", request.UserId);

            if (string.IsNullOrWhiteSpace(request.Degree))
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: Degree"));
                throw new EducationNotFoundException("The field Degree is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Instituion))
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: Institution"));
                throw new EducationNotFoundException("The field Institution is required.");
            }

            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (userExists == null)
            {
                activity?.AddEvent(new ActivityEvent("UserId not found"));
                throw new EducationNotFoundException("The provided UserId does not exist.");
            }

            var entity = request.ToEntity();
            _context.Educations.Add(entity);
            await _context.SaveChangesAsync();

            activity?.SetTag("education.id", entity.Id);
            activity?.AddEvent(new ActivityEvent("Education record created successfully"));

            _logger.LogInformation("Education created successfully with ID {id}", entity.Id);

            return entity;
        }

        public async Task<Education> GetByIdAsync(int id)
        {
            using var activity = _activitySource.StartActivity("EducationService.GetById", ActivityKind.Internal);
            activity?.SetTag("education.id", id);

            _logger.LogInformation("Fetching education record with ID {id}", id);

            var education = await _context.Educations.FirstOrDefaultAsync(e => e.Id == id);

            if (education == null)
            {
                activity?.AddEvent(new ActivityEvent("Education record not found"));
                throw new EducationNotFoundException("Education record not found.");
            }

            activity?.AddEvent(new ActivityEvent("Education retrieved successfully"));
            return education;
        }

        public async Task<IEnumerable<Education>> GetAllAsync()
        {
            using var activity = _activitySource.StartActivity("EducationService.GetAll", ActivityKind.Internal);

            _logger.LogInformation("Retrieving all education records");

            var educations = await _context.Educations.ToListAsync();

            if (!educations.Any())
            {
                activity?.AddEvent(new ActivityEvent("No education records found"));
                throw new EducationNotFoundException("No education records found.");
            }

            activity?.SetTag("education.count", educations.Count());
            activity?.AddEvent(new ActivityEvent("Education list retrieved successfully"));

            _logger.LogInformation("{count} education records retrieved", educations.Count());

            return educations;
        }

        public async Task<IEnumerable<Education>> GetByUserIdAsync(int userId)
        {
            using var activity = _activitySource.StartActivity("EducationService.GetByUserId", ActivityKind.Internal);
            activity?.SetTag("education.userId", userId);

            _logger.LogInformation("Retrieving education records for UserId {userId}", userId);

            var educations = await _context.Educations
                .Where(e => e.UserId == userId)
                .ToListAsync();

            if (!educations.Any())
            {
                activity?.AddEvent(new ActivityEvent("No education records found for user"));
                throw new EducationNotFoundException("No education records found for this user.");
            }

            activity?.SetTag("education.count", educations.Count());
            activity?.AddEvent(new ActivityEvent("Education records retrieved successfully"));

            return educations;
        }

        public async Task<Education> UpdateAsync(int id, UpdateEducationRequest request)
        {
            using var activity = _activitySource.StartActivity("EducationService.Update", ActivityKind.Internal);
            activity?.SetTag("education.id", id);

            _logger.LogInformation("Updating education record with ID {id}", id);

            var education = await _context.Educations.FirstOrDefaultAsync(e => e.Id == id);

            if (education == null)
            {
                activity?.AddEvent(new ActivityEvent("Education record not found"));
                throw new EducationNotFoundException("Education record not found.");
            }

            request.UpdateEntity(education);
            await _context.SaveChangesAsync();

            activity?.AddEvent(new ActivityEvent("Education record updated successfully"));

            _logger.LogInformation("Education record with ID {id} updated successfully", id);

            return education;
        }

        public async Task DeleteAsync(int id)
        {
            using var activity = _activitySource.StartActivity("EducationService.Delete", ActivityKind.Internal);
            activity?.SetTag("education.id", id);

            _logger.LogInformation("Deleting education record with ID {id}", id);

            var education = await _context.Educations.FirstOrDefaultAsync(e => e.Id == id);

            if (education == null)
            {
                activity?.AddEvent(new ActivityEvent("Education record not found"));
                throw new EducationNotFoundException("Education record not found.");
            }

            _context.Educations.Remove(education);
            await _context.SaveChangesAsync();

            activity?.AddEvent(new ActivityEvent("Education record deleted successfully"));

            _logger.LogInformation("Education record with ID {id} deleted successfully", id);
        }
    }
}
