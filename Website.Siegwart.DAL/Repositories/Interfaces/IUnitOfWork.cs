namespace Website.Siegwart.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        INewsRepository NewsRepository { get; }
        ITeamMemberRepository TeamMemberRepository { get; }
        IVideoMediaRepository VideoMediaRepository { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}