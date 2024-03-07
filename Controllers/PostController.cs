using Microsoft.AspNetCore.Mvc;
using mvcApp.Models;

namespace mvcApp.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : Controller
{
    SharedController _sc;

    public PostController(IConfiguration config)
    {
        _sc = new SharedController(config);
    }

    [HttpPost("getposts")]
    public List<SearchPost> GetPosts(SearchRequest searchRequest)
    {
        string text = searchRequest.searchText;
        int pgNumber = searchRequest.pageNumber;
        List<SearchPost> sposts = SharedController.GetSearchPosts(text, pgNumber, false);

        return sposts;
    }
}
