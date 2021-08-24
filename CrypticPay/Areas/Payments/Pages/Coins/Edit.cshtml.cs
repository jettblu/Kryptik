using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrypticPay.Data;

namespace CrypticPay.Areas.Payments
{
    public class EditModel : PageModel
    {
        private readonly CrypticPay.Data.CrypticPayCoinContext _context;

        public EditModel(CrypticPay.Data.CrypticPayCoinContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CrypticPayCoins CrypticPayCoins { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CrypticPayCoins = await _context.Coins.FirstOrDefaultAsync(m => m.Id == id);

            if (CrypticPayCoins == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(CrypticPayCoins).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CrypticPayCoinsExists(CrypticPayCoins.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CrypticPayCoinsExists(string id)
        {
            return _context.Coins.Any(e => e.Id == id);
        }
    }
}
