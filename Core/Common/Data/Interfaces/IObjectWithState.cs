namespace Core.Common.Data.Interfaces
{
    /// <summary>
    /// An interface to make it easy to work with Entity Framework enties. Originally created by Julia Lerman
    /// See http://shop.oreilly.com/product/0636920022237.do?sortby=publicationDate
    /// All entities which are saved into the database by EF should implement this interface and also extend the BaseObjectWithState class. 
    /// See <see cref="BaseObjectWithState"/> class. Also <see cref="EfDataRepositoryBase{T,TContext}"/> on how the interface and the base class are used.
    /// </summary>
    public interface IObjectWithState
    {
        ObjectState ObjectState { get; set; }
    }

    /// <summary>
    /// Enum that represents client-side entity state
    /// </summary>
    public enum ObjectState
    {
        Unchanged = 0,
        Added = 1,
        Modified = 2,
        Deleted = 3
    }
}