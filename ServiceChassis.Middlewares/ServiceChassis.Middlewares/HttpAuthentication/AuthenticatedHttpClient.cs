using Newtonsoft.Json;
using ServiceChassis.Middlewares.HttpAuthentication.Exceptions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServiceChassis.Middlewares.HttpAuthentication
{
    /// <summary>
    /// Abstract HTTP client implementation which wraps each call with a X-Access-Token, and performs automatic refresh
    /// </summary>
    public abstract class AuthenticatedHttpClient
    {
        private readonly bool automaticRefresh;

        protected readonly string baseUrl;
        protected readonly HttpClient httpClient;

        private string accessToken;
        private string refreshToken;
        private string email;

        protected AuthenticatedHttpClient(string baseUrl, bool automaticRefresh)
        {
            this.baseUrl = baseUrl;
            this.automaticRefresh = automaticRefresh;
            this.httpClient = new HttpClient();
        }

        protected abstract HttpContent CreateRefreshPayload(string refreshToken);
        protected abstract (string accessToken, string refreshToken) DeserializeRefreshResult(HttpResponseMessage httpResponseMessage);

        protected void SetEmail(string email)
        {
            this.email = email;
        }

        protected void SetSecurityTokens(string accessToken, string refreshToken)
        {
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;

            if (httpClient.DefaultRequestHeaders.Contains("X-Access-Token"))
                httpClient.DefaultRequestHeaders.Remove("X-Access-Token");

            httpClient.DefaultRequestHeaders.Add("X-Access-Token", this.accessToken);
        }

        protected T Get<T>(string url)
        {
            var httpResult = httpClient.GetAsync(url).Result;

            try
            {
                VerifyResult(httpResult.StatusCode);
            }
            catch (InvalidCredentialsException)
            {
                if (!automaticRefresh)
                    throw;

                Refresh();

                httpResult = httpClient.GetAsync(url).Result;
                VerifyResult(httpResult.StatusCode);
            }

            var result = DeserializeResponse<T>(httpResult).GetAwaiter().GetResult();
            return result;
        }

        protected void Post(string url, object payload)
        {
            var result = httpClient.PostAsync(url, CreateJsonPayload(payload)).GetAwaiter().GetResult();

            try
            {
                VerifyResult(result.StatusCode);
            }
            catch (InvalidCredentialsException)
            {
                if (!automaticRefresh)
                    throw;

                Refresh();

                result = httpClient.PostAsync(url, CreateJsonPayload(payload)).GetAwaiter().GetResult();
                VerifyResult(result.StatusCode);
            }
        }

        protected void PostForm(string url, MultipartFormDataContent form)
        {
            var result = httpClient.PostAsync(url, form).GetAwaiter().GetResult();

            try
            {
                VerifyResult(result.StatusCode);
            }
            catch (InvalidCredentialsException)
            {
                if (!automaticRefresh)
                    throw;

                Refresh();

                result = httpClient.PostAsync(url, form).GetAwaiter().GetResult();
                VerifyResult(result.StatusCode);
            }
        }

        protected T Post<T>(string url, object payload)
        {
            var httpResult = httpClient.PostAsync(url, CreateJsonPayload(payload)).GetAwaiter().GetResult();
            
            try
            {
                VerifyResult(httpResult.StatusCode);
            }
            catch (InvalidCredentialsException)
            {
                if (!automaticRefresh)
                    throw;

                Refresh();

                httpResult = httpClient.PostAsync(url, CreateJsonPayload(payload)).GetAwaiter().GetResult();
                VerifyResult(httpResult.StatusCode);
            }

            var result = DeserializeResponse<T>(httpResult).GetAwaiter().GetResult();
            return result;
        }

        protected void Put(string url, object payload)
        {
            var result = httpClient.PutAsync(url, CreateJsonPayload(payload)).GetAwaiter().GetResult();
            try
            {
                VerifyResult(result.StatusCode);
            }
            catch (InvalidCredentialsException)
            {
                if (!automaticRefresh)
                    throw;

                Refresh();

                result = httpClient.PutAsync(url, CreateJsonPayload(payload)).GetAwaiter().GetResult();
                VerifyResult(result.StatusCode);
            }
        }

        protected void Delete(string url)
        {
            var result = httpClient.DeleteAsync(url).GetAwaiter().GetResult();
            try
            {
                VerifyResult(result.StatusCode);
            }
            catch (InvalidCredentialsException)
            {
                if (!automaticRefresh)
                    throw;

                Refresh();

                result = httpClient.DeleteAsync(url).GetAwaiter().GetResult();
                VerifyResult(result.StatusCode);
            }
        }

        private void Refresh()
        {
            var httpResult = httpClient.PostAsync($"{baseUrl}/refresh/{email}", CreateRefreshPayload(this.refreshToken)).Result;
            VerifyResult(httpResult.StatusCode);

            (var accessToken, var refreshToken) = DeserializeRefreshResult(httpResult);

            SetSecurityTokens(accessToken, refreshToken);
        }

        protected virtual void VerifyResult(HttpStatusCode httpStatusCode)
        {
            if (httpStatusCode == HttpStatusCode.Unauthorized)
                throw new InvalidCredentialsException();
        }

        protected StringContent CreateJsonPayload(object content)
        {
            return new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
        }

        protected async Task<TResponsePayload> DeserializeResponse<TResponsePayload>(HttpResponseMessage responseMessage)
        {
            VerifyResult(responseMessage.StatusCode);

            var stringResponse = await responseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponsePayload>(stringResponse);
        }
    }
}
