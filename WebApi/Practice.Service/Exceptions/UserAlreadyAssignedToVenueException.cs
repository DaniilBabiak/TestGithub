using System;
using System.Globalization;

namespace Practice.Service.Exceptions
{
    public class UserAlreadyAssignedToVenueException : Exception
    {
        public UserAlreadyAssignedToVenueException() : base() { }

        public UserAlreadyAssignedToVenueException(string message) : base(message) { }

        public UserAlreadyAssignedToVenueException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
