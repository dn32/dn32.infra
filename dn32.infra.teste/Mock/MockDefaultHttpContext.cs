using Microsoft.AspNetCore.Http;
using System;

namespace dn32.infra.Test.Mock
{
    public class MockDefaultHttpContext //: DefaultHttpContext
    {
        public MockDefaultHttpContext(IHeaderDictionary headers)
        {
            throw new NotImplementedException();
        }

        //public IHeaderDictionary Headers { get; }

        //public MockDefaultHttpContext(IHeaderDictionary headers)
        //{
        //    Headers = headers;
        //    InitializeHttpRequest();
        //}

        //protected HttpRequest InitializeHttpRequest()
        //{
        //    var httpRequest = base.InitializeHttpRequest();
        //    if (Headers != null) { foreach (var header in Headers) { httpRequest.Headers.Adicionar(header); } }
        //    return httpRequest;
        //}
    }
}
