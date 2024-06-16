namespace GithubDL;

using Octokit;
public class GithubSearch
{
        private GitHubClient client = new GitHubClient(new ProductHeaderValue("IdiotsFirstGithubDownloader"));

        private async Task<SearchRepositoryResult> RunSearch(string searchText)
        {
                var request = new SearchRepositoriesRequest(searchText);
                return await client.Search.SearchRepo(request);
        }

        private async Task<Release> GetLatestRelease(long repoId)
        {
                return await client.Repository.Release.GetLatest(repoId);
        }
}
