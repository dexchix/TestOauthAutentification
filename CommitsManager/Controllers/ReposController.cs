using CommitsManager.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CommitsManager.Controllers
{
    public class ReposController : Controller
    {
        private readonly AppDBContext _context; 

        public ReposController(AppDBContext context) 
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        //public async Task<IActionResult> LoadCommits(int page)
        //{
        //    int pageSize = 20; // Количество коммитов на странице
        //    int skip = (page - 1) * pageSize; // Пропустить определенное количество коммитов

        //    //var commits = await _context.Commits
        //    //    .OrderByDescending(c => c.CommitDate) // Сортировка коммитов по дате в обратном порядке
        //    //    .Skip(skip)
        //    //    .Take(pageSize)
        //    //    .ToListAsync();

        //    //return PartialView("_CommitsPartial", commits); // Возвращаем частичное представление с коммитами
        //}
    }
}
