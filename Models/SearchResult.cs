using System.Collections.Generic;

namespace HeySearch.Models
{
    /// <summary>
    /// A generic model for search result from a Social Network.
    /// </summary>
    public class SearchResult 
    {
        /// <summary>
        /// When there is a next page (next set of search results) available, 
        /// this field will have the token or URL to access the next page. 
        /// </summary>
        public string NextPageToken { get; set; }

        /// <summary>
        /// A list of items from the search result.
        /// </summary>
        public List<SearchResultItem> Items { get; set; } = new List<SearchResultItem>();

        public class SearchResultItem 
        {

            public string Id { get; set; }

            public string Content { get; set; }

            public string UserId { get; set; }

            public string UserName { get; set; }

            public List<string> ImageUrls { get; set; } = new List<string>();

            public bool Shared { get; set; }

            public string OriginalId { get; set; }
        }
    }
}