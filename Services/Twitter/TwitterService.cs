using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Text.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

using HeySearch.Services;
using HeySearch.Models;

namespace HeySearch.Services.Twitter
{
    /// <summary>
    /// Implementation of ISocialNetworkSearchService for Twitter
    /// </summary>
    /// <see cref="HeySearch.Services.ISocialNetworkSearchService" />
    public class TwitterSearch : ISocialNetworkSearchService 
    {
        private const string TWITTER_API_SEARCH = "https://api.twitter.com/labs/2/tweets/search";

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly string _bearerToken;

        /// <param name="factory">
        /// factory class to create HTTPClient 
        /// </param>
        /// <param name="config">
        /// application configuration which must have the following values in HeySearch/Twitter section.
        /// <list type="bullet">
        /// <item><description>BearerToken - Token for Twitter API</description></item>
        /// </list>
        /// </param>
        public TwitterSearch(IHttpClientFactory factory, IConfiguration config) 
        {
            _httpClientFactory = factory;

            var token = config.GetSection("HeySearch").GetSection("Twitter").GetValue<string>("BearerToken");
            if(token==null) 
            {
                throw new System.Exception("Configuration Missing: BearerToken not found in HeySearch section");  
            }
            _bearerToken = "Bearer " + token;
        }

        ///
        public async Task<List<SearchResult>> Search(string searchTerm) 
        {
            var ret = new List<SearchResult>(); 

            // Build the URL for Twitter API. Query strings will be sanitized
            var builder = new UriBuilder(TWITTER_API_SEARCH);
            builder.Port = -1; //use default port

            var query = HttpUtility.ParseQueryString(builder.Query);
            query["query"] = searchTerm;
            builder.Query = query.ToString();

            Console.WriteLine($"TwitterSearch#Search: {builder.ToString()}");

            // Build HTTP Request with authorization header
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                builder.ToString()
            );
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("authorization", _bearerToken);

            // Hit the Twitter API and convert the response into the search result
            var response = await _httpClientFactory.CreateClient().SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                //var dump = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(dump);
                var responseStream = await response.Content.ReadAsStreamAsync();
                TwitterResponse tweets = await JsonSerializer.DeserializeAsync<TwitterResponse>(responseStream);
                if(tweets.data!=null) 
                {
                    for (int i=0; i<tweets.data.Count; i++)
                    {
                        SearchResult r = new SearchResult();
                        r.Content = tweets.data[i].text;
                        r.Id = tweets.data[i].id;
                        ret.Add(r);
                    }
                }
            }

            return ret;
        }
    }
}