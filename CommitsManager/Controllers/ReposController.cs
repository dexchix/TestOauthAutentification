using CommitsManager.Domain;
using CommitsManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleTalk_GitHubOAuth2.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommitsManager.Controllers
{
    public class ReposController : Controller
    {
        private readonly AppDBContext _context; 

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> LoadCommits(int page)
        {
            int pageSize = 20; // Количество коммитов на странице
            int skip = (page - 1) * pageSize; // Пропустить определенное количество коммитов

            var commits = await _context.Commits
                .OrderByDescending(c => c.DateCreate) // Сортировка коммитов по дате в обратном порядке
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return PartialView("_CommitsPartial", commits); // Возвращаем частичное представление с коммитами
        }
        [HttpPost]
        public async Task<IActionResult> DeleteSelectedCommits(int[] selectedItems)
        {
            var dbContext = new AppDBContext();
            if (selectedItems != null && selectedItems.Any())
            {
                foreach (var commitId in selectedItems)
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
            //return RedirectToPage("~/Pages/Index");
            return LocalRedirect("/");
        }
    }
}
