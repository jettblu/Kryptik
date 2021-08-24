using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CrypticPay.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Twilio.Rest.Lookups.V1;
using CrypticPay.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CrypticPay.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<CrypticPayUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly CrypticPayContext _context;



        public ForgotPasswordModel(UserManager<CrypticPayUser> userManager, CountryService countryService, IEmailSender emailSender, ISmsSender smsSender, CrypticPayContext context)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _context = context;
            AvailableCountries = countryService.GetCountries();
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string PhoneNumberToSave { get; set; }
        public List<SelectListItem> AvailableCountries { get; }

        public class InputModel
        {

            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }


            // The country selected by the user.
            [Display(Name = "Phone number country")]
            public string PhoneNumberCountryCode { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // uncomment below for email functionality
                /*var user = await _userManager.FindByEmailAsync(Input.Email);*/



                // get phone number in correct format
                try
                {
                    var numberDetails = await PhoneNumberResource.FetchAsync(
                        pathPhoneNumber: new Twilio.Types.PhoneNumber(Input.PhoneNumber),
                        countryCode: Input.PhoneNumberCountryCode,
                        type: new List<string> { "carrier" });

                    PhoneNumberToSave = numberDetails.PhoneNumber.ToString();
                }
                catch
                {
                    ModelState.AddModelError("", "Please double check your phone number and try again.");
                    return Page();
                }

                var user = Utils.FindUserByMobile(PhoneNumberToSave, _context);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                // uncomment below for email functionality
                /*await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");*/

                var tinyLink = Utils.SquishLink(HtmlEncoder.Default.Encode(callbackUrl));

                var message = $"Please reset your password by clicking here: {tinyLink}'.";

                var result = await _smsSender.SendSmsAsync(PhoneNumberToSave, message);

                if (result.Status == Twilio.Rest.Api.V2010.Account.MessageResource.StatusEnum.Failed)
                {
                    ModelState.AddModelError("", $"While initiating your password reset, there was an error sending the verification code: {result.Status}");
                }
                else
                {
                    // redirect to forgotten password confirmation
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }



                
            }

            return Page();
        }
    }
}
