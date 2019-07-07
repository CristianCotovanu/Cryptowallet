using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoWallet.Models
{
    public class TransactionViewModel
    {
        public string FromUser { get; set; }
        public string ToUser { get; set; }

        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
        public DateTime DateTime { get; set; }

        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }

    }
}