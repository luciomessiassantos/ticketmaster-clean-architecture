using Microsoft.EntityFrameworkCore;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Entities;
using TicketMaster.Domain.Entities.EventAggregate;
using TicketMaster.Domain.Entities.OrderAggregate;

namespace TicketMaster.Infrastructure.Data;

public class TicketMasterDbContext(
    DbContextOptions<TicketMasterDbContext> options
) : DbContext(options)
{

    public DbSet<Event> Events { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<TicketLot> TicketLots { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<IssuedTicket> IssuedTickets { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureAuditableEntity<Event, Guid>(modelBuilder, "Events");
        ConfigureAuditableEntity<Session, Guid>(modelBuilder, "Sessions");
        ConfigureAuditableEntity<TicketLot, Guid>(modelBuilder, "TicketLots");
        ConfigureAuditableEntity<Order, Guid>(modelBuilder, "Orders");
        ConfigureAuditableEntity<IssuedTicket, Guid>(modelBuilder, "IssuedTickets");
        ConfigureAuditableEntity<Customer, Guid>(modelBuilder, "Customers");

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000).IsRequired(false);
            entity.Property(e => e.Organizer).HasMaxLength(200).IsRequired(false);
            entity.Property(e => e.EventState).HasConversion<string>().IsRequired();
            
            entity.Navigation(e => e.Sessions)
                .HasField("_sessions")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            entity.HasMany(e => e.Sessions)
                  .WithOne(s => s.Event)
                  .HasForeignKey(s => s.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.Property(s => s.SessionState).HasConversion<string>().IsRequired();
            
            entity.OwnsOne(s => s.Address, address =>
            {
                address.Property(a => a.Country).HasColumnName("Address_Country").HasMaxLength(100).IsRequired();
                address.Property(a => a.State).HasColumnName("Address_State").HasMaxLength(100).IsRequired();
                address.Property(a => a.City).HasColumnName("Address_City").HasMaxLength(100).IsRequired();
                address.Property(a => a.Neighborhood).HasColumnName("Address_Neighborhood").HasMaxLength(100).IsRequired();
                address.Property(a => a.Street).HasColumnName("Address_Street").HasMaxLength(200).IsRequired();
                address.Property(a => a.UF).HasColumnName("Address_UF").HasMaxLength(2).IsRequired();
                address.Property(a => a.ZipCode).HasColumnName("Address_ZipCode").HasMaxLength(10).IsRequired();
                address.Property(a => a.Place).HasColumnName("Address_Place").HasMaxLength(200).IsRequired();
            });

            entity.Navigation(s => s.TicketLots)
                .HasField("_ticketLots")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            entity.HasMany(s => s.TicketLots)
                  .WithOne(t => t.Session)
                  .HasForeignKey(t => t.SessionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TicketLot>(entity =>
        {
            entity.Property(t => t.Type).HasConversion<string>().IsRequired();
            entity.Property(t => t.Price).HasPrecision(18, 2).IsRequired();
            entity.Property(t => t.Lot).IsRequired();
            entity.Property(t => t.Remaining).IsRequired();
            entity.Property(t => t.Available).IsRequired().HasDefaultValue(true);

            entity.HasIndex(t => t.SessionId);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Email).HasMaxLength(200).IsRequired();

            entity.HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.Quantity).IsRequired();
            entity.Property(o => o.Consumed).IsRequired().HasDefaultValue(false);

            entity.HasIndex(o => o.CustomerId);
            entity.HasIndex(o => o.TicketLotId);

            entity.HasOne(o => o.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.TicketLot)
                  .WithMany()
                  .HasForeignKey(o => o.TicketLotId)
                  .OnDelete(DeleteBehavior.Restrict);

            
            entity.HasMany(o => o.IssuedTickets)
                  .WithOne(i => i.Order)
                  .HasForeignKey(i => i.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IssuedTicket>(entity =>
        {
            entity.Property(i => i.ConsumeCodeQR).HasMaxLength(500).IsRequired();
            entity.Property(i => i.Used).IsRequired().HasDefaultValue(false);
            
            entity.HasIndex(i => i.OrderId);
        });
    }

    private static void ConfigureAuditableEntity<T, TKey>(ModelBuilder modelBuilder, string tableName)
        where T : AuditableEntity<TKey>
    {
        modelBuilder.Entity<T>(entity =>
        {
            entity.ToTable(tableName);
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.Property(e => e.RowVersion)
                  .IsRowVersion()
                  .IsConcurrencyToken();

            entity.Property(e => e.IsDeleted)
                  .IsRequired()
                  .HasDefaultValue(false);

            entity.Property(e => e.DeletedAt);
            entity.Property(e => e.DeletedBy);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }

    public override int SaveChanges()
    {
        OnSaving();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnSaving();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void OnSaving()
    {
        var entries = ChangeTracker.Entries<AuditableEntity<Guid>>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }
}