using System;

namespace github_to_lametric.Helpers.Exceptions
{
    [System.Serializable]
    public class MyManagedException : Exception
    {
        public MyManagedException() { }
        public MyManagedException(string message) : base(message) { }
        public MyManagedException(string message, System.Exception inner) : base(message, inner) { }
        protected MyManagedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    
    [System.Serializable]
    public class LastValueException : MyManagedException
    {
        public LastValueException() : base("ERR02: 'last' must be greather than zero") { }
    }
}