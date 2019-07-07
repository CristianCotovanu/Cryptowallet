using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CryptoWalletDB.Domain;
using CryptoWalletDB.Mappings;

namespace CryptoWalletDB
{
    public class CryptoWalletDbContext : DbContext
    {
        static CryptoWalletDbContext()
        {
            Database.SetInitializer<CryptoWalletDbContext>(null);
        }

        public CryptoWalletDbContext()
            : base("Name=CryptoWalletDbContext") { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<UserBankAccount> UsersBankAccounts { get; set; }
        public DbSet<UserTransaction> UsersTransactions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new UserBankAccountMap());
            modelBuilder.Configurations.Add(new UserTransactionMap());
        }
    }
}
