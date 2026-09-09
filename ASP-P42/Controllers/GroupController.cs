using ASP_P42.Data;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P42.Controllers
{
    public class GroupController(DataContext dataContext) : Controller
    {
        private readonly DataContext _dataContext = dataContext;

        [HttpGet]
        public IActionResult Index()
        {
            var groups = _dataContext.ProductGroups
                .Where(g => g.IsHidden == 0)
                .ToList();

            return View(groups);
        }
    }
}
