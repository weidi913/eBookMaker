﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using FYP1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Net.Mail;

namespace FYP1.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Member> _signInManager;
        private readonly UserManager<Member> _userManager;
        private readonly IUserStore<Member> _userStore;
        private readonly IUserEmailStore<Member> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<Member> userManager,
            IUserStore<Member> userStore,
            SignInManager<Member> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }
        public bool errorDetected = false;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime BirthdayDate { get; set; }
            public string Gender { get; set; }
            public bool Term { get; set; }
            public bool Privacy { get; set; }
        }

        public async Task<IActionResult> OnGetCheckEmailExist(string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            return new JsonResult(user != null);
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            errorDetected = false;
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.birthday = Input.BirthdayDate;
                user.gender = Input.Gender;
                user.firstName = Input.FirstName;
                user.lastName = Input.LastName;
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    /*                        var user2 = await _userManager.FindByIdAsync(userId);
                    */
                    await _userManager.AddToRoleAsync(user, FYP1.Authorization.Constants.MemberRole);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        var confirmationReturnUrl = "~/Identity/Account/Login"; // You need to set returnUrl appropriately
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = confirmationReturnUrl },
                            protocol: Request.Scheme
                        );

                        try
                        {
                            using (MailMessage message = new MailMessage(
                                "corjackchin@1utar.my", //Sender
                                user.Email, //Receiver
                                "Confirm Your Email", // Title
                                "Please confirm your email by clicking <a href='" + HtmlEncoder.Default.Encode(callbackUrl) + "'>here</a>.")) //Content
                            {
                                message.IsBodyHtml = true; // Set IsBodyHtml to true to indicate that the message body contains HTML

                                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                                {
                                    client.EnableSsl = true;
                                    client.Credentials = new System.Net.NetworkCredential("corjackchin@1utar.my", "spmf ywrk xehj fyjo");
                                    await client.SendMailAsync(message);
                                }
                            }

                            //return RedirectToPage("SuccessPage");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                            return Page(); // or return error view
                        }
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            errorDetected = true;
            return Page();
        }

        private Member CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Member>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Member)}'. " +
                    $"Ensure that '{nameof(Member)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<Member> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Member>)_userStore;
        }
    }
}