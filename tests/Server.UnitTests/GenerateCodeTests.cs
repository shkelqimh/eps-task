using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Options;
using Moq;
using Server.Config;
using Server.UnitTests.Helpers;
using Server.UnitTests.TestData;

namespace Server.UnitTests;

public class GenerateCodeTests
{
    [Theory]
    [ClassData(typeof(GenerateCodesTestData))]
    public void GenerateCodes_WhenRepositoryAddsSuccessfully_ShouldReturnTrue(int count, byte length, bool expectedResult)
    {
        // arrange
        var mockRepo = new Mock<ICodeRepository>();
        mockRepo.Setup(r => r.AddRange(count, length)).Returns(expectedResult);

        var server = DiscountServerMockProvider.CreateServer(mockRepo);

        // act
        var result = server.GenerateCodes(count, length);

        // assert
        Assert.Equal(result, expectedResult);
    }
}