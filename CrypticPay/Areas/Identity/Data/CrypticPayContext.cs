using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrypticPay.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrypticPay.Data
{
    public class CrypticPayContext : IdentityDbContext<CrypticPayUser>
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<ChatData> Chats { get; set; }
        public DbSet<FileUpload> Uploads { get; set; }
        DbSet<Transaction> Transactions { get; set; }

        public CrypticPayContext(DbContextOptions<CrypticPayContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<CrypticPayUser>()
            .HasOne(p => p.WalletKryptik)
            .WithOne(b => b.Owner)
            .HasForeignKey<Wallet>(b => b.CrypticPayUserKey)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Wallet>()
                .HasMany(p => p.CurrencyWallets)
                .WithOne(b => b.WalletKryptik)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BlockchainAddress>()
                .HasOne(p => p.CurrencyWallet)
                .WithOne(b => b.AddressOnChain)
                .HasForeignKey<CurrencyWallet>(b => b.AddressKey)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
