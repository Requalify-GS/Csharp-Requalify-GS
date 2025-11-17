using Requalify.DTOs.Requests;
using Requalify.Model;

namespace Requalify.Services.Abstractions
{
    public interface ICourseService
    {
        Task<Course> CreateAsync(CreateCourseRequest request);
        Task<Course> GetByIdAsync(int id);
        Task<IEnumerable<Course>> GetByUserIdAsync(int userId);
        Task<Course> UpdateAsync(int id, UpdateCourseRequest request);
        Task DeleteAsync(int id);
    }
}
