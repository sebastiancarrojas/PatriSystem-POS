using PatriSystem.Application.Results;

using Shouldly;

namespace PatriSystem.Application.Tests.Results;

public sealed class ResultTests
{
    [Fact]
    public void Success_ValueProvided_ReturnsSuccessfulResultWithValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Failure_ErrorProvided_ReturnsFailedResultWithError()
    {
        var error = new NotFoundError("user.not_found", "Usuario no encontrado.");

        var result = Result.Failure<int>(error);

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Failure.ShouldBe(error);
    }

    [Fact]
    public void Value_ResultIsFailure_ThrowsInvalidOperationException()
    {
        var result = Result.Failure<int>(new ConflictError("x", "y"));

        Should.Throw<InvalidOperationException>(() => { _ = result.Value; });
    }

    [Fact]
    public void Failure_ResultIsSuccess_ThrowsInvalidOperationException()
    {
        var result = Result.Success(42);

        Should.Throw<InvalidOperationException>(() => { _ = result.Failure; });
    }

    [Fact]
    public void Failure_NullError_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => { _ = Result.Failure<int>(null!); });
    }
}