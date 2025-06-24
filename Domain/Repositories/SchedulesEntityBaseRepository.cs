﻿using System.Linq.Expressions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;
using SchedulingModule.Domain.Abstract;
using SchedulingModule.Domain.Context;
using SchedulingModule.Domain.Interfaces;

namespace SchedulingModule.Domain.Repositories
{

    public class GenericRepository<T> : IDisposable
    {
        public NpgsqlConnection Conn { get; private set; }
        private readonly object _lock = new object();

        public GenericRepository(IConfiguration configuration)
        {
            Conn = new NpgsqlConnection(configuration.GetConnectionString("analytic"));
            Conn.Open();
        }

        public IEnumerable<J> FromRawSQL<J>(string query)
            where J : class
        {
            lock (_lock)
            {
                return Conn.Query<J>(query);
            }
        }

        public IEnumerable<T> FromRawSQL(string query)
        {
            lock (_lock)
            {
                return Conn.Query<T>(query);
            }
        }

        public void Dispose()
        {
            Conn.Close();
        }
    }

    internal class SchedulesEntityBaseRepository<T> : ISchedulesEntityBaseRepository<T>
        where T : class, ISchedulesEntityBase, new()
    {
        private object thisLock = new object();
        private bool disposed = false;
        protected SchedulingModuleDbContext _context;

        #region Properties
        public SchedulesEntityBaseRepository(SchedulingModuleDbContext context)
        {
            _context = context;
        }
        #endregion

        /// <summary>
        /// For Adding an entry to the table after this Call Commit
        /// </summary>
        /// <param name="entity"></param>
        ///



        public virtual void Add(T entity)
        {
            lock (thisLock)
            {
                EntityEntry dbEntityEntry = _context.Entry<T>(entity);
                _context.Set<T>().Add(entity);
                _context.SaveChanges();
                this.DetachEntity(entity);
            }
        }

        /// <summary>
        /// For Adding an entry to the table after this Call Commit Asynchronously
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async virtual System.Threading.Tasks.Task AddAsync(T entity)
        {
            lock (thisLock)
            {
                EntityEntry dbEntityEntry = _context.Entry<T>(entity);
                var result = _context.Set<T>().AddAsync(entity).Result;
                _context.SaveChanges();
                this.DetachEntity(entity);
            }
        }

        public virtual async Task AddRange(List<T> entities)
        {
            lock (thisLock)
            {
                _context.Set<T>().AddRangeAsync(entities).Wait();
                _context.SaveChanges();
                foreach (var entity in entities)
                {
                    this.DetachEntity(entity);
                }
            }
        }

        /// <summary>
        /// Return total number of entry
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            lock (thisLock)
            {
                return _context.Set<T>().Count();
            }
        }

        /// <summary>
        /// Return total number of entry asynchronous
        /// </summary>
        /// <returns></returns>
        public async virtual Task<int> CountAsync()
        {
            lock (thisLock)
            {
                return _context.Set<T>().CountAsync().Result;
            }
        }

        public int CountWhere(Expression<Func<T, bool>> predicate)
        {
            lock (thisLock)
            {
                return _context.Set<T>().Count(predicate);
            }
        }

        /// <summary>
        /// Delete this entry from the database call commit after this
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Delete(T entity)
        {
            lock (thisLock)
            {
                _context.Remove<T>(entity);
                _context.SaveChanges();
            }
        }

        public virtual void DeleteRange(List<T> entities)
        {
            lock (thisLock)
            {
                _context.Set<T>().RemoveRange(entities);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Delete all the entities satisfying this condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual List<T> DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            lock (thisLock)
            {
                IEnumerable<T> entities = _context.Set<T>().Where(predicate);
                foreach (var entity in entities)
                {
                    _context.Set<T>().Remove(entity);
                }
                _context.SaveChanges();
                return entities.ToList();
            }
        }

        /// <summary>
        /// Return the entry corresponding to the key
        /// </summary>
        /// <param name="id">primary key value of the table</param>
        /// <returns></returns>
        public virtual T Get(Guid id)
        {
            lock (thisLock)
            {
                return _context.Set<T>().AsNoTracking().FirstOrDefault(x => x.Id == id);
            }
        }

        /// <summary>
        /// Return the entry corresponding to the key asynchronously.
        /// </summary>
        /// <param name="id">primary key value of the table</param>
        /// <returns></returns>
        public async virtual Task<T> GetAsync(Guid id)
        {
            lock (thisLock)
            {
                return _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id).Result;
            }
        }

        /// <summary>
        /// Return all the entry from the database.
        /// </summary>
    /// <returns></returns>
        public virtual List<T> GetAll()
        {
            lock (thisLock)
            {
                return _context.Set<T>().AsNoTracking().ToList();
            }
        }

        /// <summary>
        /// Return all the entry from the database asynchronously
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAsync()
        {
            lock (thisLock)
            {
                return _context.Set<T>().AsNoTracking().ToListAsync().Result;
            }
        }

        /// <summary>
        /// Include navigation properties of the entity.
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllIncludingAsync(
            params Expression<Func<T, object>>[] includeProperties
        )
        {
            lock (thisLock)
            {
                IQueryable<T> query = _context.Set<T>().AsNoTracking();
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
                return query.ToListAsync().Result;
            }
        }

        /// <summary>
        /// Return single entry from the database satisfying this condition
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual T Find(Expression<Func<T, bool>> match)
        {
            lock (thisLock)
            {
                return _context.Set<T>().AsNoTracking().FirstOrDefault(match);
            }
        }

        /// <summary>
        /// Return single entry from the database satisfying this condition asyncronously
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public async virtual Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            lock (thisLock)
            {
                return _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(match).Result;
            }
        }

        /// <summary>
        /// Return single entry from the database including its navigation property
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual async Task<T> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties
        )
        {
            lock (thisLock)
            {
                IQueryable<T> query = _context.Set<T>();
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return query.Where(predicate).AsNoTracking().FirstOrDefaultAsync().Result;
            }
        }

        public virtual T Find(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties
        )
        {
            lock (thisLock)
            {
                IQueryable<T> query = _context.Set<T>();
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return query.Where(predicate).AsNoTracking().FirstOrDefault();
            }
        }

        /// <summary>
        /// Return collection of the entry which satisfies this condition
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual List<T> FindAll(Expression<Func<T, bool>> match)
        {
            lock (thisLock)
            {
                return _context.Set<T>().Where(match).AsNoTracking().ToList();
            }
        }

        /// <summary>
        /// Return collection of the entry which satisfies this condition asynchronously
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            lock (thisLock)
            {
                return _context.Set<T>().Where(match).AsNoTracking().ToListAsync().Result;
            }
        }

        /// <summary>
        /// Return collection of the entry which satisfies this condition asynchronously
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> FindAllAsync(
            Expression<Func<T, bool>> match,
            params Expression<Func<T, object>>[] includeProperties
        )
        {
            lock (thisLock)
            {
                IQueryable<T> query = _context.Set<T>();
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return query.Where(match).AsNoTracking().ToListAsync().Result;
            }
        }

        public virtual async Task<List<T>> FindAllAsync(
            Expression<Func<T, bool>> match,
            List<Expression<Func<T, object>>> includeProperties
        )
        {
            lock (thisLock)
            {
                IQueryable<T> query = _context.Set<T>();
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return query.Where(match).AsNoTracking().ToListAsync().Result;
            }
        }

        /// <summary>
        /// Return collection of the entry which satisfies this condition
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public virtual List<T> FindAll(
            Expression<Func<T, bool>> match,
            params Expression<Func<T, object>>[] includeProperties
        )
        {
            lock (thisLock)
            {
                IQueryable<T> query = _context.Set<T>();
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }

                return query.Where(match).AsNoTracking().ToList();
            }
        }

        public virtual T GetWithThenInclude(
            Expression<Func<T, bool>> match,
            Func<IQueryable<T>, IQueryable<T>> func
        )
        {
            lock (thisLock)
            {
                DbSet<T> result = _context.Set<T>();

                IQueryable<T> resultWithEagerLoading = func(result).AsNoTracking();

                return resultWithEagerLoading.FirstOrDefault(match);
            }
        }

        public virtual IEnumerable<T> GetAllWithThenInclude(
            Expression<Func<T, bool>> match,
            Func<IQueryable<T>, IQueryable<T>> func
        )
        {
            lock (thisLock)
            {
                DbSet<T> result = _context.Set<T>();

                IQueryable<T> resultWithEagerLoading = func(result).AsNoTracking();

                return resultWithEagerLoading.Where(match);
            }
        }

        public virtual IEnumerable<T> GetAllWithThenIncludeNoCondition(
            Func<IQueryable<T>, IQueryable<T>> func
        )
        {
            lock (thisLock)
            {
                DbSet<T> result = _context.Set<T>();
                IQueryable<T> resultWithEagerLoading = func(result).AsNoTracking();
                return resultWithEagerLoading;
            }
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(T entity)
        {
            lock (thisLock)
            {
                _context.Set<T>().Update(entity);
                _context.SaveChanges();
                this.DetachEntity(entity);
            }
        }

        /// <summary>
        /// Update multiple enteries
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateRange(List<T> entities)
        {
            lock (thisLock)
            {
                _context.Set<T>().UpdateRange(entities);
                _context.SaveChanges();

                entities.ForEach(x => this.DetachEntity(x));
            }
        }

        //Update selected entries from resource using view

        protected virtual void Dispose(bool disposing)
        {
            lock (thisLock)
            {
                if (!this.disposed)
                {
                    if (disposing)
                    {
                        _context.Dispose();
                    }
                    this.disposed = true;
                }
            }
        }

        public void Dispose()
        {
            lock (thisLock)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        public void DetachEntity(T entity)
        {
            lock (thisLock)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }
        }

        public List<T> GetAllWithSkipTakeThenInclude(
            int skip,
            int pageLimit,
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties
        )
        {
            lock (thisLock)
            {
                IQueryable<T> query = _context.Set<T>();
                query = query.Where(predicate).OrderBy(x => x.Id).Skip(skip).Take(pageLimit);
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
                return query.ToList();
            }
        }
    }
}
