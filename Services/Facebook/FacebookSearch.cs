using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace HeySearch.Services.Facebook
{
    public class FacebookSearch //: ISocialNetworkSearchService 
    {
        private readonly IHttpClientFactory _httpClientFactory;


        public FacebookSearch(IHttpClientFactory factory) 
        {
            _httpClientFactory = factory;
        }
        
        public async Task<List<string>> Search(string q) 
        {
            var ret = new List<string>();

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "https://graph.facebook.com/v7.0/me/feed?access_token=EAAE4NXjuBZBMBAEOMo3NmV5R1MOUQS5y54HPsjvF0AoZA8lDls6MTmHTXZAfEaZCdkoodmTLcbBdUUaa7mq9eWxeDRRDUZAjFeoKEc4g5XDUV4lZCUjcbWob4GhF1EheCXXfdLmCzMZBFTigB9ZBxXDbIu5adPI0Hmf99dDzc1B9GxDZBMdSP2bFIBcJf2Kyk2qqZAcJl1kM12y776qSMDsAbjqZACMPTIIpiipx5koglZAZBXgZDZD"
            );
            request.Headers.Add("Accept", "application/json");
            
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                /*SearchResult res = await JsonSerializer.DeserializeAsync<SearchResult>(responseStream);
                for (int i=0; i<res.data.Count; i++)
                {
                    ret.Add(res.data[i].message);
                }*/
            }
        
            return ret;
        }
    }
}