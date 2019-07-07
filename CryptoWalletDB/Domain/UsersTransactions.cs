using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWalletDB.Domain
{
    public class UserTransaction
    {
        public int TransactionID { get; set; }
        public int FromAccountID { get; set; }
        public int ToAccountID { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrencyRate { get; set; }
        public DateTime TransactionDate { get; set; }

        public virtual UserBankAccount ToAccount { get; set; }
        public virtual UserBankAccount FromAccount { get; set; }
    }
}
