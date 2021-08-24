using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CrypticPay.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Twilio.Rest.Verify.V2.Service;
using Microsoft.Extensions.Options;
using CrypticPay.Services;

namespace CrypticPay.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<CrypticPayUser> _userManager;
        private readonly SignInManager<CrypticPayUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly TwilioVerifySettings _settings;
        private readonly ISmsSender _smsSender;

        public LoginModel(SignInManager<CrypticPayUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<CrypticPayUser> userManager,
            IOptions<TwilioVerifySettings> settings,
            ISmsSender smsSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _settings = settings.Value;
            _smsSender = smsSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "User Name")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

           

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                // handle case where requires 2fa, but email is not verified
              /*  if (result.Succeeded)
                {
                    var user = _userManager.FindByNameAsync(Input.UserName).Result;
                    if (Utils.Requires2fa(user))
                    {
                        result = Microsoft.AspNetCore.Identity.SignInResult.TwoFactorRequired;
                    }
                }*/

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    var user = _userManager.FindByNameAsync(Input.UserName).Result;
                    
                    // send phone verification code



                    try
                    {
                        /*var verification = await VerificationResource.CreateAsync(
                            to: user.Result.PhoneNumber,
                            channel: "sms",
                            pathServiceSid: _settings.VerificationServiceSID
                        );*/

                        
                        // generate two factor code and accompanying message
                        var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
                        var twofactorMessage = $"Your cryptic security code is {code}. The code will expire in 30 minutes.";
                        // text 2fa code
                        // remove line below when SmsSender accepts settings as dependency and is made static
                        var verification = await _smsSender.SendSmsAsync(user.PhoneNumber, twofactorMessage);
                        // alert user if 2fa message failed to send
                        if(verification.Status == Twilio.Rest.Api.V2010.Account.MessageResource.StatusEnum.Failed)
                        {
                            ModelState.AddModelError("", $"While initiating SMS 2fa, there was an error sending the verification code: {verification.Status}");
                        }
                        else
                        {
                            // redirect to 2fa page for code entry
                            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe, isSMS = true });
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("",
                            "There was an error sending the verification code, please check the phone number is correct and try again");
                    }




                    
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
