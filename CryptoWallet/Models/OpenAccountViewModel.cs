using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoWallet.Models
{
    public class OpenAccountViewModel
    {
        [Display(Name = "New account currency")]
        public string NewCurrency;
        public List<SelectListItem> CurrencyList { get; set; } = new List<SelectListItem>();
    }
}