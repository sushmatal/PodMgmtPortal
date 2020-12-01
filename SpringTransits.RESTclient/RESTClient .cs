
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SpringTransits.RESTClient
{
    public class RESTClient : IDisposable

    {
        public Token AccessToken { get; set; }
        public bool IncludeDefaultHeaders { get; set; } = true;
        private HttpClient _httpClient;
        private Dictionary<string, string> _headers = new Dictionary<string, string>();
        public JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings();

        public DateTime? TokenExpiryDate
        {
            get
            {
                if (AccessToken == null)
                    return null;

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadToken(AccessToken.AccessToken) as JwtSecurityToken;
                return token.ValidTo;
            }
        }

        public void AddHeader(string key, string value)
        {
            if (_headers.ContainsKey(key))
                throw new InvalidOperationException("The header already exists.");

            _headers.Add(key, value);
        }

        public void ClearHeaders()
        {
            if (_headers != null)
                _headers.Clear();
        }

        public RESTClient(string baseUrl)
        {
            _httpClient = new HttpClient(new LogHandler(new HttpClientHandler())) { BaseAddress = new Uri(baseUrl) };

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        #region GET

        public TOut Get<TOut>(string relativePath)
        {
            var result = InternalGetAsync(relativePath).Result;
            if (result.IsSuccessful)
                return JsonConvert.DeserializeObject<TOut>(result.Data);

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        public string Get(string relativePath, Dictionary<string, string> input)
        {
            var result = InternalGetAsync(relativePath, input).Result;
            if (result.IsSuccessful)
            {
                return result.Data;
            }

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        public TOut Get<TOut>(string relativePath, Dictionary<string, string> input)
        {
            var result = InternalGetAsync(relativePath, input).Result;
            if (result.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<TOut>(result.Data);
            }

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        public async Task<TOut> GetAsync<TOut>(string relativePath, Dictionary<string, string> input)
        {
            var result = await InternalGetAsync(relativePath, input);
            if (result.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<TOut>(result.Data);
            }

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        public async Task DownloadFileAsync(string relativePath, string filePath, Dictionary<string, string> input)
        {
            await GetDownloadFileAsync(relativePath, filePath, input);
        }

        #endregion GET

        #region POST

        public TOut Post<TOut>(string relativePath, Dictionary<string, string> headers = null)
        {
            var result = InternalSendAsync<TOut>(relativePath, headers).Result;
            if (result.IsSuccessful)
                return JsonConvert.DeserializeObject<TOut>(result.Data);

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        public async Task<TOut> PostAsync<TOut>(string relativePath, Dictionary<string, string> headers = null)
        {
            var result = await InternalSendAsync<TOut>(relativePath, headers);
            if (result.IsSuccessful)
                return JsonConvert.DeserializeObject<TOut>(result.Data);

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        public TOut Post<TIn, TOut>(string relativePath, TIn input, Dictionary<string, string> headers = null)
        {
            var result = InternalSendAsync(relativePath, input, headers).Result;
            if (result.IsSuccessful)
                return JsonConvert.DeserializeObject<TOut>(result.Data);

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        public async Task<TOut> PostAsync<TIn, TOut>(string relativePath, TIn input, Dictionary<string, string> headers = null)
        {
            var result = await InternalSendAsync(relativePath, input, headers);
            if (result.IsSuccessful)
                return JsonConvert.DeserializeObject<TOut>(result.Data);

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        public string Post(string relativePath, string input, Dictionary<string, string> headers = null)
        {
            var result = InternalSendAsync(relativePath, input, headers).Result;
            if (result.IsSuccessful)
                return result.Data;

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        #endregion POST

        #region PUT

        public TOut Put<TIn, TOut>(string relativePath, TIn input, Dictionary<string, string> headers = null)
        {
            var result = InternalSendAsync(relativePath, input, headers, HttpVerbs.PUT).Result;
            if (result.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<TOut>(result.Data);
            }

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        public async Task<TOut> PutAsync<TIn, TOut>(string relativePath, TIn input, Dictionary<string, string> headers = null)
        {
            var result = await InternalSendAsync(relativePath, input, headers, HttpVerbs.PUT);
            if (result.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<TOut>(result.Data);
            }

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        public string Put(string relativePath, string input, Dictionary<string, string> headers = null)
        {
            var result = InternalSendAsync(relativePath, input, headers, HttpVerbs.PUT).Result;
            if (result.IsSuccessful)
            {
                return result.Data;
            }

            throw new HttpException((int)HttpStatusCode.InternalServerError, "Internal Server Error.");
        }

        #endregion PUT

        #region Private Methods

        private HttpRequestMessage ConstructHttpRequest(HttpVerbs verb, Uri requestUri, HttpContent content = null, Dictionary<string, string> headers = null)
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod(verb.ToString()),
                RequestUri = requestUri,
            };

            if (content != null && verb == HttpVerbs.POST || verb == HttpVerbs.PUT)
            {
                httpRequest.Content = content;
            }

            if (IncludeDefaultHeaders)
            {
                httpRequest.Headers.TryAddWithoutValidation("Accept", "application/json");
                httpRequest.Headers.TryAddWithoutValidation("Accept-Encoding", "deflate");
            }

            //Global headers
            if (_headers != null)
            {
                foreach (var item in _headers)
                    httpRequest.Headers.TryAddWithoutValidation(item.Key, item.Value);
            }

            //Custom  headers
            if (headers != null)
            {
                foreach (var item in headers)
                    httpRequest.Headers.TryAddWithoutValidation(item.Key, item.Value);
            }

            if (AccessToken != null)
            {
                httpRequest.Headers.TryAddWithoutValidation("Authorization", AccessToken.TokenType + " " + AccessToken.AccessToken);
            }

            return httpRequest;
        }

        private Uri BuildCompleteUri(string relativePath)
        {
            Uri returnUri = _httpClient.BaseAddress.Combine(relativePath); // new Uri(_httpClient.BaseAddress, relativePath);// new Uri(VirtualPathUtility.Combine(VirtualPathUtility.AppendTrailingSlash(_httpClient.BaseAddress.ToString()), relativePath));

            return returnUri;
        }

        private async Task<ApiResponse> InternalSendAsync<TIn>(string relativePath, Dictionary<string, string> headers = null)
        {
            var completeUri = this.BuildCompleteUri(relativePath);
            var httpRequest = ConstructHttpRequest(HttpVerbs.POST, completeUri, headers: headers);
            ApiResponse response = await SendRequest(httpRequest);

            return response;
        }

        private async Task<ApiResponse> InternalSendAsync<TIn>(string relativePath, TIn input, Dictionary<string, string> headers = null, HttpVerbs httpVerbs = HttpVerbs.POST)
        {
            string requestBody;
            if (input.GetType() == typeof(string)) // Assumption - string if of Json type.
            {
                requestBody = input.ToString();
            }
            else
            {
                requestBody = JsonConvert.SerializeObject(input, JsonSerializerSettings);
            }

            var content = input == null ? null : new StringContent(requestBody, Encoding.UTF8, "application/json");
            var completeUri = this.BuildCompleteUri(relativePath);
            var httpRequest = ConstructHttpRequest(httpVerbs, completeUri, content, headers);
            ApiResponse response = await SendRequest(httpRequest);

            return response;
        }

        private async Task<ApiResponse> InternalGetAsync(string relativePath)
        {
            var completeUri = this.BuildCompleteUri(relativePath);
            var httpRequest = ConstructHttpRequest(HttpVerbs.GET, completeUri);
            ApiResponse response = await SendRequest(httpRequest);

            return response;
        }

        private async Task<ApiResponse> InternalGetAsync(string relativePath, Dictionary<string, string> headers)
        {
            var completeUri = this.BuildCompleteUri(relativePath);
            var httpRequest = ConstructHttpRequest(HttpVerbs.GET, completeUri, null, headers);
            ApiResponse response = await SendRequest(httpRequest);

            return response;
        }

        private async Task GetDownloadFileAsync(string relativePath, string filePath, Dictionary<string, string> headers)
        {
            var completeUri = this.BuildCompleteUri(relativePath);
            var httpRequest = ConstructHttpRequest(HttpVerbs.GET, completeUri, null, headers);
            await SendRequestToDownloadFile(httpRequest, filePath);
        }

        private async Task<ApiResponse> SendRequest(HttpRequestMessage httpRequest)
        {
            var response = new ApiResponse();
            try
            {
                var apiResponse = await _httpClient.SendAsync(httpRequest);
                if (apiResponse.IsSuccessStatusCode)
                {
                    string responseData = apiResponse.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrWhiteSpace(responseData))
                    {
                        response.IsSuccessful = true;
                        response.Data = responseData;
                    }
                }
                else
                {
                    var content = apiResponse.Content.ReadAsStringAsync().Result;
                    var message = string.IsNullOrEmpty(content) ? apiResponse.ReasonPhrase : content;
                    throw new HttpException((int)apiResponse.StatusCode, message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        private async Task SendRequestToDownloadFile(HttpRequestMessage httpRequest, string filePath)
        {
            try
            {
                var apiResponse = await _httpClient.SendAsync(httpRequest);
                if (apiResponse.IsSuccessStatusCode)
                {
                    using (var ms = await apiResponse.Content.ReadAsStreamAsync())
                    {
                        using (var fs = File.Create(filePath))
                        {
                            ms.Seek(0, SeekOrigin.Begin);
                            ms.CopyTo(fs);
                        }
                    }
                }
                else
                {
                    var content = apiResponse.Content.ReadAsStringAsync().Result;
                    var message = string.IsNullOrEmpty(content) ? apiResponse.ReasonPhrase : content;
                    throw new HttpException((int)apiResponse.StatusCode, message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Private Methods

        #region IDisposible

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_httpClient != null)
                    _httpClient.Dispose();
            }
        }

        #endregion IDisposible
    }

    public class WebApiSettings
    {
        public string BaseUrl { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public WebApiSettings()
        {
            Headers = new Dictionary<string, string>();
        }
    }

    public enum HttpVerbs
    {
        GET,
        POST,
        PATCH,
        DELETE,
        PUT
    }
}