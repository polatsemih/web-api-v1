using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace SUPBank.UnitTests.xUnit.Utilities.Helpers
{
    public static class ControllerTestHelper
    {
        public static void SetHttpContext(ControllerBase controller)
        {
            var httpContext = new DefaultHttpContext();

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(h => h.HttpContext).Returns(httpContext);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextAccessor.Object.HttpContext ?? new DefaultHttpContext()
            };
        }
    }
}
