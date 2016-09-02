using Microsoft.EntityFrameworkCore;
using Core.Common.Data.Interfaces;

namespace Core.Common.Extensions
{
    public static class EfExtensions
    {
        /// <summary>
        /// Converts the client side ObjectState object into Entity Framework EntityState 
        /// Note: the entity state chain of custody is the responsibility of the client code. 
        /// For example, if the application loads an entity from the database and then displays it on the UI for 
        /// the user to edit, the client-side ObjectState of the entity would go from ObjectState.Unchaged (at the time is is loaded from the Db), 
        ///  to ObjectState.Modified(when the user changes it. The client needs some mechanism for ensuring that it keeps track of the entities that it is
        /// currently using. For example, as soon as the user has edited an enity on the client, the UI should mark it as modified so that EF can know what to do
        /// with the object when it cames back.   
        /// </summary>
        /// <param name="objectState">The ObjectState object from the client code.  </param>
        /// <returns>An Entity Framework EntityState object</returns>
        public static EntityState ConvertState(ObjectState objectState)
        {
            switch (objectState)
            {
                case ObjectState.Added:
                    return EntityState.Added;
                case ObjectState.Modified:
                    return EntityState.Modified;
                case ObjectState.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Unchanged;
            }
        }

        /// <summary>
        /// Converts the Entity Framework EntityState into client side ObjectState object
        /// </summary>
        /// <param name="objectState"></param>
        /// <returns>A client side ObjectState</returns>
        public static ObjectState ConvertState(EntityState objectState)
        {
            switch (objectState)
            {
                case EntityState.Added:
                    return ObjectState.Added;
                case EntityState.Modified:
                    return ObjectState.Modified;
                case EntityState.Deleted:
                    return ObjectState.Deleted;
                default:
                    return ObjectState.Unchanged;
            }
        }


        /// <summary>
        /// This method must be called after attaching an entity just before calling DbContext.SaveChanges().
        /// It is the mechanism by which the DbContext knows the state each object is in and can then know explicitly what to do with each object.
        /// </summary>
        /// <param name="context">The DbContext class.</param>
        public static void ApplyStateChanges(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries<IObjectWithState>())
            {
                IObjectWithState stateInfo = entry.Entity;
                entry.State = ConvertState(stateInfo.ObjectState);
            }
        }       
    }
}
