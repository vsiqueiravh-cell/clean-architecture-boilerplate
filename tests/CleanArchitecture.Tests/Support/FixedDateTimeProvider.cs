using CleanArchitecture.Application.Abstractions;

namespace CleanArchitecture.Tests.Support;

public sealed class FixedDateTimeProvider : IDateTimeProvider
{
    public FixedDateTimeProvider(DateTimeOffset utcNow)
    {
        UtcNow = utcNow;
    }

    public DateTimeOffset UtcNow { get; }
}
