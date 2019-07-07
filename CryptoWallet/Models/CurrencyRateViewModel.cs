using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoWallet.Models
{
    public class CurrencyRateViewModel
    {
        public string Currency { get; set; }
        public decimal Rate { get; set; }
    }
}