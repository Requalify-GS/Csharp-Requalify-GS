namespace Requalify.Exceptions
{
    public class SkillNotFoundException : Exception
    {

        private const string DEFAULT_MESSAGE = "Skill not found";

        public SkillNotFoundException() : base(DEFAULT_MESSAGE) { }

        public SkillNotFoundException(string message) : base(message) { }

        public SkillNotFoundException(string? message, Exception? innerException) : base(message, innerException){ }
    }
}
