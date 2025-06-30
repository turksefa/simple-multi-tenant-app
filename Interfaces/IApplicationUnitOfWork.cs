namespace test.Interfaces
{
    public interface IApplicationUnitOfWork
    {
        ITenantManagementRepository TenantManagementRepository { get; }
        Task<int> SaveChangesAsync();
    }
}