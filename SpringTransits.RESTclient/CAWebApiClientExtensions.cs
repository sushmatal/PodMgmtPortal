using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SpringTransits.RESTClient
{
    public static class CAWebApiClientExtensions
    {
        public static string DeviceDetails(this SampleRESTClient webApiClient, string iccid)
        {
            return webApiClient.Get<JArray>($"api/values/devicedetails/{iccid}").ToString();
        }

        public static string GetUsersByCompany(this CAWebApiClient webApiClient)
        {
            return webApiClient.Get<JArray>($"api/metadata/users").ToString();
        }

        public static string GetEnums(this CAWebApiClient webApiClient)
        {
            return webApiClient.Get<JArray>($"api/metadata/enums").ToString();
        }

        public static string GetNotificationRules(this CAWebApiClient webApiClient)
        {
            return webApiClient.Get<JArray>($"api/notification/rules").ToString();
        }

        public static string GetNotificationCustomRulePredicates(this CAWebApiClient webApiClient)
        {
            return webApiClient.Get<JArray>($"api/notification/customrulepredicates").ToString();
        }

        public static string GetSubscribedNotifications(this CAWebApiClient webApiClient)
        {
            return webApiClient.Get<JArray>($"api/notification/subscribednotifications").ToString();
        }

        public static string GetNotificationCustomRules(this CAWebApiClient webApiClient)
        {
            return webApiClient.Get<JArray>($"api/notification/customrules").ToString();
        }

        public static string GetNotificationRuleMetrics(this CAWebApiClient webApiClient)
        {
            return webApiClient.Get<JArray>($"api/notification/rulemetrics").ToString();
        }

        public static string AddorEditCustomerNotifications(this CAWebApiClient webApiClient, string requestParameters)
        {
            return webApiClient.Post<string, string>($"api/notification/addoreditnotification", requestParameters);
        }

        public static string CustomizeNotificationRules(this CAWebApiClient webApiClient, string requestParameters)
        {
            return webApiClient.Post<string, string>($"api/notification/customizenotificationrules", requestParameters);
        }

        public static string AddorEditNotificationRules(this CAWebApiClient webApiClient, string requestParameters)
        {
            return webApiClient.Post<string, string>($"api/notification/addoreditnotificationrules", requestParameters);
        }

        public static string GetAllDevices(this CAWebApiClient webApiClient, string requestParameters)
        {
            return webApiClient.GetAsync<JArray>($"api/device/all", new Dictionary<string, string>() { { "requestParameters", requestParameters } }).ToString();
        }
    }
}