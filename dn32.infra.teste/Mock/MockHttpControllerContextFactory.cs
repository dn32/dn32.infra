using dn32.infra.Mock.Novos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dn32.infra.Mock
{
    public static class MockHttpControllerContextFactory
    {
        public static ControllerContext Create(IHeaderDictionary Headers = null)
        {
            var context = new ControllerContext()
            {
                ActionDescriptor = MockControllerActionDescriptorFactory.Create(),
                HttpContext = MockHttpContextFactory.Create(Headers),
                RouteData = MockRouteDataFactory.Create()
            };

            return context;
        }
    }
}