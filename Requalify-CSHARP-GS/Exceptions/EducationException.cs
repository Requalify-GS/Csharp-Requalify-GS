namespace Requalify.Exceptions
{
    public class EducationNotFoundException : Exception
    {

        private const string DEFAULT_MESSAGE = "Education not found";

        public EducationNotFoundException() : base(DEFAULT_MESSAGE) { }

        public EducationNotFoundException(string message) : base(message) { }

        public EducationNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
