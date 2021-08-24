using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using CrypticPay.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Twilio.Rest.Verify.V2.Service;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System;
using CrypticPay.Services;

namespace CrypticPay.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<CrypticPayUser> _userManager;
        private readonly IEmailSender _sender;
        private readonly ISmsSender _smsSender;
        private readonly SignInManager<CrypticPayUser> _signInManager;
        private readonly TwilioVerifySettings _settings;
        public string PhoneNumber { get; set; }

        public RegisterConfirmationModel(UserManager<CrypticPayUser> userManager, IEmailSender sender, SignInManager<CrypticPayUser> signInManager, IOptions<TwilioVerifySettings> settings, ISmsSender smsSender)
        {
            _userManager = userManager;
            _sender = sender;
            _signInManager = signInManager;
            _settings = settings.Value;
            _smsSender = smsSender;
        }

        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        [BindProperty, Required, Display(Name = "Code")]
        public string VerificationCode { get; set; }

        [BindProperty]
        public string UserName { get; set; }

        public string EmailConfirmationUrl { get; set; }
        public CrypticPayUser IdentityUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string userName, string returnUrl = null)
        {   
            // save username, so we can find user on post
            UserName = userName;
            // set phone number, so it can be displayed to user
            PhoneNumber = _userManager.FindByNameAsync(userName).Result.PhoneNumber;
            
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            IdentityUser = await _userManager.FindByNameAsync(UserName);

            await LoadPhoneNumber();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                /*var verification = await VerificationCheckResource.CreateAsync(
                    to: PhoneNumber,
                    code: VerificationCode,
                    pathServiceSid: _settings.VerificationServiceSID
                );*/
                var verification = await _smsSender.SendVerificationAsync(IdentityUser.PhoneNumber, VerificationCode);

                if (verification.Status == "approved")
                {
                    
                    IdentityUser.PhoneNumberConfirmed = true;
                    var updateResult = await _userManager.UpdateAsync(IdentityUser);

                    if (updateResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(IdentityUser, isPersistent: true);
                        return RedirectToPage("Manage/ConfirmPhoneSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("", "There was an error confirming the verification code, please try again");
                    }
                }
                else
                {
                    ModelState.AddModelError("", $"There was an error confirming the verification code: {verification.Status}");
                }
            }

            catch (Exception)
            {
                ModelState.AddModelError("",
                    "There was an error confirming the code, please check the verification code is correct and try again");
            }

            return Page();
        }
       

        private async Task LoadPhoneNumber()
        {
            if (IdentityUser == null)
            {
                throw new Exception($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            PhoneNumber = IdentityUser.PhoneNumber;
        }






            // uncomment below for email registration confirmation

            /* var user = await _userManager.FindByNameAsync();
             if (user == null)
             {
                 return NotFound($"Unable to load user with email '{email}'.");
             }

             Email = email;
             // Once you add a real email sender, you should remove this code that lets you confirm the account
             DisplayConfirmAccountLink = false;
             if (DisplayConfirmAccountLink)
             {
                 var userId = await _userManager.GetUserIdAsync(user);
                 var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                 code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                 EmailConfirmationUrl = Url.Page(
                     "/Account/ConfirmEmail",
                     pageHandler: null,
                     values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                     protocol: Request.Scheme);
             }*/

           
        
    }
}
