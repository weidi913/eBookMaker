﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FYP1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FYP1.Areas.Identity.Pages.Account.Manage
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<Member> _userManager;
        private readonly SignInManager<Member> _signInManager;
        private readonly ILogger<ProfileModel> _logger;

        public ProfileModel(
            UserManager<Member> userManager,
            SignInManager<Member> signInManager,
            ILogger<ProfileModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }
        [BindProperty]
        public InputPasswordModel PasswordInput { get; set; }
        [BindProperty]
        public InputPersonalInfoModel InfoInput { get; set; }
        public Member curUser { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        public string InfoMessage { get; set; }
        public string PasswordMessage { get; set; }
        public string PasswordDetailedMessage { get; set; }
        
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
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
        public class InputPasswordModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
        public class InputPersonalInfoModel
        {
            public string ImageData { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime BirthdayDate { get; set; }
            public string Gender { get; set; }
        }
        public async Task<IActionResult> OnGetAsync(string? InfoMessage, string? PasswordMessage, string? PasswordDetailedMessage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            curUser = user;
            this.InfoMessage = InfoMessage;
            this.PasswordMessage = PasswordMessage;
            this.PasswordDetailedMessage = PasswordDetailedMessage;

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }
        public async Task<IActionResult> OnPostPersonalInfoAsync()
        {
            /*            if (!ModelState.IsValid)
                        {
                            return Page();
                        }*/
            var testingData = InfoInput;
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            // Check if FirstName, BirthdayDate, and Gender are not empty or null
            if (!string.IsNullOrEmpty(InfoInput.FirstName))
            {
                user.firstName = InfoInput.FirstName;
            }
            // Check if FirstName, BirthdayDate, and Gender are not empty or null
            if (!string.IsNullOrEmpty(InfoInput.Gender))
            {
                user.gender = InfoInput.Gender;
            }
            // Check if FirstName, BirthdayDate, and Gender are not empty or null
            if (InfoInput.BirthdayDate != DateTime.MinValue)
            {
                user.birthday = InfoInput.BirthdayDate;
            }
            user.lastName = InfoInput.LastName;
            user.imageData = InfoInput.ImageData;

            // Save changes
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                StatusMessage = "Your personal info has been changed.";
                InfoMessage = "Your personal info has been changed.";

                // Changes saved successfully
                return RedirectToPage("./Profile", new { InfoMessage = 0 });
            }
            else
            {
                // Handle case where update fails
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                StatusMessage = "Your personal fail has NOT been changed.";
                return RedirectToPage(); // Redirect to profile page or any other appropriate action
            }
        }
        public async Task<IActionResult> OnPostPasswordAsync()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            string errorMsg = "";
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, PasswordInput.OldPassword, PasswordInput.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    errorMsg += error.Description;
                    errorMsg += " ";
                }
                return RedirectToPage("./Profile", new { PasswordMessage = 1, PasswordDetailedMessage = errorMsg });

            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToPage("./Profile", new { PasswordMessage = 0});

        }
    }
}
