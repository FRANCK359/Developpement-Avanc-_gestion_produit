using System;

namespace AdvancedDevSample.Application.Exceptions
{
    /// <summary>
    /// Exception de base pour les exceptions d'application
    /// </summary>
    public abstract class ApplicationException : Exception
    {
        protected ApplicationException(string message) : base(message)
        {
        }

        protected ApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception pour les ressources non trouvées
    /// </summary>
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Exception pour les erreurs de validation
    /// </summary>
    public class ValidationException : ApplicationException
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
}