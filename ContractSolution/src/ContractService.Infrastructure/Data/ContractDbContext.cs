using Microsoft.EntityFrameworkCore;
using ContractService.Domain.Entities;

namespace ContractService.Infrastructure.Data;

public class ContractDbContext : DbContext
{
    public ContractDbContext(DbContextOptions<ContractDbContext> options) : base(options) { }

    public DbSet<Contract> Contracts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contract>(b =>
        {
            b.ToTable("contracts");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).IsRequired();
            b.Property(c => c.ProposalId).IsRequired();
            b.Property(c => c.ContractDate).IsRequired();
        });
    }
}
