namespace Requalify.Exceptions
{
    public class UserNotFoundException : Exception
    {

        private const string DEFAULT_MESSAGE = "User not found";

        public UserNotFoundException() : base(DEFAULT_MESSAGE) { }

        public UserNotFoundException(string message) : base(message) { }

        public UserNotFoundException(string message, Exception innerException) : base(message, innerException) { }

    }
}
