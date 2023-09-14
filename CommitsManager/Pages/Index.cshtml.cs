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
        private AppDBContext _dbContext = new AppDBContext();
        private static long? _selectedRepository;
        

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

        public async void OnGet(int? page)
        {
            //var dbContext = new AppDBContext();

            //CurrentPage = page ?? 1;

            //var commitsForRepository = _dbContext.Commits.Where(c => c.RepositoryId == repoEntity.Id).ToList();
            //int skip = (CurrentPage - 1) * PageSize;
            //Commits = dbContext.Commits.Skip(skip).Take(PageSize).ToList();

            //int totalCommits = dbContext.Commits.Count();
            //PageCount = (int)Math.Ceiling((double)totalCommits / PageSize);

            if (_selectedRepository != null)
            {
                CurrentPage = page ?? 1;
                int skip = (CurrentPage - 1) * PageSize;
                Commits = _dbContext.Commits.Where(c => c.RepositoryId == _selectedRepository)
                                           .Skip(skip)
                                           .Take(PageSize)
                                           .ToList();

                int totalCommits = _dbContext.Commits.Count(c => c.RepositoryId == _selectedRepository);
                PageCount = (int)Math.Ceiling((double)totalCommits / PageSize);
            }
            else
            {
                Commits = new List<CommitEntity>();
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

                var repo = repos.FirstOrDefault(x => x.Owner.Login == owner && x.Name == repository);
                var repoExistInDB = _dbContext.Repositories.Any(x => x.Name == repository);

                if (repo != null && !repoExistInDB)
                {
                    var commits = github.Repository.Commit.GetAll(repo.Id).Result;

                    var repoEntity = new RepositoryEntity() { Name = repo.Name, Owner = repo.Owner.Login };
                    var commitsEntity = EfConverter.ConvertCommits(commits, repoEntity);

                    foreach (var commit in commitsEntity)
                    {
                        _dbContext.Commits.Add(commit);
                    }
                    _dbContext.Repositories.Add(repoEntity);

                    _dbContext.SaveChanges();

                    //var commitsForRepository = _dbContext.Commits.Where(c => c.RepositoryId == repoEntity.Id).ToList();
                    _selectedRepository = repoEntity.Id;
                }
                else if (repo != null && repoExistInDB)
                {
                    _selectedRepository = _dbContext.Repositories.FirstOrDefault(x=>x.Name == repository).Id;
                }
                else
                    _selectedRepository= null;
            }
            return LocalRedirect("/");
        }
    }
    public class MyFormData
    {
        public string Owner { get; set; }
        public string Repository { get; set; }
    }
}
