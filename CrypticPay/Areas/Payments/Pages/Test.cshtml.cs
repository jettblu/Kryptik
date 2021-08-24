using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CrypticPay.Areas.Payments.Pages
{
    public class TestModel : PageModel
    {   
        public string Message { get; set; }

        public void OnGet()
        {
        }

        public JsonResult OnPostTest()
        {
            Message = "Testing";

      
            return new JsonResult(Message);
        }
    }
}
