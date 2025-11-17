using Challenge_MOTTU.Exceptions;
using Microsoft.EntityFrameworkCore;
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

        public CourseService(AppDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<Course> CreateAsync(CreateCourseRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new CourseNotFoundException("The field Title is required.");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new CourseNotFoundException("The field Description is required.");

            if (string.IsNullOrWhiteSpace(request.Category))
                throw new CourseNotFoundException("The field Category is required.");

            if (string.IsNullOrWhiteSpace(request.Difficulty))
                throw new CourseNotFoundException("The field Difficulty is required.");

            var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists)
                throw new CourseNotFoundException("The provided UserId does not exist.");

            var entity = request.ToEntity();

            _context.Courses.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        // GET BY ID
        public async Task<Course> GetByIdAsync(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                throw new CourseNotFoundException("Course not found.");

            return course;
        }

        // GET COURSES BY USER ID
        public async Task<IEnumerable<Course>> GetByUserIdAsync(int userId)
        {
            var courses = await _context.Courses
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!courses.Any())
                throw new CourseNotFoundException("No courses found for this user.");

            return courses;
        }

        // UPDATE
        public async Task<Course> UpdateAsync(int id, UpdateCourseRequest request)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                throw new CourseNotFoundException("Course not found.");

            request.UpdateEntity(course);

            await _context.SaveChangesAsync();

            return course;
        }

        // DELETE
        public async Task DeleteAsync(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                throw new CourseNotFoundException("Course not found.");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }
}
