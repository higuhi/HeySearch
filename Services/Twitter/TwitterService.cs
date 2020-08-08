using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        /// <summary>
        /// Twitter implementation of Search interface.
        /// </summary>
        /// <param name="searchTerm">keywords to search recent tweets </param>
        /// <param name="options">a set of key-value pairs for search options</param>
        /// <returns>SearchResult object from the serach</returns>
        public async Task<SearchResult> Search(string searchTerm, Dictionary<ISocialNetworkSearchService.OPTIONS,string> options = null) 
        {
            const string TWITTER_API_SEARCH = "https://api.twitter.com/labs/2/tweets/search";

            // Build the URL for Twitter API. Query strings will be sanitized
            var builder = new UriBuilder(TWITTER_API_SEARCH);
            builder.Port = -1; //use default port

            var query = HttpUtility.ParseQueryString(builder.Query);
            query["query"] = searchTerm;
            query["tweet.fields"] = "id,text,author_id,attachments,referenced_tweets";
            query["expansions"]   = "attachments.media_keys,author_id";
            query["media.fields"] = "media_key,preview_image_url,type,url";
            query["user.fields"]  = "id,name";
            
            buildQueryFromOptions(query, options);

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
                return ToSearchResult(tweets);
            }
            else 
            {
                return null;
            }
        }

        /// <summary>
        /// From the key-value dictionary, builds a query for Twitter API.
        /// </summary>
        /// <param name="query">the query object ot be updated</param>
        /// <param name="options">a set of key-value options</param>
        /// <seealso cref="ISocialNetworkSearchService.OPTIONS">
        private void buildQueryFromOptions(NameValueCollection query, Dictionary<ISocialNetworkSearchService.OPTIONS,string> options) 
        {
            if(options==null) return;

            if(options.ContainsKey(ISocialNetworkSearchService.OPTIONS.IMAGE_ONLY)) 
            {
                query["query"] = query["query"] + " has:images";
            }

            if(options.ContainsKey(ISocialNetworkSearchService.OPTIONS.ORIGINAL_ONLY))
            {
                query["query"] = query["query"] + " -is:retweet";
            }

            if(options.ContainsKey(ISocialNetworkSearchService.OPTIONS.NEXT_PAGE_TOKEN))
            {
                query["next_token"] = options[ISocialNetworkSearchService.OPTIONS.NEXT_PAGE_TOKEN];
            }
        }

        /// <summary>
        /// Converts the TwitterResponse to a generic SearchResult
        /// </summary>
        /// <param name="tweets">TwitterResponse to be cnverted</param>
        /// <returns>SearchResult object (or null if tweets is null or has no data inside)</returns>
        private SearchResult ToSearchResult(TwitterResponse tweets) 
        {
            if(tweets==null) return null;
            if(tweets.data==null) return null;

            // Since detailed information on users and media attachments are 
            // seperated from the tweet data, we will use dictironary to lookup 
            // user and media attachment details. 
            var users = new Dictionary<string, TwitterResponse.User>();
            var media = new Dictionary<string, TwitterResponse.Media>();

            if(tweets.includes!=null) 
            {
                // make a lookup table for the user
                if(tweets.includes.users!=null) 
                {
                    foreach(var u in tweets.includes.users) 
                    {
                        if(!users.TryAdd(u.id, u)) 
                        {
                            Console.WriteLine($"WARN: {u.id} was added multiple times.");
                        }
                    }
                }

                // make a lookup table for media attachment 
                if(tweets.includes.media!=null)
                {
                    foreach(var m in tweets.includes.media) 
                    {
                        if(!media.TryAdd(m.media_key, m)) 
                        {
                            Console.WriteLine($"WARN: {m.media_key} was added multiple times.");
                        }
                    }
                }
            }

            // Let's convert TwitterResponse to more generic SearchResult
            SearchResult result = new SearchResult();
            result.NextPageToken = tweets.meta.next_token;
            foreach(var tweetData in tweets.data)
            {
                SearchResult.SearchResultItem item = new SearchResult.SearchResultItem();
                item.Id = tweetData.id;
                item.Content = tweetData.text;
                item.UserId = tweetData.author_id;

                //look up the username from the dictionary
                try 
                {
                    item.UserName = users[item.UserId].username;
                } 
                catch (KeyNotFoundException) 
                {
                    Console.WriteLine($"WARN: {item.UserId} does not exist in the user extension.");
                    item.UserName = "";
                }
                
                //look up media attachement if there are any attachment
                if(tweetData.attachments!=null) 
                {
                    foreach(var key in tweetData.attachments.media_keys) 
                    {
                        try 
                        {
                            if(!string.IsNullOrEmpty(media[key].url))
                            {
                                //'photo' type should have URL 
                                item.ImageUrls.Add(media[key].url);
                            }
                            else 
                            {
                                //'animated_gif' type has preview_image_url
                                item.ImageUrls.Add(media[key].preview_image_url);
                            }
                        }
                        catch(KeyNotFoundException) 
                        {
                            Console.WriteLine($"WARN: {key} does not exist in the media extension.");
                        }
                    }
                }

                //If this is a retweet, then look for the original tweet ID.
                //There could be multiple referenced IDs. We will use the smallest ID,
                //assuming it is the original tweet. 
                if(tweetData.referenced_tweets!=null) 
                {
                    item.Shared = true; // this item is shared 
                    
                    ulong min = ulong.MaxValue; // Twitter ID is unsigned 62 bits. 
                    foreach(var reference in tweetData.referenced_tweets) 
                    {
                        ulong id = ulong.Parse(reference.id);
                        if(id<min) 
                        {
                            min = id;
                            item.OriginalId =  reference.id;
                        }   
                    }                    
                }

                result.Items.Add(item);
            }
            
            return result;
        }
    }
}