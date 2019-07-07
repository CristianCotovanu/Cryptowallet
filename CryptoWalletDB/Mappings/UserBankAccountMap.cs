using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using CryptoWalletDB.Domain;

namespace CryptoWalletDB.Mappings
{
    public class UserBankAccountMap : EntityTypeConfiguration<UserBankAccount>
    {
        public UserBankAccountMap()
        {
            ToTable("UsersBankAccounts").HasKey(t => t.AccountID);

            Property(t => t.AccountID).HasColumnName("AccountID").IsRequired();
            Property(t => t.UserID).HasColumnName("UserID").IsRequired();
            Property(t => t.Currency).HasColumnName("Currency").IsRequired().HasMaxLength(3);
            Property(t => t.Amount).HasColumnName("Amount").IsRequired();

            HasRequired(t => t.User) 
                .WithMany(t => t.UserBankAccounts)
                .HasForeignKey(f => f.UserID);
        }
    }
}
