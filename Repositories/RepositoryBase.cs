using Microsoft.EntityFrameworkCore;
using Repositories.Database;

namespace Repositories
{
    public abstract class RepositoryBase<TEntity>(AppDbContext context) : IRepositoryBase<TEntity> where TEntity : class
    {

        protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

        public virtual async Task CreateAsync(TEntity entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual async Task Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public virtual async Task Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public virtual async Task DeleteById(Guid id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity == null)
                return;

            DbSet.Remove(entity);
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
        
    }
}
