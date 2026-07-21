using Microsoft.EntityFrameworkCore;
using Repositories.Databasee;

namespace Repositories
{
    public abstract class RepositoryBase<TEntity> where TEntity : class
    {

        protected readonly AppDbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        protected RepositoryBase(AppDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

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
        
    }
}
