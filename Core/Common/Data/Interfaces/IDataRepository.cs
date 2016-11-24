using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Common.Data.Models;
using LinqKit;


// ReSharper disable once CheckNamespace
namespace Core.Common.Data.Interfaces
{
    public interface IDataRepository
    {
    }

    public interface IDataRepository<TEntity> : IDataRepository
        where TEntity : BaseObjectWithState, IObjectWithState, new()
    {
        /// <summary>
        /// Attempts to save an entity into the database and returns a boolean indicating success or failure.
        /// </summary>
        /// <param name="entity">The entity to save into the database.</param>
        /// <returns>True if persistence succeeded, throws Exception otherwise </returns>
        Task<bool> PersistEntity(TEntity entity);

        /// <summary>
        /// Finds an entity by its id.
        /// </summary>
        /// <param name="id">The ID of the entity to search for.</param>
        /// <returns>The entity if found, it's up to the implementer what to return if the entity was not found. They can throw a NotFoundException if needed.</returns>
        Task<TEntity> FindEntityById(long id);

        /// <summary>
        /// Finds an entity by its id. In this case the ID is a string
        /// </summary>
        /// <param name="id">The ID of the entity to search for.</param>
        /// <returns>The entity if found, it's up to the implementer what to return if the entity was not found. They can throw a NotFoundException if needed.</returns>
        Task<TEntity> FindEntityByStringId(long id);    
    

        /// <summary>
        /// Checks if an entity exists in the repository by its ID.
        /// </summary>
        /// <param name="entityId">The ID of the entity to search for.</param>
        /// <returns>True if the entity was found, false otherwise.</returns>
        Task<bool> EntityExists(int entityId);

        /// <summary>
        /// Returns entity of type TEntity from the repository based on predicate. 
        /// </summary>
        /// <returns>The entity if found.</returns>
        Task<TEntity> FindEntityByPredicate(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Returns all entities of type TEntity from the repository based on predicate. 
        /// </summary>
        /// <returns>A collection of TEntity entities if found.</returns>
        Task<IEnumerable<TEntity>> FindAllEntitiesByPredicate(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        ///  Returns a collection of TEntity entity in chunks. The client can also specify search and paging requirements criteria.
        /// </summary>
        /// <param name="pageNumber">The page number to return  when paging.</param>
        /// <param name="pageSize">The page size when paging.</param>
        /// <param name="totalRecords">The total number of records which were returned by the query criteria. Needed for displaying paging navigation links.</param>
        /// <param name="sortColumn">The sort column in the underlying data store. Should be indexed or performance could be severly reduced.</param>
        /// <param name="sortDirection">Either ASC or DESC.</param>
        /// <param name="searchFilter">A predicate to search by</param>
        /// <returns>A collection of TEntity entities.</returns>
        IEnumerable<TEntity> FindAllEntitiesByCriteria(
            int? pageNumber,
            int? pageSize,
            out int totalRecords,
            string sortColumn,
            string sortDirection,
            ExpressionStarter<TEntity> searchFilter);
    }
}