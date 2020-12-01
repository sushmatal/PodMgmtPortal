using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace SpringTransits.RESTClient
{
    public class CARESTClient : RESTClient
    {
        public CARESTClient(string baseUrl) : base(baseUrl)
        {
        }

        public async Task<CARESTClient> Login(string userName, string password, string relativeUri = "account/login")
        {
            var inputParamter = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));

            AccessToken = await GetAsync<Token>(relativeUri, new Dictionary<string, string>() { { "apiKey", inputParamter } });

            return this;
        }

        /// <summary>
        /// New Post Method introduced exclusively for Magenta to support V6 oAuth token
        /// </summary>
        /// <param name="userName"> Client_Id, you'll get this from TMO</param>
        /// <param name="password"> Client_Secret, you'll get this from TMO </param>
        /// <param name="clientId"> B2bClient or also known as StoreId for this particular request</param>
        /// <param name="dealerCode"> Dealer Code</param>
        /// <param name="relativeUri"> /oauth2/v6/tokens</param>
        /// <returns></returns>
        public async Task<CARESTClient> Login(string userName, string password, string clientId, string dealerCode, string relativeUri = "account/login")
        {
            AddHeader(HttpRequestHeader.Accept.ToString(), "application/json");
            var request = new MagentaLoginRequest
            {
                StoreId = clientId,
                DealerCode = dealerCode,
                ClientId = userName,
                ClientSecret = password,
                Scope = "response_mode",
                GrantType = "client_credentials"
            };
            ClearHeaders();

            AccessToken = await PostAsync<MagentaLoginRequest, Token>($"{relativeUri}", request);
            if (AccessToken.TokenType.Equals("BearerToken"))
                AccessToken.TokenType = "Bearer";

            return this;
        }
    }

    public class MagentaLoginRequest
    {
        public string StoreId { get; set; }

        public string DealerCode { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Scope { get; set; }

        public string GrantType { get; set; }
    }
}