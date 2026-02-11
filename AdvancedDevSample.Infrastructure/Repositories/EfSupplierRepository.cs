using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedDevSample.Domain.Entities;
using AdvancedDevSample.Domain.Interfaces;
using AdvancedDevSample.Infrastructure.DbContext;

namespace AdvancedDevSample.Infrastructure.Repositories
{
    /// <summary>
    /// Implémentation EF Core du repository de fournisseurs
    /// </summary>
    public class EfSupplierRepository : ISupplierRepository
    {
        private readonly AdvancedDevSampleDbContext _context;

        public EfSupplierRepository(AdvancedDevSampleDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Supplier?> GetByIdAsync(Guid id)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Supplier?> GetByNameAsync(string name)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync()
        {
            return await _context.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Supplier> AddAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
            return supplier;
        }

        public Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var supplier = await GetByIdAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Suppliers.AnyAsync(s => s.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Suppliers.AnyAsync(s => s.Name == name);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}