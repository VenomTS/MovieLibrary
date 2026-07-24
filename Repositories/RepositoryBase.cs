using Microsoft.EntityFrameworkCore;
using Repositories.Database;
using System.Linq.Expressions;

namespace Repositories
{
    public abstract class RepositoryBase<TEntity>(AppDbContext context) : IRepositoryBase<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> dbSet = context.Set<TEntity>();

        public IQueryable<TEntity> AsQueryable() => dbSet.AsQueryable();

        public async Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = context.Set<TEntity>();
            
            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(x => EF.Property<Guid>(x, "Id") == id);
        }

        public virtual async Task CreateAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public virtual async Task Update(TEntity entity)
        {
            dbSet.Update(entity);
        }

        public virtual async Task Delete(TEntity entity)
        {
            dbSet.Remove(entity);
        }

        public virtual async Task DeleteById(Guid id)
        {
            var entity = await dbSet.FindAsync(id);
            if (entity == null)
                return;

            dbSet.Remove(entity);
        }
        
        public virtual async Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public virtual async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
