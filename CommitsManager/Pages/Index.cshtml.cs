using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommitsManager.Domain;
using CommitsManager.Domain.Entities;
using CommitsManager.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;
using Octokit.Internal;

namespace CommitsManager.Pages
{
    public class IndexModel : PageModel
    {
        private AppDBContext _dbContext = new AppDBContext();
        private static long? _selectedRepository;
        

        [BindProperty]
        public MyFormData FormData { get; set; }

        public List<CommitEntity> Commits { get; set; }

        public void OnGet(int? page)
        {
            if (_selectedRepository != null)
            {
               
                Commits = _dbContext.Commits.Where(c => c.RepositoryId == _selectedRepository).ToList();
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
