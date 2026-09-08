using Dfe.FindInformationAcademiesTrusts.Domain;
using System.Linq.Expressions;
using DfE.FindInformationAcademiesTrusts.Domain.Common;

namespace Dfe.FindInformationAcademiesTrusts.Domain.Interfaces.Repositories
{
    /// <summary>Repository</summary>
    /// <typeparam name="TAggregate"></typeparam>
    public interface IRepository<TAggregate>
        where TAggregate : IAggregateRoot
    {
        /// <summary>Returns a queryable (un-resolved!!!!) list of objects.</summary>
        /// <returns>Do not expose IQueryable outside of the domain layer</returns>
        IQueryable<TAggregate> Query();

        /// <summary>
        /// Returns an enumerated (resolved!) list of objects based on known query predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        /// <remarks>We know that its the same as doing IQueryable`T.Where(p=&gt; p.value=x).Enumerate but this limits the repo to only ever returning resolved lists.</remarks>
        ICollection<TAggregate> Fetch(Expression<Func<TAggregate, bool>> predicate);

        /// <summary>
        /// Asynchronously returns an enumerated (resolved!) list of objects based on known query predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>We know that its the same as doing IQueryable`T.Where(p=&gt; p.value=x).Enumerate but this limits the repo to only ever returning resolved lists.</remarks>
        Task<ICollection<TAggregate>> FetchAsync(
          Expression<Func<TAggregate, bool>> predicate,
          CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Finds an entity with the given primary key value.  If an entity with the
        /// given primary key values exists in the context, then it is returned immediately
        /// without making a request to the store. Otherwise, a request is made to the
        /// store for an entity with the given primary key value and this entity, if
        /// found, is attached to the context and returned. If no entity is found in
        /// the context or the store, then null is returned.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns>The entity found, or null.</returns>
        TAggregate Find(params object[] keyValues);

        /// <summary>
        /// Returns the first entity of a sequence that satisfies a specified condition
        /// or a default value if no such entity is found.
        /// </summary>
        /// <param name="predicate">A function to test an entity for a condition</param>
        /// <returns>The entity found, or null</returns>
        TAggregate Find(Expression<Func<TAggregate, bool>> predicate);

        /// <summary>
        /// Asynchronously finds an entity with the given primary key value.  If an entity with the
        /// given primary key values exists in the context, then it is returned immediately
        /// without making a request to the store. Otherwise, a request is made to the
        /// store for an entity with the given primary key value and this entity, if
        /// found, is attached to the context and returned. If no entity is found in
        /// the context or the store, then null is returned.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns>The entity found, or null.</returns>
        Task<TAggregate> FindAsync(params object[] keyValues);

        /// <summary>
        /// Asynchronously returns the first entity of a sequence that satisfies a specified condition
        /// or a default value if no such entity is found.
        /// </summary>
        /// <param name="predicate">A function to test an entity for a condition</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The entity found, or null</returns>
        Task<TAggregate> FindAsync(
          Expression<Func<TAggregate, bool>> predicate,
          CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets an entity with the given primary key value.  If an entity with the
        /// given primary key values exists in the context, then it is returned immediately
        /// without making a request to the store. Otherwise, a request is made to the
        /// store for an entity with the given primary key value and this entity, if
        /// found, is attached to the context and returned. If no entity is found in
        /// the context or the store -or- more than one entity is found, then an
        /// InvalidOperationException is thrown.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns>The entity found</returns>
        /// <exception cref="T:System.InvalidOperationException">If no entity is found in the context or the store -or- more than one entity is found, then an</exception>
        TAggregate Get(params object[] keyValues);

        /// <summary>
        /// Gets an entity that satisfies a specified condition,
        /// and throws an exception if more than one such element exists.
        /// </summary>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <exception cref="T:System.InvalidOperationException">
        /// No entity satisfies the condition in predicate. -or- More than one entity satisfies the condition in predicate. -or- The source sequence is empty.
        /// </exception>
        TAggregate Get(Expression<Func<TAggregate, bool>> predicate);

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value.  If an entity with the
        /// given primary key values exists in the context, then it is returned immediately
        /// without making a request to the store. Otherwise, a request is made to the
        /// store for an entity with the given primary key value and this entity, if
        /// found, is attached to the context and returned. If no entity is found in
        /// the context or the store -or- more than one entity is found, then an
        /// InvalidOperationException is thrown.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns>The entity found</returns>
        /// <exception cref="T:System.InvalidOperationException">If no entity is found in the context or the store -or- more than one entity is found, then an</exception>
        Task<TAggregate> GetAsync(params object[] keyValues);

        /// <summary>
        /// Asynchronously gets an entity that satisfies a specified condition,
        /// and throws an exception if more than one such element exists.
        /// </summary>
        /// <param name="predicate">A function to test an element for a condition.</param>
        /// <exception cref="T:System.InvalidOperationException">
        /// No entity satisfies the condition in predicate. -or- More than one entity satisfies the condition in predicate. -or- The source sequence is empty.
        /// </exception>
        Task<TAggregate> GetAsync(Expression<Func<TAggregate, bool>> predicate);

        /// <summary>
        ///  Adds the given entity to the context underlying the set in the Added state
        ///  such that it will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <returns>The entity</returns>
        /// <remarks>Note that entities that are already in the context in some other state will
        /// have their state set to Added. Add is a no-op if the entity is already in
        /// the context in the Added state.</remarks>
        TAggregate Add(TAggregate entity);

        /// <summary>
        ///  Asynchronously adds the given entity to the context underlying the set in the Added state
        ///  such that it will be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The entity</returns>
        /// <remarks>Note that entities that are already in the context in some other state will
        /// have their state set to Added. Add is a no-op if the entity is already in
        /// the context in the Added state.</remarks>
        Task<TAggregate> AddAsync(TAggregate entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        IEnumerable<TAggregate> AddRange(ICollection<TAggregate> entities);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<TAggregate>> AddRangeAsync(
          ICollection<TAggregate> entities,
          CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Marks the given entity as Deleted such that it will be deleted from the database
        ///     when SaveChanges is called. Note that the entity must exist in the context
        ///     in some other state before this method is called.
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        /// <returns>The entity</returns>
        /// <remarks>Note that if the entity exists in the context in the Added state, then this
        /// method will cause it to be detached from the context. This is because an
        /// Added entity is assumed not to exist in the database such that trying to
        /// delete it does not make sense.</remarks>
        TAggregate Remove(TAggregate entity);

        /// <summary>
        ///     Asynchronously marks the given entity as Deleted such that it will be deleted from the database
        ///     when SaveChanges is called. Note that the entity must exist in the context
        ///     in some other state before this method is called.
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The entity</returns>
        /// <remarks>Note that if the entity exists in the context in the Added state, then this
        /// method will cause it to be detached from the context. This is because an
        /// Added entity is assumed not to exist in the database such that trying to
        /// delete it does not make sense.</remarks>
        Task<TAggregate> RemoveAsync(TAggregate entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Executes a delete statement filtering the rows to be deleted.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of row deleted.</returns>
        /// <remarks>
        /// When executing this method, the statement is immediately executed on the
        /// database provider and is not part of the change tracking system. Also, changes
        /// will not be reflected on any entities that have already been materialized
        /// in the current contex
        /// </remarks>
        int Delete(Expression<Func<TAggregate, bool>> predicate);

        /// <summary>
        /// Removes the given collection of entities from the DbContext
        /// </summary>
        /// <param name="entities">The collection of entities to remove.</param>
        IEnumerable<TAggregate> RemoveRange(ICollection<TAggregate> entities);

        /// <summary>
        /// Asynchronously removes the given collection of entities from the DbContext
        /// </summary>
        /// <param name="entities">The collection of entities to remove.</param>
        /// <param name="cancellationToken"></param>
        Task<IEnumerable<TAggregate>> RemoveRangeAsync(
          ICollection<TAggregate> entities,
          CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the given entity in the DbContext and executes SaveChanges()
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        TAggregate Update(TAggregate entity);

        /// <summary>
        /// Asynchronously updates the given entity in the DbContext and executes SaveChanges()
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken"></param>
        Task<TAggregate> UpdateAsync(TAggregate entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}
