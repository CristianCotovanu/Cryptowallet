using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using CryptoWalletDB.Domain;

namespace CryptoWalletDB.Mappings
{
    public class UserTransactionMap : EntityTypeConfiguration<UserTransaction>
    {
        public UserTransactionMap()
        {
            ToTable("UsersTransactions").HasKey(t => t.TransactionID);

            Property(t => t.TransactionID).HasColumnName("TransactionID").IsRequired();
            Property(t => t.FromAccountID).HasColumnName("FromAccountID").IsRequired();
            Property(t => t.ToAccountID).HasColumnName("ToAccountID").IsRequired();
            Property(t => t.Amount).HasColumnName("Amount").IsRequired();
            Property(t => t.TransactionDate).HasColumnName("TransactionDate").IsRequired();
            Property(t => t.CurrencyRate).HasColumnName("CurrencyRate").IsRequired();

            HasRequired(t => t.FromAccount)
                .WithMany(t => t.FromTransactions)
                .HasForeignKey(f => f.FromAccountID);

            HasRequired(t => t.ToAccount)
                .WithMany(t => t.ToTransactions)
                .HasForeignKey(b => b.ToAccountID);
        }
    }
}
