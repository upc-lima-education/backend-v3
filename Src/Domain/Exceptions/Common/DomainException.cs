namespace Backend.Src.Domain.Exceptions.Common;
/// <summary>
/// Every custom exception MUST extend from Domain Exception
/// </summary>
public abstract class DomainException(string message) : Exception(message)
{
    public abstract string Title { get; }
    public abstract int StatusCode { get; }
}