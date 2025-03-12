using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Quartz.Domain.Entities;

namespace Wallet.Quartz.Infrastructure.Contexts;

public class QuartzContext : DbContext
{
    public DbSet<Notification> Notifications { get; set; }

    public QuartzContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //@TODO заменить на постгрес
        optionsBuilder.UseSqlite("Data Source=notifications.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }

    private class UserConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}