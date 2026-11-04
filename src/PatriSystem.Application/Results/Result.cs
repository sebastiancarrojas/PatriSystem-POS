namespace PatriSystem.Application.Results;

public sealed class Result<T>
{
    private readonly T? _value;
    private readonly BusinessError? _failure;

    internal Result(T? Value, BusinessError? Failure)
    {
        _value = Value;
        _failure = Failure;
    }

    public bool IsSuccess => _failure is null;
    public bool IsFailure => !IsSuccess;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException(
            "No se puede leer Value de un resultado fallido.");
    public BusinessError Failure => IsFailure
        ? _failure!
        : throw new InvalidOperationException(
            "No se puede leer Failure de un resultado exitoso.");
}

public static class Result
{
    public static Result<T> Success<T>(T value) => new(value, null);
    public static Result<T> Failure<T>(BusinessError failure)
    {
        ArgumentNullException.ThrowIfNull(failure, nameof(failure));
        return new Result<T>(default, failure);
    }
}