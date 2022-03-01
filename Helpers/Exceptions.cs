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

    [System.Serializable]
    public class BranchMissingException : MyManagedException
    {
        public BranchMissingException() : base("ERR03: 'branch' must be not empty or null (i.e. master)") { }
    }

    [System.Serializable]
    public class RepositoryMissingException : MyManagedException
    {
        public RepositoryMissingException() : base("ERR04: 'Repository' must be not empty or null") { }
    }

    public class UserMissingException : MyManagedException
    {
        public UserMissingException() : base("ERR05: 'Username' must be not empty or null") { }
    }

     public class DisplaynameMissingException : MyManagedException
    {
        public DisplaynameMissingException() : base("ERR05: 'Displayname' must be not empty or null") { }
    }

}