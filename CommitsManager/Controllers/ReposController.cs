using Microsoft.AspNetCore.Mvc;

namespace CommitsManager.Controllers
{
    public class ReposController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public void GetCommits()
        {

        }
    }
}
