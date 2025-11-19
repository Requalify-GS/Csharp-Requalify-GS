using System.ComponentModel.DataAnnotations;

namespace Requalify.Model
{
    public class User
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public DateTime DataNascimento { get; set; }

        public string CargoAtual { get; set; } = string.Empty;

        public string AreaInteresse { get; set; } = string.Empty;

        public ICollection<Education> Educations { get; set; }
        public ICollection<Skill> Skills { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
