using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InBusinessForTests.Data.Repository
{
    public class Repository<T> where T : Entity
    {
        private readonly ApplicationContext _dbContext;

        public Repository(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                var entry = _dbContext.Set<T>()
                    .Add(entity);
                await _dbContext.SaveChangesAsync();
                return entry.Entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException($"{nameof(UpdateAsync)} entity must not be null");
            }

            try
            {
                var attachedEntity = _dbContext.Set<T>()
                    .GetLocalOrAttach(local => local.Id == entity.Id, () => entity);
                _dbContext.Entry(attachedEntity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated: {ex.Message}");
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException($"{nameof(DeleteAsync)} entity must not be null");
            }

            try
            {
                var attachedEntity = _dbContext.Set<T>()
                    .GetLocalOrAttach(local => local.Id == entity.Id, () => entity);
                _dbContext.Entry(attachedEntity).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be deleted: {ex.Message}");
            }
        }

        public async Task<T> GetAsync(int id)
        {
            try
            {
                var entity = await _dbContext.Set<T>()
                    .FirstOrDefaultAsync(e => e.Id == id);
                if (entity is null)
                {
                    throw new ArgumentException($"Entity with ID: {id}, does not exist");
                }

                return entity;
            }
            catch (ArgumentException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(T)} could not be read: {ex.Message}");
            }
        }

        public async Task<IList<T>> GetWhereAndIncludeAsync<TProperty>(Expression<Func<T, bool>> where,
            Expression<Func<T, TProperty>> include)
        {
            try
            {
                var entities = await _dbContext.Set<T>()
                    .Include(include)
                    .Where(where)
                    .ToListAsync();

                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(T)} could not be read: {ex.Message}");
            }
        }

        public async Task<IList<T>> GetWhereAndIncludeThenIncludeAsync<TProperty1, TProperty2>(Expression<Func<T, bool>> where,
            Expression<Func<T, IEnumerable<TProperty1>>> include,
            Expression<Func<TProperty1, TProperty2>> thenInclude)
        {
            try
            {
                var entities = await _dbContext.Set<T>()
                    .Include(include)
                    .ThenInclude(thenInclude)
                    .Where(where)
                    .ToListAsync();

                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(T)} could not be read: {ex.Message}");
            }
        }

        public async Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> where)
        {
            try
            {
                var entities = await _dbContext.Set<T>()
                    .Where(where).ToListAsync();

                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(T)} could not be read: {ex.Message}");
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return _dbContext.Set<T>();
        }

        public async Task<Boolean> UpdateBatchAsync(IEnumerable<T> entities)
        {
            if (entities is null)
            {
                throw new ArgumentNullException($"{nameof(UpdateBatchAsync)} entity must not be null");
            }

            foreach (var entity in entities)
            {
                try
                {
                    var attachedEntity = _dbContext.Set<T>()
                        .GetLocalOrAttach(local => local.Id == entity.Id, () => entity);
                    _dbContext.Entry(attachedEntity).State = EntityState.Modified;
                }
                catch (Exception ex)
                {
                    throw new Exception($"{nameof(entity)} could not be updated: {ex.Message}");
                }
            }

            return (await _dbContext.SaveChangesAsync()) > 0;
        }
    }

    public static class DbSetExtensions
    {
        public static T GetLocalOrAttach<T>(this DbSet<T> collection, Func<T, bool> searchLocalQuery,
            Func<T> getAttachItem) where T : class
        {
            var localEntity = collection.Local.FirstOrDefault(searchLocalQuery);

            if (localEntity != null)
            {
                return localEntity;
            }

            localEntity = getAttachItem();
            collection.Attach(localEntity);

            return localEntity;
        }
    }
}