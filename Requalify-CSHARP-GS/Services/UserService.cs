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

        // CREATE
        public async Task<User> CreateAsync(CreateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new UserNotFoundException("O campo Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new UserNotFoundException("O campo Email é obrigatório.");

            var emailEmUso = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.Trim() == request.Email.Trim());

            if (emailEmUso != null)
                throw new UserNotFoundException("O email informado já está em uso.");

            if (string.IsNullOrWhiteSpace(request.Senha))
                throw new UserNotFoundException("O campo Senha é obrigatório.");

            var entity = request.ToEntity();

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        // GET ALL
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _context.Users
                .Include(u => u.Skills)
                .Include(u => u.Courses)
                .Include(u => u.Educations)
                .ToListAsync();

            if (!users.Any())
                throw new UserNotFoundException("Nenhum usuário encontrado.");

            return users;
        }

        // GET BY EMAIL
        public async Task<User> GetByEmailAsync(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new UserNotFoundException("E-mail está vazio.");

            var user = await _context.Users
                .Include(u => u.Skills)
                .Include(u => u.Courses)
                .Include(u => u.Educations)
                .FirstOrDefaultAsync(p => p.Email == email);

            if (user is null)
                throw new UserNotFoundException("Usuário com este e-mail não existe.");

            return user;
        }

        // GET BY ID
        public async Task<User> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .Include(u => u.Skills)
                .Include(u => u.Courses)
                .Include(u => u.Educations)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                throw new UserNotFoundException("Usuário não encontrado.");

            return user;
        }

        // UPDATE
        public async Task<User> UpdateAsync(int id, UpdateUserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new UserNotFoundException("Usuário não encontrado.");

            request.UpdateEntity(user);

            await _context.SaveChangesAsync();

            return user;
        }

        // DELETE
        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new UserNotFoundException("Usuário não encontrado.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
