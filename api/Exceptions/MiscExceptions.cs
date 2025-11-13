namespace Fadebook.Exceptions;

public class NoUsernameException : Exception 
{
    public NoUsernameException() : base("Username is required.") { }
    public NoUsernameException(string message) : base(message) { }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}
