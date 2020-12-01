using llx.cc.models.External.Netcracker;
using llx.cc.models.External.Netcracker.RequestModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpringTransits.RESTClient
{
    public static class CarrierSampleExtensions
    {
        public async static Task<QueryAccountResponse> QueryAccount(this CAWebApiClient webApiClient, NetcrackerDefaultHeadlers defaultHeadlers, QueryAccountByAccountIdRequest requestBody)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);

            return await webApiClient.PostAsync<QueryAccountByAccountIdRequest, QueryAccountResponse>($"account/query-account", requestBody, headers);
        }

        public async static Task<SubscriberInquiryResponse> SubscriberInquiryByIccid(this CAWebApiClient webApiClient,
            NetcrackerDefaultHeadlers defaultHeadlers, SubscriberInquiryRequest requestBody)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);
            return await webApiClient.PostAsync<SubscriberInquiryRequest, SubscriberInquiryResponse>($"subscriber/subscriber-inquiry", requestBody, headers);
        }

        #region Subscriber Actions

        public async static Task<SubscriberActionResponse> DeactivateSubscriber(this CAWebApiClient webApiClient,
            NetcrackerDefaultHeadlers defaultHeadlers, SubscriberActionBaseRequest deactivateSubscriberRequest)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);
            return await webApiClient.PutAsync<SubscriberActionBaseRequest, SubscriberActionResponse>($"subscriber/deactivate-subscriber", deactivateSubscriberRequest, headers);
        }

        public async static Task<SubscriberActionResponse> SuspendSubscriber(this CAWebApiClient webApiClient,
          NetcrackerDefaultHeadlers defaultHeadlers, SubscriberActionBaseRequest suspendSubscriberRequest)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);
            return await webApiClient.PutAsync<SubscriberActionBaseRequest, SubscriberActionResponse>($"subscriber/suspend-subscriber", suspendSubscriberRequest, headers);
        }

        public async static Task<SubscriberActionResponse> RestoreSubscriber(this CAWebApiClient webApiClient,
         NetcrackerDefaultHeadlers defaultHeadlers, SubscriberActionBaseRequest restoreSubscriberRequest)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);
            return await webApiClient.PutAsync<SubscriberActionBaseRequest, SubscriberActionResponse>($"subscriber/restore-subscriber", restoreSubscriberRequest, headers);
        }

        public async static Task<SubscriberActionResponse> ChangeIMEI(this CAWebApiClient webApiClient,
         NetcrackerDefaultHeadlers defaultHeadlers, ChangeIMEIRequest changeIMEIRequest)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);
            return await webApiClient.PutAsync<ChangeIMEIRequest, SubscriberActionResponse>($"subscriber/change-imei", changeIMEIRequest, headers);
        }

        public async static Task<SubscriberActionResponse> ChangeMSISDN(this CAWebApiClient webApiClient,
        NetcrackerDefaultHeadlers defaultHeadlers, ChangeMSISDNRequest changeMSISDNRequest)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);
            return await webApiClient.PutAsync<ChangeMSISDNRequest, SubscriberActionResponse>($"subscriber/change-msisdn", changeMSISDNRequest, headers);
        }

        public async static Task<SubscriberActionResponse> ChangeSIM(this CAWebApiClient webApiClient,
        NetcrackerDefaultHeadlers defaultHeadlers, ChangeSIMRequest changeSIMRequest)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);
            return await webApiClient.PutAsync<ChangeSIMRequest, SubscriberActionResponse>($"subscriber/change-sim", changeSIMRequest, headers);
        }

        public async static Task<SubscriberActionResponse> ActivateSubsriber(this CAWebApiClient webApiClient,
        NetcrackerDefaultHeadlers defaultHeadlers, ActivateSubscriberRequest activateSubscriberRequest)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);
            return await webApiClient.PostAsync<ActivateSubscriberRequest, SubscriberActionResponse>($"subscriber/activate-subscriber", activateSubscriberRequest, headers);
        }

        public async static Task<SubscriberActionResponse> ReactivateSubsriber(this CAWebApiClient webApiClient,
        NetcrackerDefaultHeadlers defaultHeadlers, ReactivateSubscriberRequest reactivateSubscriberRequest)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);
            return await webApiClient.PutAsync<ReactivateSubscriberRequest, SubscriberActionResponse>($"subscriber/reactivate-subscriber", reactivateSubscriberRequest, headers);
        }

        public async static Task<QueryProductsResponse> QueryProducts(this CAWebApiClient webApiClient, NetcrackerDefaultHeadlers defaultHeadlers, QueryProductsRequest queryProductsRequest)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);
            return await webApiClient.PostAsync<QueryProductsRequest, QueryProductsResponse>($"reference/query-products", queryProductsRequest, headers);
        }

        public async static Task<QuerySIMResponse> QuerySimByIccid(this CAWebApiClient webApiClient, NetcrackerDefaultHeadlers defaultHeadlers, QuerySIMRequest request)
        {
            Dictionary<string, string> headers = ConstructDefaultHeaders(defaultHeadlers);
            return await webApiClient.PostAsync<QuerySIMRequest, QuerySIMResponse>($"device/query-sim", request, headers);
        }

        #endregion Subscriber Actions

        private static Dictionary<string, string> ConstructDefaultHeaders(NetcrackerDefaultHeadlers defaultHeadlers)
        {
            var headers = new Dictionary<string, string>
            {
                { "user_key", defaultHeadlers.UserKey },
                { "PartnerId", defaultHeadlers.PartnerId },
                { "SenderId", defaultHeadlers.SenderId },
                { "Content-Type", "application/json" },
            };

            if (defaultHeadlers.IsPITEnvironment)
                headers.Add("Environment", "PIT");

            return headers;
        }
    }

    public class NetcrackerDefaultHeadlers
    {
        public string UserKey { get; set; }
        public string PartnerId { get; set; }
        public string SenderId { get; set; }
        public bool IsPITEnvironment { get; set; } = false;
    }
}