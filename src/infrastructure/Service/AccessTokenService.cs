using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace infrastructure.Service
{
    public class BearerTokenResponse : HttpResponseMessage
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
    public class AccessTokenService : IAccessTokenService
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AccessTokenService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<string> GetAccessTokenAsync()
        {

            var form = new Dictionary<string, string>
        {
                        {"grant_type", "client_credentials" },
                        {"client_id",  _configuration["Authentication:ClientId"] },
                        {"client_secret",  _configuration["Authentication:ClientSecret"] },
                         {"scope",  _configuration["Authentication:Scope"] }
        };
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Consumer-Key", _configuration["Authentication:ClientId"]);
            var tokenResponse = client.PostAsync(_configuration["Authentication:AccessTokenUrl"],
                new FormUrlEncodedContent(form)).Result;
            tokenResponse.EnsureSuccessStatusCode();
            var token = await tokenResponse.Content.ReadFromJsonAsync<BearerTokenResponse>();
            return token.AccessToken;
        }
    }
}
