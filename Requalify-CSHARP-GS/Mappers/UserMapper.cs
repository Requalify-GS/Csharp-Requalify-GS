using Requalify.DTOs.Requests;
using Requalify.DTOs.Responses;
using Requalify.Model;

namespace Requalify.Mappers
{
    public static class UserMapper
    {
        public static User ToEntity(this CreateUserRequest request)
        {
            return new User
            {
                Nome = request.Nome,
                Email = request.Email,
                Senha = request.Senha,
                Telefone = request.Telefone,
                DataNascimento = request.DataNascimento,
                CargoAtual = request.CargoAtual,
                AreaInteresse = request.AreaInteresse
            };
        }

        public static void UpdateEntity(this UpdateUserRequest request, User entity)
        {
            entity.Nome = request.Nome;
            entity.Email = request.Email;
            entity.Telefone = request.Telefone;
            entity.DataNascimento = request.DataNascimento;
            entity.CargoAtual = request.CargoAtual;
            entity.AreaInteresse = request.AreaInteresse;
        }

        public static UserResponse ToResponse(this User entity)
        {
            return new UserResponse
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Email = entity.Email,
                Telefone = entity.Telefone,
                DataNascimento = entity.DataNascimento,
                CargoAtual = entity.CargoAtual,
                AreaInteresse = entity.AreaInteresse,
                Skills = entity.Skills?.Select(s => s.ToResponse()).ToList(),
                Courses = entity.Courses?.Select(c => c.ToResponse()).ToList(),
                Educations = entity.Educations?.Select(e => e.ToResponse()).ToList()
            };
        }
    }
}
