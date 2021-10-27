using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CrypticPay.Areas.Identity.Data;
using CrypticPay.ProfilePhoto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
// for validating phone number input
using Twilio.Exceptions;
using Twilio.Rest.Lookups.V1;
using Twilio.Rest.Verify.V2.Service;

namespace CrypticPay.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<CrypticPayUser> _userManager;
        private readonly SignInManager<CrypticPayUser> _signInManager;
        private readonly TwilioVerifySettings _settings;
        private readonly IEmailSender _emailSender;
        private readonly StorageAccountOptions _storageSettings;

        public IndexModel(
            UserManager<CrypticPayUser> userManager,
            SignInManager<CrypticPayUser> signInManager,
            CountryService countryService,
            IOptions<TwilioVerifySettings> settings,
            IEmailSender emailSender,
            IOptions<StorageAccountOptions> storagSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _settings = settings.Value;
            _storageSettings = storagSettings.Value;
            // Load the countries from the service
            AvailableCountries = countryService.GetCountries();
            // set email sender
            _emailSender = emailSender;
        }
        public List<SelectListItem> AvailableCountries { get; }

        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            
            [DataType(DataType.Text)]
            [Display(Name = "Full name")]
            public string Name { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "UserName")]
            public string NewUserName { get; set; }


            [Display(Name = "Birth Date")]
            [DataType(DataType.Date)]
            public DateTime DOB { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Enable 2fa")]
            public bool EnableTwofactorAuth { get; set; }

            [TempData]
            public string StatusMessage { get; set; }

            // The country selected by the user
            [Display(Name = "Phone number country")]
            public string PhoneNumberCountryCode { get; set; }

            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }

 
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            [Display(Name = "New Avatar")]
            public IFormFile NewPhoto { get; set; }

            public string Code { get; set; }

        }

        private async Task LoadAsync(CrypticPayUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;

            Input = new InputModel
            {
                Name = user.Name,
                DOB = user.DOB,
                PhoneNumber = phoneNumber,
                NewEmail = user.Email,
                NewUserName = user.UserName,
                Code = token,
                EnableTwofactorAuth = user.TwoFactorEnabled
            };
            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAvatarAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // verify file is valid 
            if (!Utils.IsValidPhoto(Input.NewPhoto.FileName))
            {
                StatusMessage = "Error: Invalid file type.";
                return RedirectToPage();
            }

            BlobUtility blobUtility = new BlobUtility(_storageSettings, user);

            Stream outStream = Avatar.CropImage(Input.NewPhoto);

            await blobUtility.UploadImage(outStream, Input.NewPhoto.FileName);

            var blobURI = blobUtility.GetBlobURI();

            // update user profile photo URI
            // potentially add check for identical photo

            user.ProfilePhotoPath = blobURI;

            await _userManager.UpdateAsync(user);

            StatusMessage = "Avatar has been updated.";
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostUpdatePhoneDetailsAsync()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            // save phone number if new
            if (Input.PhoneNumber != phoneNumber)
            {
                try
                {   
                    // format phone number
                    var numberDetails = await PhoneNumberResource.FetchAsync(
                        pathPhoneNumber: new Twilio.Types.PhoneNumber(Input.PhoneNumber),
                        countryCode: Input.PhoneNumberCountryCode,
                        type: new List<string> { "carrier" });

                    // save new phone number

                    var numberToSave = numberDetails.PhoneNumber.ToString();
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, numberToSave);
                    if (!setPhoneResult.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                    }
                    // new number should not be confirmed
                    user.PhoneNumberConfirmed = false;
                    await _userManager.UpdateAsync(user);

                    await _signInManager.RefreshSignInAsync(user);
                    StatusMessage = "Your number has been updated";

                }
                catch (ApiException ex)
                {
                    ModelState.AddModelError($"{nameof(Input)}.{nameof(Input.PhoneNumber)}",
                        $"The number you entered was not valid (Twilio code {ex.Code}), please check it and try again");
                    return Page();
                }   

            }
            // send verification code if number isn't confirmed
                if (!user.PhoneNumberConfirmed)
                {
                    try
                    {
                        // create verification code message
                        var verification = await VerificationResource.CreateAsync(
                            to: Input.PhoneNumber,
                            channel: "sms",
                            pathServiceSid: _settings.VerificationServiceSID
                        );



                        if (verification.Status == "pending")
                        {
                            return RedirectToPage("ConfirmPhone");
                        }

                        ModelState.AddModelError("", $"There was an error sending the verification code: {verification.Status}");
                    


                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("",
                            "There was an error sending the verification code, please check the phone number is correct and try again");
                    }
                }
            return Page();

        }


            public async Task<IActionResult> OnPostUpdateBasicDetailsAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }


            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
            }

            if(Input.NewUserName != user.UserName)
            {
                user.UserName = Input.NewUserName;
            }

            if (Input.DOB != user.DOB)
            {
                user.DOB = Input.DOB;
            }


            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";


            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
      
            var user = await _userManager.GetUserAsync(User);

            // update 2fa setting if changed
            if (Input.EnableTwofactorAuth != user.TwoFactorEnabled)
            {
                user.TwoFactorEnabled = Input.EnableTwofactorAuth;
                // update DB
                await _userManager.UpdateAsync(user);
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
                return RedirectToPage();
            }

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // don't reset passsword if no password is specified
            if(Input.Password == null)
            {
                return RedirectToPage();
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(
                    Input.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                StatusMessage = "Confirmation link to change email sent. Please check your email.";
                return RedirectToPage();
            }

            StatusMessage = "Error: Your email is unchanged.";
            return RedirectToPage();
        }
    }
}