using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using HeySearch.Models;

namespace HeySearch.Services
{
    /// <summary>
    /// A set of interfaces to make a search in a Social Network. 
    /// </summary>
    public interface ISocialNetworkSearchService
    {

        /// <summary>
        /// A set of options available for ISocialNetworkSearchService.
        /// Implementation of ISocialNetworkSearchService must support those options.
        /// </summary>
        public enum OPTIONS
        {
            IMAGE_ONLY,
            ORIGINAL_ONLY,
            NEXT_PAGE_TOKEN,
        }

        /// <summary>
        /// Searches contents from a social network using the given search term.
        /// The searchTerm will be sanitized within this functin.
        /// This interface will run asynchronously.
        /// </summary>
        /// <param name="searchTerm">
        /// search term used to search contents in social network.
        /// </param>
        /// <param name="options">
        /// a key-value paired options for the search. 
        /// See <see cref="ISocialNetworkSearchService.OPTIONS" >OPTIONS</see> for availabel options.
        /// </param>
        /// <returns>
        /// SearchResult which contains the search results 
        /// </returns>
        Task<SearchResult> Search(string searchTerm, Dictionary<OPTIONS, string> options = null);
    }
}