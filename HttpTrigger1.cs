using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PnP.Core.Services;

namespace Company.Function
{
    public class HttpTrigger1
    {
        private readonly ILogger _logger;
        private PnPContext ctx;
        public HttpTrigger1(ILoggerFactory loggerFactory, IPnPContextFactory factory)
        {
            _logger = loggerFactory.CreateLogger<HttpTrigger1>();
            ctx = factory.Create(new Uri("site"));
        }

        [Function("HttpTrigger1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var web = await ctx.Web.GetAsync(w => w.Title);

            string responseMessage = web.Title;
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString(responseMessage);

            return response;
        }
    }
}
