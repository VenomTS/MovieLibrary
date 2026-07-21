using Microsoft.EntityFrameworkCore;
using Repositories.Database;
using System.Linq.Expressions;

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

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = DbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        /*public virtual async Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = DbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(x => EF.Property<Guid>(x, "Id") == id);
        }*/

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
