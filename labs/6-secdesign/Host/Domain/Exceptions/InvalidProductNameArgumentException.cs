using System;

namespace SecureByDesign.Host.Domain
{
    public class InvalidProductNameArgumentException : ArgumentException
    {
        public InvalidProductNameArgumentException(string message) : base(message)
        {
        }
    }
}
