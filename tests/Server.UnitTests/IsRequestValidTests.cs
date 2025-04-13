using Infrastructure.Persistence.Repositories;
using Moq;
using Server.UnitTests.Helpers;

namespace Server.UnitTests;

public class IsRequestValidTests
{
    [Theory]
    [InlineData(50, 8, true)]   // Valid request
    [InlineData(101, 8, false)] // Invalid: count too high
    [InlineData(50, 5, false)]  // Invalid: length too short
    [InlineData(50, 11, false)] // Invalid: length too long
    [InlineData(100, 8, true)]  // Valid: count at max
    [InlineData(50, 6, true)]   // Valid: length at min
    [InlineData(50, 10, true)]  // Valid: length at max
    public void IsRequestValid_ReturnsExpectedResult(int count, byte length, bool expected)
    {
        // arrange
        var server = DiscountServerMockProvider.CreateServer(new Mock<ICodeRepository>());

        // act
        var result = server.IsRequestValid(count, length);

        // assert
        Assert.Equal(expected, result);
    }
}