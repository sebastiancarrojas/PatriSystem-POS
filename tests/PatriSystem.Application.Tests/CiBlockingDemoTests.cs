using Shouldly;

namespace PatriSystem.Application.Tests;

public sealed class CiBlockingDemoTests
{
    [Fact]
    public void Demo_DeliberateFailure_BlocksTheMerge()
    {
        1.ShouldBe(2);
    }
}