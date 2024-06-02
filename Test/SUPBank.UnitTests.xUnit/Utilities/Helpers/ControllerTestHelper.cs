using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using MediatR;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Entities;

namespace SUPBank.UnitTests.xUnit.Utilities.Helpers
{
    public static class ControllerTestHelper
    {
        public static void SetupValidationService<TRequest>(Mock<IValidationService> validationServiceMock, TRequest request, string? expectedErrorMessage)
        {
            validationServiceMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(expectedErrorMessage);
        }

        public static void SetupMediator<TRequest, TResponse>(Mock<IMediator> mediatorMock, TRequest request, TResponse mediatorResponse)
        {
            mediatorMock.Setup(m => m.Send(request!, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(mediatorResponse);
        }

        public static void SetupCacheService(Mock<IRedisCacheService> cacheServiceMock, List<EntityMenu>? cachedMenus)
        {
            cacheServiceMock.Setup(c => c.GetCacheAsync<List<EntityMenu>>(Cache.CacheKeyMenu))
                            .ReturnsAsync(cachedMenus);
        }

        public static void SetHttpContext(ControllerBase controller)
        {
            var httpContext = new DefaultHttpContext();

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext)
                               .Returns(httpContext);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextAccessor.Object.HttpContext ?? new DefaultHttpContext()
            };
        }
    }
}
