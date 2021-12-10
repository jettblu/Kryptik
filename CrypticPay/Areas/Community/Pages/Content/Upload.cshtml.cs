using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CrypticPay.Areas.Identity.Data;
using CrypticPay.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrypticPay.Areas.Community.Pages.Content
{
    public class UploadModel : PageModel
    {
        private readonly UserManager<CrypticPayUser> _userManager;
        private readonly CrypticPayContext _context;
        private Services.DecentralizedStorage _dstorage { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "New File")]
            public IFormFile NewFile { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
        }
        public UploadModel(UserManager<CrypticPayUser> userManager, Services.DecentralizedStorage dstorage, CrypticPayContext context)
        {
            _userManager = userManager;
            _dstorage = dstorage;
            _context = context;
        }
        public void OnGet()
        {
        }

        // upload file input to decentralized storage system
        // ONLY SUPPORTS IMAGES RIGHT NOW TO MINIMZE STORAGE COST
        // ADD MESSAGES THAT REFLECT STATUS TO CLIENT
        public async Task<IActionResult> OnPostUploadAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            // verify file is valid 
            if (Input.NewFile == null || !Utils.IsValidPhoto(Input.NewFile.FileName)) return RedirectToPage();

            // upload file
            Data.Responses.ResponseUpload response = await _dstorage.UploadFile(Input.NewFile.FileName, Input.NewFile);

            if(response.Status == Globals.Status.Success)
            {
                // save upload to local DB
                var upload = new Data.FileUpload()
                {
                    CID = response.CID,
                    CreationTime = DateTime.Now,
                    OwnerId = user.Id,
                    Description = Input.Description,
                    Name = Input.Name
                };
                _context.Uploads.Add(upload);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}
