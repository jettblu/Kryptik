using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CrypticPay.Areas.Community.Pages.Content
{
    public class UploadModel : PageModel
    {
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
        }
        public UploadModel(Services.DecentralizedStorage dstorage)
        {
            _dstorage = dstorage;
        }
        public void OnGet()
        {
        }

        // upload file input to decentralized storage system
        // ONLY SUPPORTS IMAGES RIGHT NOW TO MINIMZE STORAGE COST
        // ADD MESSAGES THAT REFLECT STATUS TO CLIENT
        public async Task<IActionResult> OnPostUploadAsync()
        {
            // verify file is valid 
            if (Input.NewFile == null || !Utils.IsValidPhoto(Input.NewFile.FileName)) return RedirectToPage();

            // upload file
            Globals.Status uploadStatus = await _dstorage.UploadFile(Input.NewFile.FileName, Input.NewFile);

            return RedirectToPage();
        }
    }
}
