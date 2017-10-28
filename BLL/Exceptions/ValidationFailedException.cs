using System;
using System.Runtime.Serialization;

namespace BLL.Exceptions
{
    [Serializable]
    public class ValidationFailedException : Exception
    {
        public ValidationFailedException()
        {
        }

        public ValidationFailedException(string message) : base(message)
        {
        }

        public ValidationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ValidationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
