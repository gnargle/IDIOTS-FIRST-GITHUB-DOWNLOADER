using Octokit;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IDIOTS_FIRST_GITHUB_DOWNLOADER
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GitHubClient client = new GitHubClient(new ProductHeaderValue("IdiotsFirstGithubDownloader"));
        public MainWindow()
        {
            InitializeComponent();

        }
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }


        private async Task RunSearch()
        {
            spinner.Visibility = Visibility.Visible;
            try
            {
                var request = new SearchRepositoriesRequest(repoTxt.Text);
                var result = await client.Search.SearchRepo(request);
                int end = result.TotalCount > 10 ? 10 : result.TotalCount;
                for (int i = 0; i < end; i++)
                {
                    var repo = result.Items[i];
                    try
                    {
                        var rel = await client.Repository.Release.GetLatest(repo.Id);
                        foreach (var asset in rel.Assets)
                        {
                            var link = new Button();
                            link.Content = $"{repo.FullName}\n{asset.Name}";
                            link.FontSize = 16;
                            link.Background = Brushes.Red;
                            link.Foreground = Brushes.White;
                            link.Height = 50;
                            resultsPanel.Children.Add(link);
                            void handler(object sender, EventArgs args)
                            {
                                OpenUrl(asset.BrowserDownloadUrl);
                            };

                            link.Click += handler;
                        }
                    }
                    catch (NotFoundException)
                    {
                        //no release, ignore.
                    }
                }
            } finally { spinner.Visibility = Visibility.Collapsed;}
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await RunSearch();
        }

        private async void repoTxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await RunSearch();
            }
        }
    }
}