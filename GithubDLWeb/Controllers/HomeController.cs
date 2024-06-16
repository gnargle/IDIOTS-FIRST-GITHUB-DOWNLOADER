using GithubDL;
using GithubDLWeb.Models;
using Octokit;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GithubDLWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GithubSearch _search;

        public HomeController(ILogger<HomeController> logger, GithubSearch search)
        {
            _logger = logger;
            _search = search;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult<SearchResult>> Search([FromQuery] string searchString)
        {
            if (String.IsNullOrWhiteSpace(searchString))
                return RedirectToAction("Index");
            if (searchString.Contains("http"))
            {
                //user has pasted url, parse that shit
                //sample: https://github.com/gnargle/IDIOTS-FIRST-GITHUB-DOWNLOADER
                var uri = new Uri(searchString);
                searchString = uri.PathAndQuery;
            }

            var model = new SearchResult();
            var result = await _search.RunSearch(searchString);
            int end = result.TotalCount > 10 ? 10 : result.TotalCount;
            for (int i = 0; i < end; i++)
            {
                var repo = result.Items[i];
                try
                {
                    var rel = await _search.GetLatestRelease(repo.Id);
                    foreach (var asset in rel.Assets)
                    {
                        model.Results.Add(new ResultEntry()
                        {
                            RepoName = $"({repo.FullName}) {asset.Name}",
                            DownloadURL = asset.BrowserDownloadUrl
                        });
                    }
                }
                catch (NotFoundException)
                {
                    //no release, ignore.
                }
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
