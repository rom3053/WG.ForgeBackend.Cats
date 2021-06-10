using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WebApp7.Cats
{
    public class CatIoRateLimitConfiguration : RateLimitConfiguration
    {
        IHttpContextAccessor _httpContextAccessor;
        public CatIoRateLimitConfiguration(
            IHttpContextAccessor httpContextAccessor,
            IOptions<IpRateLimitOptions> ipOptions,
            IOptions<ClientRateLimitOptions> clientOptions)
                : base(ipOptions, clientOptions)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void RegisterResolvers()
        {
            base.RegisterResolvers();

            ClientResolvers.Add(new ClientQueryStringResolveContributor(_httpContextAccessor));
        }
    }

    public class ClientQueryStringResolveContributor : IClientResolveContributor
    {
        private IHttpContextAccessor httpContextAccessor;

        public ClientQueryStringResolveContributor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string ResolveClient()
        {
            var request = httpContextAccessor.HttpContext?.Request;
            var queryDictionary =
                Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(
                    request.QueryString.ToString());
            if (queryDictionary.ContainsKey("api_key")
                && !string.IsNullOrWhiteSpace(queryDictionary["api_key"]))
            {
                return queryDictionary["api_key"];
            }

            return Guid.NewGuid().ToString();
        }

        public Task<string> ResolveClientAsync(HttpContext httpContext)
        {
            return Task.Factory.StartNew(()=>ResolveClient());
        }

    }
}
