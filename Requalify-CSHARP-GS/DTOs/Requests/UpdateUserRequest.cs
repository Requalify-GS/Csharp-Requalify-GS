namespace Requalify.DTOs.Requests
{
    public class UpdateUserRequest
    {
        public string Nome { get; set; }

        public string Email { get; set; }

        public string Telefone { get; set; }

        public DateTime DataNascimento { get; set; }

        public string CargoAtual { get; set; }

        public string AreaInteresse { get; set; }
    }
}
