using System;

namespace SecureByDesign.Host.Domain
{
    public class InvalidSearchTermArgumentException : ArgumentException
    {
        public InvalidSearchTermArgumentException(string message) : base(message)
        {
        }
    }
}
