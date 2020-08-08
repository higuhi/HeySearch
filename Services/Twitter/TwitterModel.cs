using System.Collections.Generic;

namespace HeySearch.Services.Twitter
{

    ///<summary>
    ///Model representation of the JSON response from the Twitter Search API.
    ///</summary>
    ///<seealso cref="https://developer.twitter.com/en/docs/labs/recent-search/api-reference/get-recent-search"/>
    public class TwitterResponse
    {
        public IList<Tweet> data { get; set; }
        public Meta meta { get; set; }

        public class Tweet
        {
            public string id { get; set; }
            public string text { get; set; }
        }

        public class Meta
        {
            public string newest_id { get; set; }
            public string oldest_id { get; set; }
            public int result_count { get; set; }
            public string next_token { get; set; }
        }
    }
}