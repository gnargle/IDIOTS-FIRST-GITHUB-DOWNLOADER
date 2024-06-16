namespace GithubDLWeb.Models
{
    public class ResultEntry
    {
        public String RepoName { get; set; }
        public String DownloadURL { get; set; }
    }
    public class SearchResult
    {
        public List<ResultEntry> Results { get; set; }
        public SearchResult()
        {
            Results = new List<ResultEntry>();
        }
    }
}
