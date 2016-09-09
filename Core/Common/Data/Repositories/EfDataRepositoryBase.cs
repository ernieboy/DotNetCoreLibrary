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

namespace Core.Common.Data.Repositories
{
    /// <summary>
    /// An Entity Framework base abastract class which implements the IDataRepository interface. All repository classes which interact with EF
    ///  should extend this class and customise the already provided functionality, e.g. they should override all virtual methods to suite their needs.
    /// </summary>
    /// <typeparam name="TEntity">The actual entity type to save into the repository</typeparam>
    /// <typeparam name="TContext">A class that extends Entity Framework DbContext class.</typeparam>
    public abstract class EfDataRepositoryBase<TEntity, TContext> : IDataRepository<TEntity>
        where TEntity : BaseObjectWithState, IObjectWithState, new()
        where TContext : DbContext, new()
    {
        protected TContext Context;

        public short QueriesMaxTimeoutInSeconds { get; set; }


        public async Task<bool> PersistEntity(TEntity entity)
        {
            entity.DateModified = DateTime.Now;
            AddOrUpdate(entity);
            Context.ApplyStateChanges();
            await Context.SaveChangesAsync();
            entity.ObjectState = ObjectState.Unchanged;
            return true;
        }

        public async Task<TEntity> FindEntityById(int id)
        {
            return await FindSingleEntityById(id);
        }

        public virtual async Task<TEntity> FindEntityByPredicate(
            Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllEntitiesByPredicate(
            Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.FromResult(Context.Set<TEntity>()
            .Where(predicate).ToList());
        }

        public async Task<bool> EntityExists(int entityId)
        {
            return await SingleEntityExists(entityId);
        }

        public async Task<IEnumerable<TEntity>> FindAllEntities()
        {
            return await Task.FromResult(Context.Set<TEntity>().ToList());
        }

        public IEnumerable<TEntity> FindAllEntitiesByCriteria(
            int? pageNumber, int? pageSize,
            out int totalRecords, string sortColumn,
            string sortDirection, ExpressionStarter<TEntity> searchPredicate)
        {
            return FindAllByCriteria(
                pageNumber, pageSize, out totalRecords,
                 sortColumn, sortDirection, searchPredicate);
        }


        ///Note, if the PK of the entity you are persisting is not called Id then override this
        // method in your own derived repository
        protected virtual void AddOrUpdate(TEntity entity)
        {
            if (entity.Id == default(int) && entity.ObjectState == ObjectState.Added)
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
        /// In most situations the Id property of the entity class is the primary key. However, in some cases we might use the ClassName+Id as the PK 
        /// e.g. CustomerId. In that case we have to use Reflection to inspect the entity's properties and find out if one of them has the [Key] attribute. If
        /// so, we use that property as the PK. Otherwise we fall back to using Id as the PK. 
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>The entity if it exists in the db, otherwise null is returned.</returns>
        protected virtual async Task<TEntity> FindSingleEntityById(int id)
        {
            TEntity toReturn;
            var entityType = typeof(TEntity);
            // Find the first property of the entity that is decorated with the [Key] attribute. 
            // If there is one we assume it's being used instead of the defualt Id property
            PropertyInfo keyMember = entityType.GetProperties().FirstOrDefault(p => p.GetCustomAttributes<KeyAttribute>().Any());
            if (keyMember == null || (keyMember.PropertyType != typeof(long) && keyMember.PropertyType != typeof(int)))
            {
                //Filter by the defualt Id column if there isn't any other property of the entity that is marked with the [Key] attribute. 
                toReturn =  await Task.FromResult(Context.Set<TEntity>().SingleOrDefault(x => x.Id == id));
            }
            else
            {
                // Use Dynamic Linq to filter by the the column that's decorated with [Key] attribute. 
                // See http://weblogs.asp.net/scottgu/dynamic-linq-part-1-using-the-linq-dynamic-query-library 
                string filter = $"{keyMember.Name}  = @0";
                toReturn =  await Task.FromResult(Context.Set<TEntity>().FirstOrDefault(filter, id));
            }
            return toReturn;
        }


        protected virtual async Task<bool> SingleEntityExists(int entityId)
        {
            return await Task.FromResult(Context.Set<TEntity>()
            .Any(x => x.Id == entityId));
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