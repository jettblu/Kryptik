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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace CrypticPay.Areas.Identity.Pages.Account.Manage
{
    public class ProfilePhotoModel : PageModel
    {
        private readonly UserManager<CrypticPayUser> _userManager;
        private readonly SignInManager<CrypticPayUser> _signInManager;
        private readonly StorageAccountOptions _settings;



        public ProfilePhotoModel(
            UserManager<CrypticPayUser> userManager,
            SignInManager<CrypticPayUser> signInManager,
            IOptions<StorageAccountOptions> settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _settings = settings.Value;
        }


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "New Avatar")]
            public IFormFile NewPhoto { get; set; }
        }

        private async Task LoadAsync(CrypticPayUser user)
        {
            var photoLink = user.ProfilePhotoPath;

            Input = new InputModel
            {
                // put photo link here to display current profile photo
                NewPhoto = null
            };

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

        public async Task<IActionResult> OnPostAsync()
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
            if (!Utils.IsValidPhoto(Input.NewPhoto.FileName)) return RedirectToPage();

            BlobUtility blobUtility = new BlobUtility(_settings, user);

            await blobUtility.UploadImage(Input.NewPhoto);

            var blobURI = blobUtility.GetBlobURI();

            // update user profile photo URI
            // potentially add check for identical photo

            user.ProfilePhotoPath = blobURI;

            await _userManager.UpdateAsync(user);

            StatusMessage = "Avatar has been updated.";
            return RedirectToPage();
        }

    }
}

