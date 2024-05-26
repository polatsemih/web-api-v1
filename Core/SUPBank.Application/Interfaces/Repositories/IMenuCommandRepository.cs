using SUPBank.Domain.Entities;

namespace SUPBank.Application.Interfaces.Repositories
{
    public interface IMenuCommandRepository : IGenericCommandRepository<EntityMenu>
    {
        public Task<bool> CreateMenuAsync(EntityMenu menu, CancellationToken cancellationToken);
        public Task<long?> CreateMenuAndGetIdAsync(EntityMenu menu, CancellationToken cancellationToken);
        public Task<int> UpdateMenuAsync(EntityMenu menu, CancellationToken cancellationToken);
        public Task<bool> DeleteMenuAsync(long id, CancellationToken cancellationToken);

        public Task<int> RollbackMenuByIdAsync(long id, CancellationToken cancellationToken);
        public Task<int> RollbackMenuByScreenCodeAsync(int screenCode, CancellationToken cancellationToken);
        public Task<int> RollbackMenusByTokenAsync(Guid rollbackToken, CancellationToken cancellationToken);
    }
}
