﻿using Microsoft.AspNetCore.Routing;

namespace dn32.infra.Mock {
    public static class MockRouteDataFactory {
        public static RouteData Create () {
            var route = new RouteData ();
            return route;
        }
    }
}