namespace GithubDL;

using Microsoft.Extensions.Configuration;
using Octokit;

public class GithubSearch
{
    private GitHubClient _client;
    private IConfiguration _configuration;

    public GithubSearch(IConfiguration configuration)
    {
        _configuration = configuration;
        _client = new GitHubClient(new ProductHeaderValue("IdiotsFirstGithubDownloader"))
        {
            Credentials = new Credentials("githubdlweb", _configuration["githublogin"], AuthenticationType.Basic)
        };
    }

    public async Task<SearchRepositoryResult> RunSearch(string searchText)
    {
            var request = new SearchRepositoriesRequest(searchText);
            return await _client.Search.SearchRepo(request);
    }

    public async Task<Release> GetLatestRelease(long repoId)
    {
            return await _client.Repository.Release.GetLatest(repoId);
    }
}
