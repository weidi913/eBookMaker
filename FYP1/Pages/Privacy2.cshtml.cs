using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;

namespace FYP1.Pages
{
    [AllowAnonymous]
    public class PrivacyMode2l : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (MailMessage message = new MailMessage("corjackchin@1utar.my", "corjackchin@hotmail.com", "SubjectText", "BodyText"))
                {
                    using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                    {
                        client.EnableSsl = true;
                        client.Credentials = new System.Net.NetworkCredential("corjackchin@1utar.my", "spmf ywrk xehj fyjo");
                        await client.SendMailAsync(message);
                    }
                }

                return RedirectToPage("SuccessPage");
            }
            catch (Exception ex) 
            {
                                Console.WriteLine($"An error occurred: {ex.Message}");
                return Page(); // or return error view
            }
        }
    }
}