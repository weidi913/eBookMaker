using FYP1.Data;
using FYP1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FYP1.Pages
{
    public class AboutModel : DI_BasePageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<Member> _userManager;
        private readonly IConfiguration Configuration;
        public AboutModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<Member> userManager,
                        IConfiguration configuration)
            : base(context, authorizationService, userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
            Configuration = configuration;
        }

        public Member curUser { get; set; } = default;

        public async Task OnGetAsync()
        {
            curUser = await _userManager.GetUserAsync(User);
        }
    }
}
