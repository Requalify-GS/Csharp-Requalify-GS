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
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CourseService> _logger;
        private readonly ActivitySource _activitySource;

        public CourseService(AppDbContext context, ILogger<CourseService> logger, ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
        }

        public async Task<Course> CreateAsync(CreateCourseRequest request)
        {
            using var activity = _activitySource.StartActivity("CourseService.Create", ActivityKind.Internal);
            activity?.SetTag("course.userId", request.UserId);

            _logger.LogInformation("Creating new course for UserId {userId}", request.UserId);

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                activity?.AddEvent(new ActivityEvent("Missing Title"));
                throw new CourseNotFoundException("The field Title is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                activity?.AddEvent(new ActivityEvent("Missing Description"));
                throw new CourseNotFoundException("The field Description is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Category))
            {
                activity?.AddEvent(new ActivityEvent("Missing Category"));
                throw new CourseNotFoundException("The field Category is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Difficulty))
            {
                activity?.AddEvent(new ActivityEvent("Missing Difficulty"));
                throw new CourseNotFoundException("The field Difficulty is required.");
            }

            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (userExists == null)
            {
                activity?.AddEvent(new ActivityEvent("User not found"));
                throw new CourseNotFoundException("The provided UserId does not exist.");
            }

            var entity = request.ToEntity();
            _context.Courses.Add(entity);
            await _context.SaveChangesAsync();

            activity?.SetTag("course.id", entity.Id);
            activity?.AddEvent(new ActivityEvent("Course created successfully"));

            _logger.LogInformation("Course created successfully with ID {id}", entity.Id);

            return entity;
        }

        public async Task<Course> GetByIdAsync(int id)
        {
            using var activity = _activitySource.StartActivity("CourseService.GetById", ActivityKind.Internal);
            activity?.SetTag("course.id", id);

            _logger.LogInformation("Retrieving course with ID {id}", id);

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                activity?.AddEvent(new ActivityEvent("Course not found"));
                throw new CourseNotFoundException("Course not found.");
            }

            activity?.AddEvent(new ActivityEvent("Course retrieved successfully"));
            return course;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            using var activity = _activitySource.StartActivity("CourseService.GetAll", ActivityKind.Internal);

            _logger.LogInformation("Retrieving all courses");

            var courses = await _context.Courses.ToListAsync();

            if (!courses.Any())
            {
                activity?.AddEvent(new ActivityEvent("No courses found"));
                throw new CourseNotFoundException("No courses records found.");
            }

            activity?.SetTag("course.count", courses.Count());
            _logger.LogInformation("{count} courses retrieved", courses.Count);

            return courses;
        }

        public async Task<IEnumerable<Course>> GetByUserIdAsync(int userId)
        {
            using var activity = _activitySource.StartActivity("CourseService.GetByUserId", ActivityKind.Internal);
            activity?.SetTag("course.userId", userId);

            _logger.LogInformation("Retrieving courses for UserId {userId}", userId);

            var courses = await _context.Courses
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!courses.Any())
            {
                activity?.AddEvent(new ActivityEvent("No courses for this user"));
                throw new CourseNotFoundException("No courses found for this user.");
            }

            activity?.SetTag("course.count", courses.Count());
            return courses;
        }

        public async Task<Course> UpdateAsync(int id, UpdateCourseRequest request)
        {
            using var activity = _activitySource.StartActivity("CourseService.Update", ActivityKind.Internal);
            activity?.SetTag("course.id", id);

            _logger.LogInformation("Updating course with ID {id}", id);

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                activity?.AddEvent(new ActivityEvent("Course not found"));
                throw new CourseNotFoundException("Course not found.");
            }

            request.UpdateEntity(course);
            await _context.SaveChangesAsync();

            activity?.AddEvent(new ActivityEvent("Course updated successfully"));
            _logger.LogInformation("Course {id} updated successfully", id);

            return course;
        }

        public async Task DeleteAsync(int id)
        {
            using var activity = _activitySource.StartActivity("CourseService.Delete", ActivityKind.Internal);
            activity?.SetTag("course.id", id);

            _logger.LogInformation("Deleting course with ID {id}", id);

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                activity?.AddEvent(new ActivityEvent("Course not found"));
                throw new CourseNotFoundException("Course not found.");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            activity?.AddEvent(new ActivityEvent("Course deleted successfully"));
            _logger.LogInformation("Course {id} deleted successfully", id);
        }
    }
}
