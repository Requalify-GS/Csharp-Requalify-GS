using Requalify.DTOs.Requests;
using Requalify.Model;

public interface IUserService
{
    Task<User> CreateAsync(CreateUserRequest request);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(int id);
    Task<User> GetByEmailAsync(string email);
    Task<User> UpdateAsync(int id, UpdateUserRequest request);
    Task DeleteAsync(int id);
}
