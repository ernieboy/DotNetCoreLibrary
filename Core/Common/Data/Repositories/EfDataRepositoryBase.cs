using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Core.Common.Data.Interfaces;
using Core.Common.Extensions;
using LinqKit;
using System.Linq.Dynamic.Core;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Core.Common.Data.Repositories
{
    /// <summary>
    /// An Entity Framework base abastract class which implements the IDataRepository<TEntity/> interface. All repository classes which interact with EF
    ///  should extend this class and customise the already provided functionality, e.g. they should override all virtual methods to suite their needs if required.
    /// </summary>
    /// <typeparam name="TEntity">The actual entity type to save into the repository</typeparam>
    /// <typeparam name="TContext">A class that extends Entity Framework DbContext class.</typeparam>
    public abstract class EfDataRepositoryBase<TEntity, TContext> : IDataRepository<TEntity>
        where TEntity : BaseObjectWithState, IObjectWithState, new()
        where TContext : DbContext, new()
    {
        /// <summary>
        /// A DbContext which interacts with the database. It is typically provided by sibling classes via constructor injection 
        /// </summary>
        protected TContext Context;

        public short QueriesMaxTimeoutInSeconds { get; set; }

        /// <summary>
        /// Saves an entity into the database
        /// </summary>
        /// <param name="entity">True if the entity was saved successfully</param>
        /// <returns></returns>
        public async Task<bool> PersistEntity(TEntity entity)
        {
            entity.DateModified = DateTime.Now;
            AddOrUpdate(entity);
            Context.ApplyStateChanges();
            await Context.SaveChangesAsync();
            entity.ObjectState = ObjectState.Unchanged;
            return true;
        }

        /// <summary>
        /// Finds an entity from the database by Primary Key
        /// </summary>
        /// <param name="id">The id of the entity to search for</param>
        /// <returns>The entity if found, null is returned if not found</returns>
        public async Task<TEntity> FindEntityById(int id)
        {
            return await FindSingleEntityById(id);
        }

        /// <summary>
        /// Finds an entity from the database by predicate
        /// </summary>
        /// <param name="predicate">A delegate to use for searching</param>
        /// <returns>The entity if found, null is returned if not found</returns>
        public virtual async Task<TEntity> FindEntityByPredicate(
            Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Returns an IEnumerable of the given entity type from the database via a predicate search
        /// </summary>
        /// <param name="predicate">A delegate to use for searching</param>
        /// <returns>An IEnumerable fo the the entity type if found, an empty list is returned if not found</returns>
        public virtual async Task<IEnumerable<TEntity>> FindAllEntitiesByPredicate(
            Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.FromResult(Context.Set<TEntity>()
            .Where(predicate).ToList());
        }

        /// <summary>
        /// Checks if a an entity that matches the given PK entityId exists in the database
        /// </summary>
        /// <param name="entityId">The PK id to search for</param>
        /// <returns>True if the entity exists</returns>
        public async Task<bool> EntityExists(int entityId)
        {
            return await SingleEntityExists(entityId);
        }

        /// <summary>
        /// Returns a paged IEnumerable of the given entity type from the database.
        /// </summary>
        /// <param name="pageNumber">The page number to return when paging</param>
        /// <param name="pageSize">The number of items to return per page</param>
        /// <param name="totalRecords">This is an output parameter which returns the total number of items returned by the query</param>
        /// <param name="sortColumn">The sort column - must be provided</param>
        /// <param name="sortDirection">The sort direction - must be provided</param>
        /// <param name="searchPredicate">An optional search predicate</param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAllEntitiesByCriteria(
            int? pageNumber, int? pageSize,
            out int totalRecords, string sortColumn,
            string sortDirection, ExpressionStarter<TEntity> searchPredicate)
        {
            return FindAllByCriteria(
                pageNumber, pageSize, out totalRecords,
                 sortColumn, sortDirection, searchPredicate);
        }

        /// <summary>
        /// Adds or attaches an entity to the DbContext depending on what state it's in. 
        /// Note that the client MUST always tell us what state the object is in by setting the ObjectState property on the entity.
        /// All entities which we deal with MUST inherit from the abstract BaseObjectWithState class and also implement the IObjectWithState
        /// interface for this to work.
        /// </summary>
        /// <param name="entity">The entity to add or attach</param>
        protected virtual void AddOrUpdate(TEntity entity)
        {
            string keyColumnName;
            bool entityIsNewToDb = false;

            //Handle case where the PK colulumn is NOT Id e.g it might be ProductId
            if (CurrentEntityHasIntPropertyWithKeyAttribute(out keyColumnName))
            {
                Type constructedEntityType = entity.GetType();
                PropertyInfo prop = constructedEntityType.GetProperty(keyColumnName);
                object propValue = prop.GetValue(entity);
                long propertyValue = long.Parse(propValue.ToString());
                if (propertyValue == default(int) && entity.ObjectState == ObjectState.Added)
                {
                    entityIsNewToDb = true;
                }
            }
            else
            {
                //Handle case where the PK colulumn is Id 
                if (entity.Id == default(int) && entity.ObjectState == ObjectState.Added)
                {
                    entityIsNewToDb = true;
                }
            }

            if (entityIsNewToDb)
            {
                Context.Add(entity);
            }
            else
            {
                Context.Attach(entity);
            }
        }

        /// <summary>
        /// Finds a single entity by it's Primary Key. 
        /// </summary>
        /// <remarks>
        /// In most situations the Id property of the entity class is the Primary Key. However, in some cases we might use the ClassName+Id as the PK 
        /// e.g. CustomerId - this happens when we do Code First from an existing database by using the scaffold command. The existing database which are
        /// reverse engineering might not be using the Id Property as the PK but it might be using ClassName+Id i.e. ProductId.  
        /// In that case we have to edit each generated class and ensure that each PK property of the classes that were generated from the existing database 
        /// by the scaffold command are decorated with the [Key] attribute. 
        /// In this method we use Reflection to inspect the entity's properties and find out if one of them has the [Key] attribute. 
        /// If so, we use that property as the PK. Otherwise we fall back to using Id as the PK. 
        /// 
        /// TODO: What if the entity has a composite PK where two or more columns make up the PK? Also, what if the PK is specified via EF fluent configuration?
        /// We shall cross that bridge when we get there but this method is virtual, which means that a sibbling class can just override it and handle those situations accordingly.
        /// </remarks>
        /// <param name="id">The Id of the entity we are looking for</param>
        /// <returns>The entity if it exists in the db, otherwise null is returned.</returns>
        protected virtual async Task<TEntity> FindSingleEntityById(int id)
        {
            TEntity toReturn;
            string keyColumnName;
            if (!CurrentEntityHasIntPropertyWithKeyAttribute(out keyColumnName))
            {
                //Filter by the defualt Id column if there isn't any other property of the entity that is marked with the [Key] attribute. 
                toReturn = await Task.FromResult(Context.Set<TEntity>().SingleOrDefault(x => x.Id == id));
            }
            else
            {
                // Use Dynamic Linq to filter by the the column that's decorated with [Key] attribute. 
                // See http://weblogs.asp.net/scottgu/dynamic-linq-part-1-using-the-linq-dynamic-query-library 
                string filter = $"{keyColumnName}  = @0";
                toReturn = await Task.FromResult(Context.Set<TEntity>().FirstOrDefault(filter, id));
            }
            return toReturn;
        }


        /// <summary>
        /// Checks if the current entity has an integer column which is decorated with the [Key] attribute. 
        /// When that is the case we assume that the entity is using that column as the primary key instead if the default Id column 
        /// The property name is then assigned to the out param propertyName if found so that it can be returned back to the calling client.
        /// </summary>
        /// <param name="propertyName">The name of the first property which is marked with the [Key] attribute and is numeric</param>
        /// <returns>True if the property is found.</returns>
        private bool CurrentEntityHasIntPropertyWithKeyAttribute(out string propertyName)
        {
            propertyName = string.Empty;
            var entityType = typeof(TEntity);

            // Find the first property of the entity that is decorated with the [Key] attribute. 
            PropertyInfo keyMember = entityType.GetProperties().FirstOrDefault(p => p.GetCustomAttributes<KeyAttribute>().Any());

            if (keyMember == null) return false;
            //Make sure it's numeric
            if ((keyMember.PropertyType != typeof(long) && keyMember.PropertyType != typeof(int)))
            {
                return false;
            }
            propertyName = keyMember.Name;
            return true;
        }

        protected virtual async Task<bool> SingleEntityExists(int entityId)
        {
            bool result;
            string keyColumnName;
            if (CurrentEntityHasIntPropertyWithKeyAttribute(out keyColumnName))
            {
                string filter = $"{keyColumnName}  = @0";
                result = await Task.FromResult(Context.Set<TEntity>().Any(filter, entityId));
            }
            else
            {
                result = await Task.FromResult(Context.Set<TEntity>().Any(x => x.Id == entityId));
            }
            return result;
        }

        protected virtual async Task<IEnumerable<TEntity>> FindEntities()
        {
            return await Task.FromResult(Context.Set<TEntity>().ToList());
        }

        protected virtual IEnumerable<TEntity> FindAllByCriteria(
            int? pageNumber,
            int? pageSize,
            out int totalRecords,
            string sortColumn,
            string sortDirection,
            ExpressionStarter<TEntity> searchPredicate)
        {
            int pageIndex = pageNumber ?? 1;
            int sizeOfPage = pageSize ?? 10;
            if (pageIndex < 1) pageIndex = 1;
            if (sizeOfPage < 1) sizeOfPage = 5;
            int skipValue = (sizeOfPage * (pageIndex - 1));
            var searchFilter = searchPredicate ?? BuildDefaultSearchFilterPredicate();

            totalRecords =
               Context.Set<TEntity>().AsExpandable().Where(searchFilter)
               .OrderBy($"{sortColumn} {sortDirection}").Count();

            var list =
                Context.Set<TEntity>().AsExpandable()
                    .Where(searchFilter)
                    .OrderBy($"{sortColumn} {sortDirection}")
                    .Skip(skipValue)
                    .Take(sizeOfPage)
                    .ToList();
            return list;
        }

        /// <summary>
        /// Default predicate for when the client did not provide a predicate for searching. 
        /// In that case use this predicated since the search predicate is always required.
        /// This predicate just return TRUE, which means NO filtering.
        /// </summary>
        /// <returns>An expression to use for filtering</returns>
        protected virtual ExpressionStarter<TEntity> BuildDefaultSearchFilterPredicate()
        {
            Expression<Func<TEntity, bool>> filterExpression = a => true;
            ExpressionStarter<TEntity> predicate = PredicateBuilder.New(filterExpression);
            return predicate;
        }

    }
}