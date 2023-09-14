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


        public List<CommitEntity> Commits { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int PageCount { get; set; }

        public void OnGet(int? page)
        {
            var dbContext = new AppDBContext();

            CurrentPage = page ?? 1;

            int skip = (CurrentPage - 1) * PageSize;
            Commits = dbContext.Commits.Skip(skip).Take(PageSize).ToList();

            int totalCommits = dbContext.Commits.Count();
            PageCount = (int)Math.Ceiling((double)totalCommits / PageSize);
        }

        //public async Task OnGetAsync()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        string accessToken = await HttpContext.GetTokenAsync("access_token");

        //        var github = new GitHubClient(new ProductHeaderValue("AspNetCoreGitHubAuth"), new InMemoryCredentialStore(new Credentials(accessToken)));

        //        var repos = await github.Repository.GetAllForCurrent();

        //        var commits = github.Repository.Commit.GetAll(repos[1].Id).Result;

        //        Repositories = repos;






        //        var startedReposit = await github.Activity.Starring.GetAllForCurrent();

        //        StarredRepos = startedReposit;



        //        Followers = await github.User.Followers.GetAllForCurrent();
        //        Following = await github.User.Followers.GetAllFollowingForCurrent();
        //    }
        //}
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid && User.Identity.IsAuthenticated)
            {
                var owner = FormData.Owner;
                var repository = FormData.Repository;


                string accessToken = await HttpContext.GetTokenAsync("access_token");
                var github = new GitHubClient(new ProductHeaderValue("AspNetCoreGitHubAuth"), new InMemoryCredentialStore(new Credentials(accessToken)));

                var repos = await github.Repository.GetAllForCurrent();
                foreach (var repo in repos)
                {
                    if (repo.Owner.Login == owner && repo.Name == repository)
                    {
                        var commits = github.Repository.Commit.GetAll(repo.Id).Result;

                        var repoEntity = new RepositoryEntity() { Name = repo.Name, Owner = repo.Owner.Login };
                        var commitsEntity = EfConverter.ConvertCommits(commits, repoEntity);

                        var dbContext = new AppDBContext();

                        foreach (var commit in commitsEntity)
                        {
                            dbContext.Commits.Add(commit);
                        }
                        dbContext.Repositories.Add(repoEntity);

                        dbContext.SaveChanges();

                        var commitsForRepository = dbContext.Commits.Where(c => c.RepositoryId == repoEntity.Id).ToList();

                    }
                }
            }
            return Page();
        }
        public async Task<IActionResult> DeleteSelectedCommits(List<string> selectedCommits)
        {
            var dbContext = new AppDBContext();
            if (selectedCommits != null && selectedCommits.Any())
            {
                foreach (var commitId in selectedCommits)
                {
                    // Здесь реализуйте удаление коммита из вашей базы данных, используя Entity Framework или другой ORM.
                    // Пример:
                    var commitToDelete = await dbContext.Commits.FindAsync(commitId);
                    if (commitToDelete != null)
                    {
                        dbContext.Commits.Remove(commitToDelete);
                    }
                }

                await dbContext.SaveChangesAsync();
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
