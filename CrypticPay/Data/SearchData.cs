using CrypticPay.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class SearchData
    {
            public string Query;
            public bool IsNumber;
            public List<CrypticPayUser> Users;
    }
}
