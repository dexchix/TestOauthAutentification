using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;
using Octokit.Internal;

namespace SimpleTalk_GitHubOAuth2.Pages
{
    public class IndexModel : PageModel
    {
        public IReadOnlyList<Repository> Repositories { get; set; }

        public IReadOnlyList<Repository> StarredRepos { get; set; }

        public IReadOnlyList<User> Followers { get; set; }

        public IReadOnlyList<User> Following { get; set; }

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");

                var github = new GitHubClient(new ProductHeaderValue("AspNetCoreGitHubAuth"), new InMemoryCredentialStore(new Credentials(accessToken)));

                var repos= await github.Repository.GetAllForCurrent();

                var commits = github.Repository.Commit.GetAll(repos[1].Id).Result;

                Repositories = repos;

              




                var startedReposit = await github.Activity.Starring.GetAllForCurrent();

                StarredRepos = startedReposit;

                

                Followers = await github.User.Followers.GetAllForCurrent();
                Following = await github.User.Followers.GetAllFollowingForCurrent();
            }
        }
    }
}
