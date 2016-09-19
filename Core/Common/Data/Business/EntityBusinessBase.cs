using System;
using System.Collections.Generic;
using Core.Common.Data.Models;
using Core.Common.Data.Interfaces;
using Core.Common.Utilities;
using Core.Common.Extensions;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqKit;

namespace Core.Common.Data.Business
{
    public abstract class EntityBusinessBase<TEntity>
        where TEntity : BaseObjectWithState, IObjectWithState, new()
    {
        protected readonly IDataRepository<TEntity> Repository;

        protected EntityBusinessBase(IDataRepository<TEntity> repository)
        {
            Repository = repository;
        }
        public virtual async Task<TEntity> FindEntityById(int id)
        {
            var entity = await Repository.FindEntityById(id);
            return entity;
        }

        public virtual async Task<bool> PersistEntity(TEntity entity)
        {
            return await Repository.PersistEntity(entity);
        }

        protected virtual IEnumerable<TEntity> FindAllEntitiesByCriteria(
                    int? pageNumber,
                    int? pageSize,
                    out int totalRecords,
                    string sortColumn,
                    string sortDirection,
                     out int offset,
                    out int offsetUpperBound,
                    out int totalNumberOfPages,
                    OperationResult result,
                    ExpressionStarter<TEntity> searchPredicate)
        {
            if (Repository == null) throw new Exception(nameof(Repository));
            if (sortColumn.IsNullOrWhiteSpace()) Error.ArgumentNull(nameof(sortColumn));
            if (sortDirection.IsNullOrWhiteSpace()) Error.ArgumentNull(nameof(sortDirection));

            int pageIndex = pageNumber ?? 1;
            int sizeOfPage = pageSize ?? 10;
            string[] keywords = result.ObjectsDictionary["keywords"] as string[];

            var items = Repository.FindAllEntitiesByCriteria(
                 pageIndex, sizeOfPage, out totalRecords, sortColumn, sortDirection, searchPredicate);

            totalNumberOfPages = (int)Math.Ceiling((double)totalRecords / sizeOfPage);

            offset = (pageIndex - 1) * sizeOfPage + 1;
            offsetUpperBound = offset + (sizeOfPage - 1);
            if (offsetUpperBound > totalRecords) offsetUpperBound = totalRecords;

            result.AddResultObject("sortCol", sortColumn);
            result.AddResultObject("sortDir", sortDirection);
            result.AddResultObject("offset", offset);
            result.AddResultObject("pageIndex", pageIndex);
            result.AddResultObject("sizeOfPage", sizeOfPage);
            result.AddResultObject("offsetUpperBound", offsetUpperBound);
            result.AddResultObject("totalNumberOfRecords", totalRecords);
            result.AddResultObject("totalNumberOfPages", totalNumberOfPages);
            result.AddResultObject("searchTerms", string.Join(",", keywords.Select(i => i.ToString())));

            return items;
        }

        /// <summary>
        /// Returns a default search predicate to filter the data by based on the keywords supplied.
        /// Override this to supply your own search predicate
        /// </summary>
        /// <param name="keywords">Keywords to filter by</param>
        /// <returns>An expression to use for filtering</returns>
        protected virtual ExpressionStarter<TEntity> BuildSearchFilterPredicate(string[] keywords)
        {
            Expression<Func<TEntity, bool>> filterExpression = a => true;
            ExpressionStarter<TEntity> predicate = PredicateBuilder.New(filterExpression);
            
            return predicate;
        }
    }
}