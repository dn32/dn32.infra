using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Filters;

namespace dn32.infra.Mock.ControllerMock
{
    public static class MockActionExecutingContextFactory
    {
        public static ActionExecutingContext Create(DnControladorBase controller)
        {
            var actionContext = MockActionContextFactory.Create();
            return new ActionExecutingContext(
                actionContext,
                filters: new List<IFilterMetadata>(),
                actionArguments: new Dictionary<string, object>(),
                controller: controller);
        }
    }
}