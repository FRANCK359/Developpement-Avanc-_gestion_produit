using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedDevSample.Domain.Exceptions
{
    /// <summary>
    /// Exception de domaine
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}