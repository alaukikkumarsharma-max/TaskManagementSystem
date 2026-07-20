namespace TaskManagement.Application.Exceptions;

/// <summary>Thrown when a requested entity doesn't exist; maps to HTTP 404.</summary>
public class NotFoundException : Exception
{
    /// <summary>Creates the exception with a custom message.</summary>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>Creates the exception with a standard "entity with id X not found" message.</summary>
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.")
    {
    }
}

/// <summary>Thrown for invalid client input; maps to HTTP 400.</summary>
public class BadRequestException : Exception
{
    /// <summary>Creates the exception with a custom message.</summary>
    public BadRequestException(string message) : base(message)
    {
    }
}

/// <summary>Thrown for failed authentication; maps to HTTP 401.</summary>
public class UnauthorizedException : Exception
{
    /// <summary>Creates the exception with a custom message.</summary>
    public UnauthorizedException(string message) : base(message)
    {
    }
}
