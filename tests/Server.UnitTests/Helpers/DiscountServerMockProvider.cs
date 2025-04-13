using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Options;
using Moq;
using Server.Config;

namespace Server.UnitTests.Helpers;

public static class DiscountServerMockProvider
{
    public static DiscountServer CreateServer(Mock<ICodeRepository> mockRepo)
    {
        var settings = new Settings
        {
            CodeRules = new CodeRules
            {
                MaxCodeLength = 10,
                MinCodeLength = 6,
                MaxCodeGenerationWithinRequestAllowed = 100
            }
        };

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IOptions<Settings>)))
            .Returns(Options.Create(settings));
        serviceProvider.Setup(x => x.GetService(typeof(ICodeRepository)))
            .Returns(mockRepo.Object);

        return new DiscountServer(serviceProvider.Object);
    }
}