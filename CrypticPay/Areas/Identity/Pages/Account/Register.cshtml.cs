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
        private readonly ISmsSender _smsSender;

        public RegisterModel(
            UserManager<CrypticPayUser> userManager,
            SignInManager<CrypticPayUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IOptions<TwilioVerifySettings> settings,
            CountryService countryService,
            CrypticPayContext context,
            ISmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _settings = settings.Value;
            _context = context;
            _smsSender = smsSender;
            // Load the countries from the service
            AvailableCountries = countryService.GetCountries();
        }

        public string PhoneNumberToSave { get; set; }

        public List<SelectListItem> AvailableCountries { get; }
        public CrypticPayUser IdentityUser { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public SendModel SendMod { get; set; }


        public VerifyModel VerifyMod { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class SendModel
        {
            [Required]
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            // The country selected by the user.
            [Display(Name = "Phone number country")]
            public string PhoneNumberCountryCode { get; set; }
        }


        public class VerifyModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            // The country selected by the user.
            [Display(Name = "Phone number country")]
            public string PhoneNumberCountryCode { get; set; }

            [Display(Name = "Code")]
            public string VerificationCode { get; set; }
        }

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

            [Required]
            [Display(Name = "Code")]
            public string VerificationCode { get; set; }


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
            SendMod = new SendModel();
            VerifyMod = new VerifyModel();

            
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
                    StatusMessage = "Error: Please double check your phone number and try again..";
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

        // ensure verfification code is correct
        // returns true if verified 
        public async Task<IActionResult> OnPostVerifyPhoneAsync()
        {
            try
            {
                /*var verification = await VerificationCheckResource.CreateAsync(
                    to: PhoneNumber,
                    code: VerificationCode,
                    pathServiceSid: _settings.VerificationServiceSID
                );*/
                var formatresult = await FormatNumber(VerifyMod.PhoneNumber, VerifyMod.PhoneNumberCountryCode);
                if (formatresult != Globals.Status.Done)
                {
                    return new JsonResult(false);
                }
                var verification = await _smsSender.SendVerificationAsync(PhoneNumberToSave, VerifyMod.VerificationCode);

                if (verification.Status == "approved")
                {
                    return new JsonResult(true);
                    /*IdentityUser.PhoneNumberConfirmed = true;
                    var updateResult = await _userManager.UpdateAsync(IdentityUser);*/


                    /* if (updateResult.Succeeded)
                     {
                         await _signInManager.SignInAsync(IdentityUser, isPersistent: true);
                         return new JsonResult(true);
                     }
                     else
                     {
                         ModelState.AddModelError("", "There was an error confirming the verification code, please try again");
                     }
                 }*/
                    /*else
                    {
                        ModelState.AddModelError("", $"There was an error confirming the verification code: {verification.Status}");
                    }*/
                }
            }

            catch (Exception)
            {
                ModelState.AddModelError("",
                    "There was an error confirming the code, please check the verification code is correct and try again");
            }
            return new JsonResult(false);
        }


        // send verification code to phone
        // returns true if succeded
        public async Task<IActionResult> OnPostSendPhoneAsync()
        {
            var formatresult = await FormatNumber(SendMod.PhoneNumber, SendMod.PhoneNumberCountryCode);
            if (formatresult != Globals.Status.Done)
            {
                return new JsonResult(false);
            }
            var sendCodeResult = await SendCode();
            if (sendCodeResult == Globals.Status.Done) return new JsonResult(true);
            return new JsonResult(false);
            
        }


        public async Task<Globals.Status> FormatNumber(string num, string code)
        {
            // get phone number in correct format
            try
            {
                var numberDetails = await PhoneNumberResource.FetchAsync(
                    pathPhoneNumber: new Twilio.Types.PhoneNumber(num),
                    countryCode: code,
                    type: new List<string> { "carrier" });

                PhoneNumberToSave = numberDetails.PhoneNumber.ToString();
                // prevent duplicate numbers from being registered
                if (Utils.MobileAlreadyExists(PhoneNumberToSave, _context))
                {
                    StatusMessage = "Error: There is already an account associated with mobile number.";
                    ModelState.AddModelError("", "This number already exists. Please change your phone number and try again.");
                    return Globals.Status.Failure;
                }
                return Globals.Status.Done;
            }
            catch
            {
                ModelState.AddModelError("", "Please double check your phone number and try again.");
                StatusMessage = "Error: Please double check your phone number and try again..";
                return Globals.Status.Failure;
            }
        }

        public async Task<Globals.Status> SendCode()
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
                    return Globals.Status.Done;
                }
                else
                {
                    ModelState.AddModelError("", $"There was an error sending the verification code: {verification.Status}");
                    return Globals.Status.Failure;
                }        
            }
            catch (Exception)
            {
                ModelState.AddModelError("",
                    "There was an error sending the verification code, please check the phone number is correct and try again");
                return Globals.Status.Failure;
            }
        }


    }
}
