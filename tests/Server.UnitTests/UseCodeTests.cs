using Infrastructure.Persistence.Repositories;
using Moq;
using Server.UnitTests.Helpers;
using Shared.Enums;

namespace Server.UnitTests;

public class UseCodeTests
{
    [Fact]
    public void UseCode_WhenCodeDoesNotExist_ReturnsCodeDoesNotExist()
    {
        // arrange
        var mockRepo = new Mock<ICodeRepository>();
        mockRepo.Setup(r => r.Exists("ABC123")).Returns(false);

        var server = DiscountServerMockProvider.CreateServer(mockRepo);

        // act
        var result = server.UseCode("ABC123");

        // assert
        Assert.Equal((byte)UseCodeResult.CodeDoesNotExist, result);
    }

    [Fact]
    public void UseCode_WhenCodeIsAlreadyUsed_ReturnsCodeAlreadyUsed()
    {
        // arrange
        var mockRepo = new Mock<ICodeRepository>();
        mockRepo.Setup(r => r.Exists("ABC123")).Returns(true);
        mockRepo.Setup(r => r.IsUsed("ABC123")).Returns(true);

        var server = DiscountServerMockProvider.CreateServer(mockRepo);

        // act
        var result = server.UseCode("ABC123");

        // assert
        Assert.Equal((byte)UseCodeResult.CodeAlreadyUsed, result);
    }

    [Fact]
    public void UseCode_WhenUseCodeFails_ReturnsFail()
    {
        // arrange
        var mockRepo = new Mock<ICodeRepository>();
        mockRepo.Setup(r => r.Exists("ABC123")).Returns(true);
        mockRepo.Setup(r => r.IsUsed("ABC123")).Returns(false);
        mockRepo.Setup(r => r.UseCode("ABC123")).Returns(false);

        var server = DiscountServerMockProvider.CreateServer(mockRepo);

        // act
        var result = server.UseCode("ABC123");

        // assert
        Assert.Equal((byte)UseCodeResult.Fail, result);
    }

    [Fact]
    public void UseCode_WhenUseCodeSucceeds_ReturnsSuccess()
    {
        // arrange
        var mockRepo = new Mock<ICodeRepository>();
        mockRepo.Setup(r => r.Exists("ABC123")).Returns(true);
        mockRepo.Setup(r => r.IsUsed("ABC123")).Returns(false);
        mockRepo.Setup(r => r.UseCode("ABC123")).Returns(true);

        var server = DiscountServerMockProvider.CreateServer(mockRepo);

        // act
        var result = server.UseCode("ABC123");

        // assert
        Assert.Equal((byte)UseCodeResult.Success, result);
    }
}