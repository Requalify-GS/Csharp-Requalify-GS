using Requalify.DTOs.Requests;
using Requalify.Model;

namespace Requalify.Services.Abstractions
{
    public interface IEducationService
    {
        Task<Education> CreateAsync(CreateEducationRequest request);
        Task<Education> GetByIdAsync(int id);
        Task<IEnumerable<Education>> GetByUserIdAsync(int userId);
        Task<Education> UpdateAsync(int id, UpdateEducationRequest request);
        Task DeleteAsync(int id);
    }
}
