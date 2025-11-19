namespace Requalify.Exceptions
{
    public class CourseNotFoundException : Exception
    {

        private const string DEFAULT_MESSAGE = "Course not found";

        public CourseNotFoundException() : base(DEFAULT_MESSAGE) { }

        public CourseNotFoundException(string message) : base(message) { }
        
        public CourseNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
