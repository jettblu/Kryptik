using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CrypticPay.Data;

namespace CrypticPay.Areas.Payments
{
    public class DeleteModel : PageModel
    {
        private readonly CrypticPay.Data.CrypticPayCoinContext _context;

        public DeleteModel(CrypticPay.Data.CrypticPayCoinContext context)
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CrypticPayCoins = await _context.Coins.FindAsync(id);

            if (CrypticPayCoins != null)
            {
                _context.Coins.Remove(CrypticPayCoins);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
