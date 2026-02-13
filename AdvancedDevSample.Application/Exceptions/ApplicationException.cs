using System;
using System.Collections.Generic;

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
        public string ResourceType { get; }
        public object ResourceId { get; }

        public NotFoundException(string resourceType, object resourceId)
            : base($"{resourceType} avec l'identifiant '{resourceId}' n'a pas été trouvé(e)")
        {
            ResourceType = resourceType;
            ResourceId = resourceId;
        }

        public NotFoundException(string message) : base(message)
        {
            ResourceType = "Resource";
            ResourceId = string.Empty;
        }
    }

    /// <summary>
    /// Exception pour les erreurs de validation
    /// </summary>
    public class ValidationException : ApplicationException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message, IDictionary<string, string[]> errors)
            : base(message)
        {
            Errors = errors;
        }

        public ValidationException(string field, string errorMessage)
            : base($"Erreur de validation sur le champ '{field}': {errorMessage}")
        {
            Errors = new Dictionary<string, string[]>
            {
                [field] = new[] { errorMessage }
            };
        }
    }

    /// <summary>
    /// Exception pour les conflits de logique métier
    /// </summary>
    public class BusinessRuleException : ApplicationException
    {
        public string RuleName { get; }

        public BusinessRuleException(string ruleName, string message)
            : base(message)
        {
            RuleName = ruleName;
        }

        public BusinessRuleException(string message) : base(message)
        {
            RuleName = "Unknown";
        }
    }

    /// <summary>
    /// Exception pour les opérations non autorisées
    /// </summary>
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string message = "Vous n'êtes pas autorisé à effectuer cette opération")
            : base(message)
        {
        }
    }

    /// <summary>
    /// Exception pour les conflits de données
    /// </summary>
    public class ConflictException : ApplicationException
    {
        public string ConflictReason { get; }

        public ConflictException(string message) : base(message)
        {
            ConflictReason = message;
        }

        public ConflictException(string resource, string conflictReason)
            : base($"Conflit sur '{resource}': {conflictReason}")
        {
            ConflictReason = conflictReason;
        }
    }
}