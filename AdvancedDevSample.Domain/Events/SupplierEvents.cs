using System;
using AdvancedDevSample.Domain.Common;

namespace AdvancedDevSample.Domain.Events
{
    /// <summary>
    /// Événement déclenché lors de la création d'un fournisseur
    /// </summary>
    public class SupplierCreatedEvent : DomainEvent
    {
        public Guid SupplierId { get; }
        public string Name { get; }
        public string ContactEmail { get; }

        public SupplierCreatedEvent(AdvancedDevSample.Domain.Entities.Supplier supplier)
        {
            SupplierId = supplier.Id;
            Name = supplier.Name;
            ContactEmail = supplier.ContactEmail;
        }
    }

    public class SupplierUpdatedEvent : DomainEvent
    {
        public Guid SupplierId { get; }

        public SupplierUpdatedEvent(Guid supplierId)
        {
            SupplierId = supplierId;
        }
    }

    public class SupplierActivatedEvent : DomainEvent
    {
        public Guid SupplierId { get; }

        public SupplierActivatedEvent(Guid supplierId)
        {
            SupplierId = supplierId;
        }
    }

    public class SupplierDeactivatedEvent : DomainEvent
    {
        public Guid SupplierId { get; }

        public SupplierDeactivatedEvent(Guid supplierId)
        {
            SupplierId = supplierId;
        }
    }
}