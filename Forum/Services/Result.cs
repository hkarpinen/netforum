using Microsoft.AspNetCore.Identity;

namespace NETForum.Services;

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

// Non-generic Result
public class Result
{
    public bool IsSuccess { get; protected init; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; protected init; }

    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    
    public static Result FromIdentityResult(IdentityResult identityResult)
    {
        if (identityResult.Succeeded) return Success();
        
        var errorMessage = string.Join(", ", identityResult.Errors.Select(e => e.Description));
        return Failure(new Error("Identity.Failed", errorMessage));
    }
}

// Generic Result<T>
public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess 
        ? _value! 
        : throw new InvalidOperationException("Cannot access Value of a failed result");

    private Result(T value) : base(true, Error.None)
    {
        _value = value;
    }

    private Result(Error error) : base(false, error)
    {
        _value = default;
    }

    public static Result<T> Success(T value) => new(value);
    public new static Result<T> Failure(Error error) => new(error);
    
    public static implicit operator Result<T>(Error error) => Failure(error);
}