using System;
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
            /// <summary>
            /// The unique ID of this item.
            /// </summary>
            public string Id { get; set; }
            
            /// <summary>
            /// The content of this item.
            /// </summary>
            public string Content { get; set; }

            /// <summary>
            /// The date and time when this item is created.
            /// </summary>
            public DateTime Created { get; set; }

            /// <summary>
            /// The ID of the user who created this item
            /// </summary>
            public string UserId { get; set; }

            /// <summary>
            /// The name of the user who created this item
            /// </summary>
            /// <value></value>
            public string UserName { get; set; }

            /// <summary>
            /// A list of image URLs if this item contains any images.
            /// </summary>
            public List<string> ImageUrls { get; set; } = new List<string>();

            /// <summary>
            /// A flag to indicate if this item is shared (or quoted, comment on other item)
            /// </summary>
            public bool Shared { get; set; }

            /// <summary>
            /// The unique ID of the original item if this item is shared.
            /// </summary>
            public string OriginalId { get; set; }
            
            /// <summary>
            /// The ID of a user who created the original item if this item is shared.
            /// </summary>
            public string OriginalUserId { get; set; }
        }
    }
}