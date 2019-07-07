using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CryptoWallet.Models
{
    public class SendViewModel
    {
        [Range(1, 100000)]
        [Display(Name = "From account")]
        public int SenderAccountId { get; set; }

        [Required]
        [Display(Name = "Friend's Email")]
        public string ReceiverName { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        public List<SelectListItem> SenderAccounts { get; set; } = new List<SelectListItem>();
    }
}