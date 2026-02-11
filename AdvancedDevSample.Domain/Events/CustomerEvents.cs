using System;
using AdvancedDevSample.Domain.Common;

namespace AdvancedDevSample.Domain.Events
{
    /// <summary>
    /// Événement déclenché lors de la création d'un client
    /// </summary>
    public class CustomerCreatedEvent : DomainEvent
    {
        public Guid CustomerId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }

        public CustomerCreatedEvent(AdvancedDevSample.Domain.Entities.Customer customer)
        {
            CustomerId = customer.Id;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Email = customer.Email;
        }
    }

    public class CustomerUpdatedEvent : DomainEvent
    {
        public Guid CustomerId { get; }

        public CustomerUpdatedEvent(Guid customerId)
        {
            CustomerId = customerId;
        }
    }

    public class CustomerActivatedEvent : DomainEvent
    {
        public Guid CustomerId { get; }

        public CustomerActivatedEvent(Guid customerId)
        {
            CustomerId = customerId;
        }
    }

    public class CustomerDeactivatedEvent : DomainEvent
    {
        public Guid CustomerId { get; }

        public CustomerDeactivatedEvent(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
}