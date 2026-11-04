namespace PatriSystem.Application.Results;

public abstract record BusinessError(string Code, string Description);

public sealed record ValidationError(
    string Code,
    string Description,
    IReadOnlyDictionary<string, string[]> Failures) : BusinessError(Code, Description);

public sealed record NotFoundError(string Code, string Description)
    : BusinessError(Code, Description);

public sealed record ConflictError(string Code, string Description)
    : BusinessError(Code, Description);

public sealed record UnauthorizedError(string Code, string Description)
    : BusinessError(Code, Description);