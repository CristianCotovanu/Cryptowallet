using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoWallet.Models
{
    public class ExchangeViewModel
    {
        [Display(Name = "Exchange from")]
        public string FromCurrency { get; set; }

        [Display(Name = "Exchange to")]
        public string ToCurrency { get; set; }

        [Range(1, 100000)]
        [Required]
        [Display(Name = "Amount to exchange")]
        public decimal Amount { get; set; }

        public List<SelectListItem> FromCurrencyList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ToCurrencyList { get; set; } = new List<SelectListItem>();
    }
}