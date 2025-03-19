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
    public DbSet<Notification> Notifications { get; init; }

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
        }
    }

    private class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }

    private class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithMany(x => x.Categories).HasForeignKey(x => x.UserId);
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
    
    private class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.User).WithMany(x => x.Notifications).HasForeignKey(x => x.UserId);
        }
    }
}