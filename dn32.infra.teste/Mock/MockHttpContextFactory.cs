using System;
using Microsoft.AspNetCore.Http;

namespace dn32.infra.Mock {
    public static class MockHttpContextFactory {
        public static HttpContext Create (IHeaderDictionary Headers) {
            throw new NotImplementedException ();
        }
    }
}