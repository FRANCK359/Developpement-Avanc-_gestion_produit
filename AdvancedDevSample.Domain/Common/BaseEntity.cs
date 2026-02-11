using System;
using System.Collections.Generic;

namespace AdvancedDevSample.Domain.Common
{
    /// <summary>
    /// Classe de base pour toutes les entités du domaine
    /// </summary>
    public abstract class BaseEntity
    {
        private readonly List<DomainEvent> _domainEvents = new();

        public Guid Id { get; protected set; }

        /// <summary>
        /// Événements de domaine
        /// </summary>
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Ajoute un événement de domaine
        /// </summary>
        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Supprime tous les événements de domaine
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }

    /// <summary>
    /// Classe de base pour les événements de domaine
    /// </summary>
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; }

        protected DomainEvent()
        {
            OccurredOn = DateTime.UtcNow;
        }
    }
}