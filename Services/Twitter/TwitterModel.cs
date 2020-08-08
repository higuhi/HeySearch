using System;
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
        public Includes includes { get; set; }
        public Meta meta { get; set; }

        public class Tweet
        {
            public string id { get; set; }
            public string author_id { get; set; }
            public string text { get; set; }
            public DateTime created_at { get; set; }
            public Attachments attachments { get; set; }

            public IList<Reference> referenced_tweets { get; set; }
        }

        public class Attachments
        {
            public IList<string> media_keys { get; set; }
        }

        public class Reference
        {
            public string type { get; set; }
            public string id { get; set; }
        }

        public class Includes
        {
            public IList<User> users { get; set; }
            public IList<Media> media { get; set; }
        }

        public class User
        {
            public string username { get; set; }
            public string name { get; set; }
            public string id { get; set; }
        }

        public class Media
        {
            public string media_key { get; set; }
            public string type { get; set; }
            public string url { get; set; }
            public string preview_image_url { get; set; }
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