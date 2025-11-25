using FashionStore.Data.DBContext;
using FashionStore.Data.Interfaces;
using FashionStore.Data.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionStore.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _context;
        private IDbContextTransaction _transaction;

        private ICartRepository _carts;
        private IProductRepository _products;
        private IGenericRepository<Order> _orders;
        private IGenericRepository<OrderDetail> _orderDetails;

        public UnitOfWork(ApplicationDBContext context)
        {
            _context = context;
        }

        public ICartRepository Carts => _carts ??= new CartRepository(_context);
        public IProductRepository Products => _products ??= new ProductRepository(_context);

        public IGenericRepository<Order> Orders => _orders ??= new GenericRepository<Order>(_context);
        public IGenericRepository<OrderDetail> OrderDetails => _orderDetails ??= new GenericRepository<OrderDetail>(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();  
            }
            catch
            {
                await RollbackTransactionAsync();
                throw; 
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
