using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWalletDB.Domain
{
    public class UserBankAccount
    {
        public int AccountID { get; set; }
        public int UserID { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<UserTransaction> ToTransactions { get; set; }
        public virtual ICollection<UserTransaction> FromTransactions { get; set; }

        public UserBankAccount ()
        {
            FromTransactions = new List<UserTransaction>();
            ToTransactions = new List<UserTransaction>();
        }
    }
}
