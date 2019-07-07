using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using CryptoWalletDB.Domain;

namespace CryptoWalletDB.Mappings
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            ToTable("Users").HasKey(t => t.UserID);

            Property(t => t.UserID).HasColumnName("UserID").IsRequired();
            Property(t => t.Name).HasColumnName("Name").IsRequired().HasMaxLength(32);
            Property(t => t.Password).HasColumnName("Password").IsRequired().HasMaxLength(32);
            Property(t => t.Email).HasColumnName("Email").IsRequired().HasMaxLength(64);
        }
    }
}
