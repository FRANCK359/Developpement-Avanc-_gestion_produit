using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedDevSample.Domain.Interfaces;
using AdvancedDevSample.Infrastructure.DbContext;
using AdvancedDevSample.Infrastructure.Repositories;

namespace AdvancedDevSample.Infrastructure.Repositories
{
    /// <summary>
    /// Implémentation de l'Unit of Work pattern
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AdvancedDevSampleDbContext _context;
        private IProductRepository? _productRepository;
        private ICustomerRepository? _customerRepository;
        private ISupplierRepository? _supplierRepository;
        private IOrderRepository? _orderRepository;
        private bool _disposed;

        public UnitOfWork(AdvancedDevSampleDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IProductRepository Products =>
            _productRepository ??= new EfProductRepository(_context);

        public ICustomerRepository Customers =>
            _customerRepository ??= new EfCustomerRepository(_context);

        public ISupplierRepository Suppliers =>
            _supplierRepository ??= new EfSupplierRepository(_context);

        public IOrderRepository Orders =>
            _orderRepository ??= new EfOrderRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            await _context.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.CommitTransactionAsync(await _context.BeginTransactionAsync());
        }

        public async Task RollbackTransactionAsync()
        {
            _context.RollbackTransaction();
            await Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}