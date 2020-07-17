using Microsoft.AspNetCore.Http;
using System;

namespace dn32.infra.Mock
{
    public static class MockHttpContextFactory
    {
        public static HttpContext Create(IHeaderDictionary Headers)
        {
            throw new NotImplementedException();
        }
    }
}