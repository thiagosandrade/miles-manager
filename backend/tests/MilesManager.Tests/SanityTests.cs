using FluentAssertions;
using Xunit;

namespace MilesManager.Tests;

public class SanityTests
{
    [Fact]
    public void True_is_true()
    {
        true.Should().BeTrue();
    }
}
