using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;

using HeySearch.Services;
using HeySearch.Models;

namespace HeySearch.Controllers
{
    /// <summary>
    /// A controller to resonpond a set of search result in JSON.
    /// </summary>
    [Produces("application/json")]
    [Route("api/twitter")]
    public class SocialNetworkJsonController : Controller
    {
        private readonly ISocialNetworkSearchService _searchService;
        
        /// <summary>
        /// Constructs SocialNetworkJsonController using the given ISocialNetworkSearchService
        /// </summary>
        /// <param name="service">the service that this controlle should use</param> 
        public SocialNetworkJsonController(ISocialNetworkSearchService service)  
        {
            _searchService = service;
        }

#nullable enable
        /// <summary>
        /// An action to serve HTTP GET request.
        /// </summary>
        /// <param name="q">search keywords</param>
        /// <param name="io">image only option (null or empty means this option is disabled)</param>
        /// <param name="oo">original only option (null or empty means this option is disabled)</param>
        /// <param name="nt">next token or URL for pagination</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<SearchResult>> Get(string q, string? io, string? oo, string? nt)
        {
            if(string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(); 
            }

            var options = new Dictionary<ISocialNetworkSearchService.OPTIONS, string>();
            if(!string.IsNullOrEmpty(io))
            {
                options.Add(ISocialNetworkSearchService.OPTIONS.IMAGE_ONLY,"");
            }
            if(!string.IsNullOrEmpty(oo))
            {
                options.Add(ISocialNetworkSearchService.OPTIONS.ORIGINAL_ONLY,"");    
            }
            if(!string.IsNullOrEmpty(nt))
            {
                options.Add(ISocialNetworkSearchService.OPTIONS.NEXT_PAGE_TOKEN, nt);
            }

            SearchResult result = await _searchService.Search(q, options);

            if(result==null) 
            {
                return NotFound();
            }
            else 
            {
                return result;
            }
        }
#nullable disable
    }
}