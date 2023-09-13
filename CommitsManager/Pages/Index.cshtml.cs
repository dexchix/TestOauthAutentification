using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CommitsManager.Domain;
using CommitsManager.Domain.Entities;
using CommitsManager.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;
using Octokit.Internal;

namespace SimpleTalk_GitHubOAuth2.Pages
{
    public class IndexModel : PageModel
    {
        IMapper mapper;

        [BindProperty]
        public MyFormData FormData { get; set; }

        public IReadOnlyList<Repository> Repositories { get; set; }

        public IReadOnlyList<Repository> StarredRepos { get; set; }

        public IReadOnlyList<User> Followers { get; set; }

        public IReadOnlyList<User> Following { get; set; }

        public IndexModel()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            mapper = config.CreateMapper();
        }
        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");

                var github = new GitHubClient(new ProductHeaderValue("AspNetCoreGitHubAuth"), new InMemoryCredentialStore(new Credentials(accessToken)));

                var repos = await github.Repository.GetAllForCurrent();

                var commits = github.Repository.Commit.GetAll(repos[1].Id).Result;

                Repositories = repos;






                var startedReposit = await github.Activity.Starring.GetAllForCurrent();

                StarredRepos = startedReposit;



                Followers = await github.User.Followers.GetAllForCurrent();
                Following = await github.User.Followers.GetAllFollowingForCurrent();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                var owner = FormData.Owner;
                var repository = FormData.Repository;


                string accessToken = await HttpContext.GetTokenAsync("access_token");
                var github = new GitHubClient(new ProductHeaderValue("AspNetCoreGitHubAuth"), new InMemoryCredentialStore(new Credentials(accessToken)));

                var repos = await github.Repository.GetAllForCurrent();
                foreach(var repo in repos )
                {
                    if(repo.Owner.Login == owner && repo.Name == repository)
                    {
                        var commits = github.Repository.Commit.GetAll(repo.Id).Result;

                        var dbContext = new AppDBContext();

                        var repos1 = new RepositoryEntity() { Name = repo.Name, Owner = repo.Owner.Login };
                        
                    }
                }
            }
            return Page();
        }
    }
    public class MyFormData
    {
        public string Owner { get; set; }
        public string Repository { get; set; }
    }
}
