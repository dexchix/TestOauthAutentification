using CommitsManager.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CommitsManager.Controllers
{
    public class ReposController : Controller
    {
        private readonly AppDBContext _context = new AppDBContext(); 

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelectedCommits(int[] selectedItems)
        {
            var dbContext = new AppDBContext();
            if (selectedItems != null && selectedItems.Any())
            {
                foreach (var commitId in selectedItems)
                {
                    var commitToDelete = await dbContext.Commits.FindAsync(commitId);
                    if (commitToDelete != null)
                    {
                        dbContext.Commits.Remove(commitToDelete);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
            return LocalRedirect("/");
        }
    }
}
