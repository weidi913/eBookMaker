using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FYP1.Pages
{
    [AllowAnonymous]
    public class TermModel : PageModel
    {
        private readonly ILogger<TermModel> _logger;

        public TermModel(ILogger<TermModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}