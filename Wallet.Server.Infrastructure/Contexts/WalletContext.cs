using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Server.Domain.Entities;

namespace Wallet.Server.Infrastructure.Contexts;

public sealed class WalletContext : DbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Transaction> Transactions { get; init; }
    public DbSet<Category> Categories { get; init; }
    public DbSet<Goal> Goals { get; init; }

    public WalletContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //@TODO заменить на постгрес
        optionsBuilder.UseSqlite("Data Source=wallet.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }

    private class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Categories).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            builder.HasMany(x => x.Goals).WithOne(x => x.User).HasForeignKey(x => x.UserId);
        }
    }

    private class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Category).WithMany(x => x.Transactions).HasForeignKey(x => x.CategoryId);
        }
    }

    private class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithMany(x => x.Categories).HasForeignKey(x => x.UserId);
            builder.HasMany(x => x.Transactions).WithOne(x => x.Category).HasForeignKey(x => x.CategoryId);
        }
    }

    private class GoalConfiguration : IEntityTypeConfiguration<Goal>
    {
        public void Configure(EntityTypeBuilder<Goal> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithMany(x => x.Goals).HasForeignKey(x => x.UserId);
        }
    }
}