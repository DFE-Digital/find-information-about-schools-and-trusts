
using DfE.FindInformationAcademiesTrusts.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DfE.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories;

namespace DfE.FindInformationAcademiesTrusts.Data.FiatDb.Repositories
{
#pragma warning disable CS8603, S2436

    /// <summary>Constructor</summary>
    /// <param name="dbContext"></param>
    [ExcludeFromCodeCoverage]
    public abstract class Repository<TAggregate, TDbContext>(TDbContext dbContext) : IRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
        where TDbContext : DbContext
    {
        /// <summary>
        /// The <typeparamref name="TDbContext" />
        /// </summary>
        protected readonly TDbContext DbContext = dbContext;

        /// <summary>Shorthand for _dbContext.Set</summary>
        /// <returns></returns>
        protected virtual DbSet<TAggregate> DbSet()
        {
            return DbContext.Set<TAggregate>();
        }

        /// <inheritdoc />
        public virtual IQueryable<TAggregate> Query() => (IQueryable<TAggregate>)DbSet();

        /// <inheritdoc />
        public virtual ICollection<TAggregate> Fetch(Expression<Func<TAggregate, bool>> predicate)
        {
            return (ICollection<TAggregate>)((IQueryable<TAggregate>)DbSet()).Where<TAggregate>(predicate).ToList<TAggregate>();
        }

        /// <inheritdoc />
        public virtual async Task<ICollection<TAggregate>> FetchAsync(
          Expression<Func<TAggregate, bool>> predicate,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            return (ICollection<TAggregate>)await EntityFrameworkQueryableExtensions.ToListAsync<TAggregate>(((IQueryable<TAggregate>)DbSet()).Where<TAggregate>(predicate), cancellationToken);
        }

        /// <inheritdoc />
        public virtual TAggregate Find(params object[] keyValues) => DbSet().Find(keyValues);

        /// <inheritdoc />
        public virtual TAggregate Find(Expression<Func<TAggregate, bool>> predicate)
        {
            return ((IQueryable<TAggregate>)DbSet()).FirstOrDefault<TAggregate>(predicate);
        }

        /// <inheritdoc />
        public virtual async Task<TAggregate> FindAsync(params object[] keyValues)
        {
            return await DbSet().FindAsync(keyValues);
        }

        /// <inheritdoc />
        public virtual async Task<TAggregate> FindAsync(
          Expression<Func<TAggregate, bool>> predicate,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<TAggregate>((IQueryable<TAggregate>)DbSet(), predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual TAggregate Get(Expression<Func<TAggregate, bool>> predicate)
        {
            return ((IQueryable<TAggregate>)DbSet()).Single<TAggregate>(predicate);
        }

        /// <inheritdoc />
        public virtual TAggregate Get(params object[] keyValues)
        {
            return Find(keyValues) ?? throw new InvalidOperationException(
                $"Entity type {(object)typeof(TAggregate)} is null for primary key {(object)keyValues}");
        }

        /// <inheritdoc />
        public virtual async Task<TAggregate> GetAsync(Expression<Func<TAggregate, bool>> predicate)
        {
            return await EntityFrameworkQueryableExtensions.SingleAsync<TAggregate>((IQueryable<TAggregate>)DbSet(), predicate, new CancellationToken());
        }

        /// <inheritdoc />
        public virtual async Task<TAggregate> GetAsync(params object[] keyValues)
        {
            return await FindAsync(keyValues) ?? throw new InvalidOperationException(
                $"Entity type {(object)typeof(TAggregate)} is null for primary key {(object)keyValues}");
        }

        /// <inheritdoc />
        public virtual TAggregate Add(TAggregate entity)
        {
            DbContext.Add<TAggregate>(entity);
            DbContext.SaveChanges();
            return entity;
        }

        /// <inheritdoc />
        public virtual async Task<TAggregate> AddAsync(TAggregate entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await DbContext.AddAsync<TAggregate>(entity, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        /// <inheritdoc />
        public virtual IEnumerable<TAggregate> AddRange(ICollection<TAggregate> entities)
        {
            DbContext.AddRange((IEnumerable<object>)entities);
            DbContext.SaveChanges();
            return (IEnumerable<TAggregate>)entities;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TAggregate>> AddRangeAsync(
          ICollection<TAggregate> entities,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            await DbContext.AddRangeAsync((IEnumerable<object>)entities, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);
            return (IEnumerable<TAggregate>)entities;
        }

        /// <inheritdoc />
        public virtual TAggregate Remove(TAggregate entity)
        {
            DbContext.Remove<TAggregate>(entity);
            DbContext.SaveChanges();
            return entity;
        }

        /// <inheritdoc />
        public virtual async Task<TAggregate> RemoveAsync(
          TAggregate entity,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            DbContext.Remove<TAggregate>(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        /// <inheritdoc />
        public virtual int Delete(Expression<Func<TAggregate, bool>> predicate)
        {
            return DbSet().Where(predicate).ExecuteDelete();
        }

        /// <inheritdoc />
        public virtual IEnumerable<TAggregate> RemoveRange(ICollection<TAggregate> entities)
        {
            DbSet().RemoveRange((IEnumerable<TAggregate>)entities);
            DbContext.SaveChanges();
            return (IEnumerable<TAggregate>)entities;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TAggregate>> RemoveRangeAsync(
          ICollection<TAggregate> entities,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            DbSet().RemoveRange((IEnumerable<TAggregate>)entities);
            await DbContext.SaveChangesAsync(cancellationToken);
            return (IEnumerable<TAggregate>)entities;
        }

        /// <inheritdoc />
        public virtual TAggregate Update(TAggregate entity)
        {
            DbContext.Update<TAggregate>(entity);
            DbContext.SaveChanges();
            return entity;
        }

        /// <inheritdoc />
        public virtual async Task<TAggregate> UpdateAsync(
          TAggregate entity,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            DbContext.Update<TAggregate>(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }
    }
#pragma warning restore CS8603, S2436
}
