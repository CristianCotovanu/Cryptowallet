using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CryptoWallet.Models
{
    public class DepositViewModel
    {
        [Range(1,10000)]
        public decimal Amount { get; set; }
    }
}