using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWalletDB.Domain
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public virtual ICollection<UserBankAccount> UserBankAccounts { get; set; }

        public User()
        {
            UserBankAccounts = new List<UserBankAccount>();
        }
    }
}
