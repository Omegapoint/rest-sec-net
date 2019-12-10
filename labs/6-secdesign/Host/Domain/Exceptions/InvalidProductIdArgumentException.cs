using System;

namespace SecureByDesign.Host.Domain
{
    public class InvalidProductIdArgumentException : ArgumentException
    {
        public InvalidProductIdArgumentException(string message) : base(message)
        {
        }
    }
}
