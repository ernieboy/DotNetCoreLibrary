using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Data.Models
{
    /// <summary>
    /// An OperationResult object is useful when you perform an operation and you need to return multiple pieces of data from the operation
    /// For example, you might need to return a list of errors that occur as part of invoking the operation. You could use this class when saving stuff into storage
    /// either locally in a database or remotely in web service calls. 
    /// Inspired by Deborah Kurata (https://www.pluralsight.com/authors/deborah-kurata).
    /// </summary>
    public class OperationResult
    {
        private List<string> _messages;
        private Dictionary<string, object> _objectsDictionary;

        /// <summary>
        /// We initially set Success to True on construction because we are optimistic that the operation will succeed.  Always remember to set Success
        /// to False as soon as errors are encountered when invoking the operation. 
        /// </summary>
        public OperationResult()
        {
            Success = true;
            _messages = new List<string>();
            _objectsDictionary = new Dictionary<string, object>();
        }

        /// <summary>
        /// This flag communicates to the client whether the operation was a success or failure. It should be set to False if the operation failed. The
        /// client must always check this flag after invoking an operation which returns an OperationResult object in order to decide what to do next.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// List of messages to return to the client. You would normally populate this list with error messages which occur when performing the operation.
        /// The client should then use these messages to communicate with the user e.g. display the messages as UI errors. Note, these messages must be sanitised 
        /// before being displayed on the UI, for example, do not add stack traces here since they will reveal sensitive information about the underlying sysytem.
        /// Stack traces are useless to users as well.
        /// </summary>
        public IReadOnlyCollection<string> Messages => _messages.Any() ? new List<string>(_messages) : (_messages = new List<string>());

        /// <summary>
        /// Adds a message to the list of messages to return back to the client.
        /// </summary>
        /// <param name="message">The message to add</param>
        public void AddMessage(string message)
        {
            if (message == null) return;
            _messages.Add(message);
        }

        /// <summary>
        /// This dictionary is useful if you want to return some objects back to the calling client. For example, suppose we were persisting a new record into the 
        /// database, we could add the entity which was just persisted into this dictionary should the client require it. e.g. 
        ///  ObjectsDictionary.Add("PersistedRecord", entity). The client would then have to cast it to the appropriate entity type when extracting it from the 
        /// dictionary.
        /// </summary>
        public IReadOnlyDictionary<string, object> ObjectsDictionary => _objectsDictionary ?? (_objectsDictionary = new Dictionary<string, object>());

        /// <summary>
        /// Adds an object to the object dictionary
        /// </summary>
        /// <param name="objectKey">The key for retrieving the object</param>
        /// <param name="objectValue">The value of the object</param>
        public void AddObjectToDictionary(string objectKey, object objectValue)
        {
            if (string.IsNullOrEmpty(objectKey) || objectValue == null) return;
            _objectsDictionary.Add(objectKey, objectValue);
        }
    }
}
