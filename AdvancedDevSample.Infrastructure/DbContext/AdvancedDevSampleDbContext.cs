using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Reflection;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Common;

namespace AdvancedDevSample.Infrastructure.DbContext
{
    /// <summary>
    /// Contexte de base de données pour l'application
    /// </summary>
    public class AdvancedDevSampleDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        private IDbContextTransaction? _currentTransaction;

        public AdvancedDevSampleDbContext(DbContextOptions<AdvancedDevSampleDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration pour Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.IsActive);

                entity.HasOne<Supplier>()
                    .WithMany()
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Ignore(e => e.DomainEvents);
            });

            // Configuration pour Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasIndex(e => e.Email).IsUnique();
                entity.Ignore(e => e.DomainEvents);
            });

            // Configuration pour Supplier
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ContactEmail).IsRequired().HasMaxLength(200);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasIndex(e => e.Name).IsUnique();
                entity.Ignore(e => e.DomainEvents);
            });

            // Configuration pour Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderDate).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasConversion<int>();
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne<Customer>()
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.OwnsMany(e => e.OrderItems, oi =>
                {
                    oi.WithOwner().HasForeignKey("OrderId");
                    oi.Property<Guid>("Id");
                    oi.HasKey("Id");
                    oi.Property(e => e.ProductId).IsRequired();
                    oi.Property(e => e.Quantity).IsRequired();
                    oi.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                    oi.HasIndex(e => e.ProductId);
                });

                entity.Navigation(e => e.OrderItems).UsePropertyAccessMode(PropertyAccessMode.Field);
                entity.Ignore(e => e.DomainEvents);
            });

            // Configuration pour User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.LastLoginAt);

                entity.HasIndex(e => e.Email).IsUnique();
                entity.Ignore(e => e.DomainEvents);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents!)
                .ToList();

            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            var result = await base.SaveChangesAsync(cancellationToken);

            // Optionnel : dispatcher les événements ici
            // await DispatchEventsAsync(domainEvents);

            return result;
        }

        public async Task<IDbContextTransaction?> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction? transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}