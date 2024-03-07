using Microsoft.AspNetCore.Mvc;
using mvcApp.Models;

namespace mvcApp.Controllers;

public class HomeController : Controller
{
    SharedController _sc;

    public HomeController(IConfiguration config)
    {
        _sc = new SharedController(config);
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<SearchPost> sposts = SharedController.GetSearchPosts("Hello", 1, true);
        return View(sposts);
    }

    // [HttpPost]
    // public IActionResult Index(string searchText, int pageNumber)
    // {
    //     string text = searchText;
    //     int pgNumber = pageNumber;
    //     Console.WriteLine(text);
    //     Console.WriteLine(pgNumber);
    //     List<SearchPost> sposts = SharedController.GetSearchPosts(text, pgNumber, true);
    //     return View(sposts);
    // }
}
