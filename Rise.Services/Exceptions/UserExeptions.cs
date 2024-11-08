public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message) : base(message) { }
}

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string message) : base(message) { }
}

public class ExternalServiceException : Exception
{
    public ExternalServiceException(string message, Exception innerException) : base(message, innerException) { }
}

public class DatabaseOperationException : Exception
{
    public DatabaseOperationException(string message, Exception innerException) : base(message, innerException) { }
}
