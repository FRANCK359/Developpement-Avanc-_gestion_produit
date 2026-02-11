using System;

namespace AdvancedDevSample.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception d'infrastructure
    /// </summary>
    public class InfrastructureException : Exception
    {
        public InfrastructureException(string message) : base(message)
        {
        }

        public InfrastructureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception pour les erreurs de connexion à la base de données
    /// </summary>
    public class DatabaseConnectionException : InfrastructureException
    {
        public DatabaseConnectionException(string message) : base(message)
        {
        }

        public DatabaseConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception pour les erreurs de contrainte de base de données
    /// </summary>
    public class DatabaseConstraintException : InfrastructureException
    {
        public string ConstraintName { get; }

        public DatabaseConstraintException(string message, string constraintName)
            : base(message)
        {
            ConstraintName = constraintName;
        }

        public DatabaseConstraintException(string message, string constraintName, Exception innerException)
            : base(message, innerException)
        {
            ConstraintName = constraintName;
        }
    }
}