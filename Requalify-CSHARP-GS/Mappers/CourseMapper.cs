using Requalify.DTOs.Requests;
using Requalify.DTOs.Responses;
using Requalify.Model;

namespace Requalify.Mappers
{
    public static class CourseMapper
    {
        public static Course ToEntity(this CreateCourseRequest request)
        {
            return new Course
            {
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                Difficulty = request.Difficulty,
                Url = request.Url,
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(this UpdateCourseRequest request, Course entity)
        {
            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.Category = request.Category;
            entity.Difficulty = request.Difficulty;
            entity.Url = request.Url;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        public static CourseResponse ToResponse(this Course entity)
        {
            return new CourseResponse
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Category = entity.Category,
                Difficulty = entity.Difficulty,
                Url = entity.Url
            };
        }
    }
}
