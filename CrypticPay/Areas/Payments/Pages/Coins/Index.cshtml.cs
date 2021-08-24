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
    public class IndexModel : PageModel
    {
        private readonly CrypticPay.Data.CrypticPayCoinContext _context;

        public IndexModel(CrypticPay.Data.CrypticPayCoinContext context)
        {
            _context = context;
        }

        public IList<CrypticPayCoins> CrypticPayCoins { get;set; }

        public async Task OnGetAsync()
        {
            CrypticPayCoins = await _context.Coins.ToListAsync();
        }
    }
}
