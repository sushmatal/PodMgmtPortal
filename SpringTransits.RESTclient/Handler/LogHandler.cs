using ca.api.helpers;
using CA.Extensions;
using llx.cc.models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CA.WebApi.Client
{
    public class LogHandler : DelegatingHandler
    {
        public LogHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var apiLogParameters = request.GetHeaderValue("LogParameters").FromJson<ApiLogParameters>();
            if (apiLogParameters != null)
            {
                request.Headers.Remove("LogParameters");
                var requestContent = $"Request : {request.ToString()}, Request Content : {(request.Content == null ? string.Empty : await request.Content?.ReadAsStringAsync())}";

                SqlHelper.LogAPIRequest(apiLogParameters, requestContent);
            }

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (apiLogParameters != null)
            {
                var responseContent = await response.Content?.ReadAsStringAsync();
                SqlHelper.LogAPIResponse(apiLogParameters, response.StatusCode.ToString(), responseContent);
            }

            return response;
        }
    }

    public static class Extensions
    {
        public static string GetHeaderValue(this HttpRequestMessage request, string headerName)
        {
            IEnumerable<string> outputValue;
            if (request.Headers.TryGetValues(headerName, out outputValue))
            {
                if (outputValue.Count() > 0)
                    return outputValue.First();

                return string.Empty;
            }

            return string.Empty;
        }
    }
}