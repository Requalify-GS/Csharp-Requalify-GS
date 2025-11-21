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
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly ActivitySource _activitySource;

        public UserService(AppDbContext context, ILogger<UserService> logger, ActivitySource activitySource)
        {
            _context = context;
            _logger = logger;
            _activitySource = activitySource;
        }

        public async Task<User> CreateAsync(CreateUserRequest request)
        {
            using var activity = _activitySource.StartActivity("UserService.CreateUser", ActivityKind.Internal);
            activity?.SetTag("user.email", request.Email);
            activity?.SetTag("user.name", request.Nome);

            _logger.LogInformation("Creating new user with email: {email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Nome))
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: Nome"));
                throw new UserNotFoundException("The field 'name' is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: Email"));
                throw new UserNotFoundException("The field 'email' is required.");
            }

            var emailInUse = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.Trim() == request.Email.Trim());

            if (emailInUse != null)
            {
                activity?.AddEvent(new ActivityEvent("Email already in use"));
                throw new UserNotFoundException("The provided email is already in use.");
            }

            if (string.IsNullOrWhiteSpace(request.Senha))
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: Senha"));
                throw new UserNotFoundException("The field 'password' is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Telefone))
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: Telefone"));
                throw new UserNotFoundException("The field 'phone' is required.");
            }

            if (request.DataNascimento == default)
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: DataNascimento"));
                throw new UserNotFoundException("The field 'birthDate' is required.");
            }

            if (string.IsNullOrWhiteSpace(request.CargoAtual))
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: CargoAtual"));
                throw new UserNotFoundException("The field 'currentRole' is required.");
            }

            if (string.IsNullOrWhiteSpace(request.AreaInteresse))
            {
                activity?.AddEvent(new ActivityEvent("Missing required field: AreaInteresse"));
                throw new UserNotFoundException("The field 'interestArea' is required.");
            }

            var entity = request.ToEntity();
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            activity?.SetTag("user.id", entity.Id);
            activity?.AddEvent(new ActivityEvent("User created successfully"));

            _logger.LogInformation("User created successfully with ID {id}", entity.Id);

            return entity;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var activity = _activitySource.StartActivity("UserService.GetAllUsers", ActivityKind.Internal);

            _logger.LogInformation("Retrieving all users");

            var users = await _context.Users
                .Include(u => u.Skills)
                .Include(u => u.Courses)
                .Include(u => u.Educations)
                .ToListAsync();

            if (!users.Any())
            {
                activity?.AddEvent(new ActivityEvent("No users found"));
                throw new UserNotFoundException("No users were found.");
            }

            activity?.SetTag("users.count", users.Count);
            activity?.AddEvent(new ActivityEvent("Users retrieved successfully"));

            _logger.LogInformation("Returned {count} users", users.Count);
            return users;
        }

        public async Task<User> GetByEmailAsync(string? email)
        {
            using var activity = _activitySource.StartActivity("UserService.GetByEmail", ActivityKind.Internal);
            activity?.SetTag("user.email", email);

            _logger.LogInformation("Searching user by email: {email}", email);

            if (string.IsNullOrWhiteSpace(email))
                throw new UserNotFoundException("E-mail cannot be empty.");

            var user = await _context.Users
                .Include(u => u.Skills)
                .Include(u => u.Courses)
                .Include(u => u.Educations)
                .FirstOrDefaultAsync(p => p.Email == email);

            if (user is null)
            {
                activity?.AddEvent(new ActivityEvent("User not found"));
                throw new UserNotFoundException("No user was found with the provided e-mail.");
            }

            activity?.SetTag("user.id", user.Id);

            _logger.LogInformation("User {id} found by email", user.Id);
            return user;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            using var activity = _activitySource.StartActivity("UserService.GetById", ActivityKind.Internal);
            activity?.SetTag("user.id", id);

            _logger.LogInformation("Searching user by ID: {id}", id);

            var user = await _context.Users
                .Include(u => u.Skills)
                .Include(u => u.Courses)
                .Include(u => u.Educations)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                activity?.AddEvent(new ActivityEvent("User not found"));
                throw new UserNotFoundException("User not found.");
            }

            activity?.AddEvent(new ActivityEvent("User retrieved successfully"));

            _logger.LogInformation("User {id} found successfully", id);
            return user;
        }

        public async Task<User> UpdateAsync(int id, UpdateUserRequest request)
        {
            using var activity = _activitySource.StartActivity("UserService.UpdateUser", ActivityKind.Internal);
            activity?.SetTag("user.id", id);

            _logger.LogInformation("Updating user {id}", id);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                activity?.AddEvent(new ActivityEvent("User not found for update"));
                throw new UserNotFoundException("User not found.");
            }

            request.UpdateEntity(user);
            await _context.SaveChangesAsync();

            activity?.AddEvent(new ActivityEvent("User updated successfully"));

            _logger.LogInformation("User {id} updated successfully", id);
            return user;
        }

        public async Task DeleteAsync(int id)
        {
            using var activity = _activitySource.StartActivity("UserService.DeleteUser", ActivityKind.Internal);
            activity?.SetTag("user.id", id);

            _logger.LogInformation("Deleting user {id}", id);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                activity?.AddEvent(new ActivityEvent("User not found for deletion"));
                throw new UserNotFoundException("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            activity?.AddEvent(new ActivityEvent("User deleted successfully"));

            _logger.LogInformation("User {id} deleted successfully", id);
        }
    }
}
