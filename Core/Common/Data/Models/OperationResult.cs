using System.Collections.Generic;

namespace Core.Common.Data.Models
{
    /// <summary>
    /// An OperationResult object is useful when you perform an operation and you need to return multiple  pieces of data from the operation
    /// For example, you might need to return a list of errors that occur as part of invoking the operation. You could use this class when saving stuff into storage
    /// either locally in a database or remotely in web service calls.
    /// </summary>
    public class OperationResult
    {
        private ICollection<string> _messages;

        private Dictionary<string, object> _messagesDictionary;

        /// <summary>
        /// We initially set Success to True on construction because we are optimistic that the operation will succeed.  Always remember to set Success
        /// to False as soon as errors are encountered when invoking the operation. 
        /// </summary>
        public OperationResult()
        {
            Success = true;
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
        public ICollection<string> Messages
        {
            get { return _messages ?? (_messages = new List<string>()); }
            set { _messages = value; }
        }

        /// <summary>
        /// This dictionary is useful if you wnat to return some objects back to the calling client. For example, suppose we were persisting a new record into the 
        /// database, we could add the entity which was just persisted into this dictionary should the client require it. e.g. 
        ///  MessagesDictionary.Add("PersistedRecord", entity). The client would then have to cast it to the appropriate entity type when extracting it from the 
        /// dictionary.
        /// </summary>
        public Dictionary<string, object> MessagesDictionary
        {
            get { return _messagesDictionary ?? (_messagesDictionary = new Dictionary<string, object>()); }
            set { _messagesDictionary = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message)
        {
            if (message == null) return;
            Messages.Add(message);
        }
    }
}
