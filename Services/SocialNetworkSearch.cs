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
        /// Searches contents from a social network using the given search term.
        /// This interface will run asynchronously.
        /// </summary>
        /// <param name="q">
        /// search term used to search contents in social network.
        /// </param>
        /// <returns>
        /// a list of SearchResult
        /// </returns>
        Task<List<SearchResult>> Search(string q);
    }
}