using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CrypticPay.Data;

namespace CrypticPay.Areas.Payments
{
    public class CreateModel : PageModel
    {
        private readonly CrypticPay.Data.CrypticPayCoinContext _context;

        public CreateModel(CrypticPay.Data.CrypticPayCoinContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CrypticPayCoins CrypticPayCoins { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Coins.Add(CrypticPayCoins);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
