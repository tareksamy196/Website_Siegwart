using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Website.Siegwart.DAL.Data.Contexts;
using Website.Siegwart.DAL.Repositories.Interfaces;

namespace Website.Siegwart.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        private readonly Lazy<ICategoryRepository> _categoryRepository;
        private readonly Lazy<IProductRepository> _productRepository;
        private readonly Lazy<INewsRepository> _newsRepository;
        private readonly Lazy<ITeamMemberRepository> _teamMemberRepository;
        private readonly Lazy<IVideoMediaRepository> _videoMediaRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            _categoryRepository = new Lazy<ICategoryRepository>(() =>
                new CategoryRepository(context));
            _productRepository = new Lazy<IProductRepository>(() =>
                new ProductRepository(context));
            _newsRepository = new Lazy<INewsRepository>(() =>
                new NewsRepository(context));
            _teamMemberRepository = new Lazy<ITeamMemberRepository>(() =>
                new TeamMemberRepository(context));
            _videoMediaRepository = new Lazy<IVideoMediaRepository>(() =>
                new VideoMediaRepository(context));
        }

        public ICategoryRepository CategoryRepository => _categoryRepository.Value;
        public IProductRepository ProductRepository => _productRepository.Value;
        public INewsRepository NewsRepository => _newsRepository.Value;
        public ITeamMemberRepository TeamMemberRepository => _teamMemberRepository.Value;
        public IVideoMediaRepository VideoMediaRepository => _videoMediaRepository.Value;

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException(
                    "An error occurred while saving to the database.", ex);
            }
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction is not null)
                throw new InvalidOperationException(
                    "A transaction is already in progress.");

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("No transaction in progress.");

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
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("No transaction in progress.");

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}