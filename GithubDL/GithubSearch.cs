namespace GithubDL;

using Octokit;
public class GithubSearch
{
        private GitHubClient client = new GitHubClient(new ProductHeaderValue("IdiotsFirstGithubDownloader"));

        public async Task<SearchRepositoryResult> RunSearch(string searchText)
        {
                var request = new SearchRepositoriesRequest(searchText);
                return await client.Search.SearchRepo(request);
        }

        public async Task<Release> GetLatestRelease(long repoId)
        {
                return await client.Repository.Release.GetLatest(repoId);
        }
}
