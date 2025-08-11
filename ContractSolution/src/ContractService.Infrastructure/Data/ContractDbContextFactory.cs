using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContractService.Infrastructure.Data;

public class ContractDbContextFactory : IDesignTimeDbContextFactory<ContractDbContext>
{
    public ContractDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("CONTRACTS_CONNECTION") ??
                   "Host=localhost;Database=contractsdb;Username=postgres;Password=postgres";
        var optionsBuilder = new DbContextOptionsBuilder<ContractDbContext>();
        optionsBuilder.UseNpgsql(conn);
        return new ContractDbContext(optionsBuilder.Options);
    }
}
