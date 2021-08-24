using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using CrypticPay.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio.Rest.Verify.V2.Service;
using Microsoft.AspNetCore.Mvc.Rendering;
using Twilio.Rest.Lookups.V1;
using CrypticPay.ProfilePhoto;
using CrypticPay.Data;

namespace CrypticPay.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly TwilioVerifySettings _settings;
        private readonly SignInManager<CrypticPayUser> _signInManager;
        private readonly UserManager<CrypticPayUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly CrypticPayContext _context;

        public RegisterModel(
            UserManager<CrypticPayUser> userManager,
            SignInManager<CrypticPayUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IOptions<TwilioVerifySettings> settings,
            CountryService countryService,
            CrypticPayContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _settings = settings.Value;
            _context = context;
            // Load the countries from the service
            AvailableCountries = countryService.GetCountries();
        }

        public string PhoneNumberToSave { get; set; }

        public List<SelectListItem> AvailableCountries { get; }
        public CrypticPayUser IdentityUser { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "User Name")]
            public string UserName { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }


            // The country selected by the user.
            [Display(Name = "Phone number country")]
            public string PhoneNumberCountryCode { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            Input = new InputModel();
            
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // get phone number in correct format
                try
                {
                    var numberDetails = await PhoneNumberResource.FetchAsync(
                        pathPhoneNumber: new Twilio.Types.PhoneNumber(Input.PhoneNumber),
                        countryCode: Input.PhoneNumberCountryCode,
                        type: new List<string> { "carrier" });

                    PhoneNumberToSave = numberDetails.PhoneNumber.ToString();
                    // prevent duplicate numbers from being registered
                    if(Utils.MobileAlreadyExists(PhoneNumberToSave, _context))
                    {
                        StatusMessage = "Error: There is already an account associated with mobile number.";
                        ModelState.AddModelError("", "This number already exists. Please change your phone number and try again.");
                        return Page();
                    }

                }
                catch
                {
                    ModelState.AddModelError("", "Please double check your phone number and try again.");
                    return Page();
                }

                // set random avatar
                var profilePhoto = Avatar.RetrieveRandomURI();
                // create user
                var user = new CrypticPayUser 
                {
                    UserName = Input.UserName,
                    Name = Input.FullName,
                    PhoneNumber = PhoneNumberToSave,
                    ProfilePhotoPath = profilePhoto
                };

                var result = await _userManager.CreateAsync(user, Input.Password);


                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    IdentityUser = await _userManager.FindByNameAsync(Input.UserName);

                    if (_userManager.Options.SignIn.RequireConfirmedPhoneNumber)
                    {   
                        // send phone verification code

                        try
                        {
                            var verification = await VerificationResource.CreateAsync(
                                to: PhoneNumberToSave,
                                channel: "sms",
                                pathServiceSid: _settings.VerificationServiceSID
                            );

                            if (verification.Status == "pending")
                            {
                                // redirect to confirmation page for code entry
                                return RedirectToPage("RegisterConfirmation", new {userName = Input.UserName, returnUrl = returnUrl });
                            }

                            ModelState.AddModelError("", $"There was an error sending the verification code: {verification.Status}");
                        }
                        catch (Exception)
                        {
                            ModelState.AddModelError("",
                                "There was an error sending the verification code, please check the phone number is correct and try again");
                        }

                       
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    


                    // uncomment below for email registration process

                    /*                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                                        var callbackUrl = Url.Page(
                                            "/Account/ConfirmEmail",
                                            pageHandler: null,
                                            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                                            protocol: Request.Scheme);

                                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                                        {
                                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                                        }
                                        else
                                        {
                                            await _signInManager.SignInAsync(user, isPersistent: false);
                                            return LocalRedirect(returnUrl);
                                        }*/
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
