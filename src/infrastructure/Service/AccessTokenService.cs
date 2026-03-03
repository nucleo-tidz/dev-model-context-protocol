using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Service
{
    public class BearerTokenResponse : HttpResponseMessage
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
    public class AccessTokenService: IAccessTokenService
    {
        private const string TokenCacheKey = "BearerToken";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(20);
        private static readonly SemaphoreSlim Semaphore = new(1, 1);
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;

        public AccessTokenService( IHttpClientFactory httpClientFactory,IConfiguration configuration, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            if (_memoryCache.TryGetValue<string>(TokenCacheKey, out var cachedToken))
            {
                return cachedToken;
            }

            await Semaphore.WaitAsync(cancellationToken);
            try
            {
                if (_memoryCache.TryGetValue<string>(TokenCacheKey, out cachedToken))
                {
                    return cachedToken;
                }

                var token = await GetToken();
                CacheToken(token);
                return token;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task<string> GetToken()
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
        private void CacheToken(string token)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheDuration);

            _memoryCache.Set(TokenCacheKey, token, cacheEntryOptions);
        }
    }
}
