using Microsoft.AspNetCore.Mvc;

namespace Nongzhsh.JobHub.Pages
{
    public class IndexModel : JobHubPageModelBase
    {
        public IActionResult OnGet()
        {
            return RedirectToPage("/JobHub/Index");
        }
    }
}