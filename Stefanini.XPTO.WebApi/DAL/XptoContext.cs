namespace Stefanini.XPTO.WebApi.DAL
{
  using System.Data.Entity;
  using System.Data.Entity.ModelConfiguration.Conventions;
  using Stefanini.XPTO.WebApi.Models;

  public class XptoContext : DbContext
  {
    public XptoContext()
        : base("XptoContext")
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductClient> ProductsClient { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
    }
  }
}