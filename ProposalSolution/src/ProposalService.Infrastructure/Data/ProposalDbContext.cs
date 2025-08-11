using Microsoft.EntityFrameworkCore;
using ProposalService.Domain.Entities;

namespace ProposalService.Infrastructure.Data;

public class ProposalDbContext : DbContext
{
    public ProposalDbContext(DbContextOptions<ProposalDbContext> options) : base(options) { }

    public DbSet<Proposal> Proposals => Set<Proposal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var p = modelBuilder.Entity<Proposal>();
        p.ToTable("proposals");
        p.HasKey(x => x.Id);
        p.Property(x => x.Title).IsRequired().HasMaxLength(200);
        p.Property(x => x.CreatedAt).IsRequired();
        p.Property(x => x.Status).HasConversion<string>().IsRequired();
    }
}
