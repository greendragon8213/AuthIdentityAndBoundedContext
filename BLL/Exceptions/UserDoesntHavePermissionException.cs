using System;
using System.Runtime.Serialization;

namespace BLL.Exceptions
{
    [Serializable]
    public class UserDoesntHavePermissionException : Exception
    {
        public UserDoesntHavePermissionException()
        {
        }

        public UserDoesntHavePermissionException(string message) : base(message)
        {
        }

        public UserDoesntHavePermissionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserDoesntHavePermissionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
