using System.Threading.Tasks;
using Core.Common.Data.Interfaces;
using Core.Common.Data.Models;

namespace Core.Common.Data.Business
{
    public interface IEntityBusiness<TEntity>
        where TEntity : BaseObjectWithState, IObjectWithState, new()
    {

        OperationResult ListItems(
            int? pageNumber, int? pageSize, string sortCol,
            string sortDir, string searchTerms);

        Task<TEntity> FindEntityById(int id);

        Task<bool> PersistEntity(TEntity entity);
    }
}