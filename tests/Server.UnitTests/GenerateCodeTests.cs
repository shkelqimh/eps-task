using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Options;
using Moq;
using Server.Config;
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

        var settings = new Settings
        {
            CodeRules = new CodeRules
            {
                MaxCodeGenerationWithinRequestAllowed = 2000,
                MinCodeLength = 7,
                MaxCodeLength = 8
            }
        };
        
        var mockProvider = new Mock<IServiceProvider>();
        mockProvider.Setup(p => p.GetService(typeof(IOptions<Settings>)))
            .Returns(Options.Create(settings));
        mockProvider.Setup(p => p.GetService(typeof(ICodeRepository)))
            .Returns(mockRepo.Object);

        var server = new DiscountServer(mockProvider.Object);

        // act
        var result = server.GenerateCodes(count, length);

        // assert
        Assert.Equal(result, expectedResult);
    }
}