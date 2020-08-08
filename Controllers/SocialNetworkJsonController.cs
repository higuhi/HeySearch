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
    [Route("api")]
    public class SocialNetworkJsonController : Controller
    {
        private readonly ISocialNetworkSearchService _searchService;
        
        public SocialNetworkJsonController(ISocialNetworkSearchService service)  
        {
            _searchService = service;
        }

#nullable enable
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