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

        [HttpGet]
        public async Task<IEnumerable<SearchResult>> Get()
        {
            return await _searchService.Search("Aarhus");
        }

    }
}