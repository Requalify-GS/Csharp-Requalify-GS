using Requalify.DTOs.Requests;
using Requalify.Model;

namespace Requalify.Services.Abstractions
{
    public interface ISkillService
    {
        Task<Skill> CreateAsync(CreateSkillRequest request);
        Task<IEnumerable<Skill>> GetAllAsync();
        Task<Skill> GetByIdAsync(int id);
        Task<IEnumerable<Skill>> GetByUserIdAsync(int userId);
        Task<Skill> UpdateAsync(int id, UpdateSkillRequest request);
        Task DeleteAsync(int id);
    }
}
