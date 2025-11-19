using Microsoft.EntityFrameworkCore;
using Requalify.Connection;
using Requalify.Services.Abstractions;
using Requalify.DTOs.Requests;
using Requalify.Mappers;
using Requalify.Model;
using Requalify.Exceptions;

namespace Requalify.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateAsync(CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new UserNotFoundException("The field 'name' is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new UserNotFoundException("The field 'email' is required.");

            var emailInUse = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.Trim() == request.Email.Trim());

            if (emailInUse != null)
                throw new UserNotFoundException("The provided email is already in use.");

            if (string.IsNullOrWhiteSpace(request.Senha))
                throw new UserNotFoundException("The field 'password' is required.");

            if (string.IsNullOrWhiteSpace(request.Telefone))
                throw new UserNotFoundException("The field 'phone' is required.");

            if (request.DataNascimento == default)
                throw new UserNotFoundException("The field 'birthDate' is required.");

            if (string.IsNullOrWhiteSpace(request.CargoAtual))
                throw new UserNotFoundException("The field 'currentRole' is required.");

            if (string.IsNullOrWhiteSpace(request.AreaInteresse))
                throw new UserNotFoundException("The field 'interestArea' is required.");

            var entity = request.ToEntity();

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }


        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _context.Users
                .Include(u => u.Skills)
                .Include(u => u.Courses)
                .Include(u => u.Educations)
                .ToListAsync();

            if (!users.Any())
                throw new UserNotFoundException("No users were found.");

            return users;
        }

        public async Task<User> GetByEmailAsync(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new UserNotFoundException("E-mail cannot be empty.");

            var user = await _context.Users
                .Include(u => u.Skills)
                .Include(u => u.Courses)
                .Include(u => u.Educations)
                .FirstOrDefaultAsync(p => p.Email == email);

            if (user is null)
                throw new UserNotFoundException("No user was found with the provided e-mail.");

            return user;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Skills)
                .Include(u => u.Courses)
                .Include(u => u.Educations)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                throw new UserNotFoundException("User not found.");

            return user;
        }

        public async Task<User> UpdateAsync(int id, UpdateUserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new UserNotFoundException("User not found.");

            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new UserNotFoundException("The field 'name' is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new UserNotFoundException("The field 'email' is required.");

            if (string.IsNullOrWhiteSpace(request.Telefone))
                throw new UserNotFoundException("The field 'phone' is required.");

            if (request.DataNascimento == default)
                throw new UserNotFoundException("The field 'birthDate' is required.");

            if (string.IsNullOrWhiteSpace(request.CargoAtual))
                throw new UserNotFoundException("The field 'currentRole' is required.");

            if (string.IsNullOrWhiteSpace(request.AreaInteresse))
                throw new UserNotFoundException("The field 'interestArea' is required.");

            request.UpdateEntity(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new UserNotFoundException("User not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
