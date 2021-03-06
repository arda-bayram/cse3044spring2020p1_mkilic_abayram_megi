﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SnackExchange.Web.Models;
using SnackExchange.Web.Models.Auth;
using SnackExchange.Web.Repository;

namespace SnackExchange.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<Address> _addressRepository;

        public RegisterModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IRepository<Country> countryRepository,
            IRepository<Address> addressRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _countryRepository = countryRepository;
            _addressRepository = addressRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Country")]
            public string Country { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Country Code")]
            public string CountryCode { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Currency")]
            public string Currency { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Address Title")]
            public string AddressTitle { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Address Plus Code")]
            public string AddressPlusCode { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Address Text")]
            public string AddressText { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                #region Country
                var country = new Country
                {
                    Name = Input.Country,
                    Currency = Input.Currency,
                    Code = Input.CountryCode,
                };
                var countries = _countryRepository.FindBy(c => c.Name == country.Name);
                var previousCountry = countries.FirstOrDefault();

                if (previousCountry != null)
                {
                    country = previousCountry;
                    _logger.LogInformation("Country already exists in database.");
                }
                else
                {
                    _countryRepository.Insert(country);
                    _logger.LogInformation("Country added to database.");
                }
                #endregion Country
                #region User
                var user = new AppUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Country = country,
                    IsModerator = false,
                    CountryCode = country.Code,
                    UserStatus = UserStatus.Active
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                #endregion User

                #region Address
                var address = new Address
                {
                    Title = Input.AddressTitle,
                    Text = Input.AddressText,
                    PlusCode = Input.AddressPlusCode,
                    User = user,
                    UserId = user.Id,
                    Country = country
                };

                if (address != null)
                {
                    _addressRepository.Insert(address);
                    _logger.LogInformation("Address added to database.");
                }
                else
                {
                    _logger.LogInformation("Address error while inserting database.");
                }
                // update user address list
                user.Addresses.Add(address);
                var updateResult = _userManager.UpdateAsync(user);
                if (!updateResult.Result.Succeeded)
                {
                    _logger.LogError("User cannot be updated.");
                }
                country.Addresses.Add(address);
                _countryRepository.Update(country);
                #endregion Address
                //end of register



                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
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
            return Page();
        }
    }
}
