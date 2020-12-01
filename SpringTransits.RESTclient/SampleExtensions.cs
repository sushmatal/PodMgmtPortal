using System.Collections.Generic;

namespace CA.WebApi.Client
{
    public static class SampleExtensions
    {
        public static string GetBilledUnbilledUsage(this CAWebApiClient webApiClient, string ban, int pageNumber = 0, int pageSize = 100, int billSequenceNumber = 0)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "ban", ban },
                { "pageNumber",pageNumber.ToString() },
                { "pageSize", pageSize.ToString() },
                { "billSequenceNumber", billSequenceNumber.ToString() },
            };

            return webApiClient.Post<string>($"voice/tmo/billed-unbilled-usage-summary", headers);
        }
    }
}