using Requalify.Hateoas;

namespace Requalify.DTOs.Responses
{
    public class UserResponse : ResourceResponse
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Telefone { get; set; }

        public DateTime DataNascimento { get; set; }

        public string CargoAtual { get; set; }

        public string AreaInteresse { get; set; }

        public List<SkillResponse> Skills { get; set; }

        public List<CourseResponse> Courses { get; set; }

        public List<EducationResponse> Educations { get; set; }
    }
}
