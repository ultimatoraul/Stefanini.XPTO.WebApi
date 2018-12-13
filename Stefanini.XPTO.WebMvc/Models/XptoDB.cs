namespace Stefanini.XPTO.WebMvc.Models {
  using System;
  using System.Data.Entity;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;

  public partial class XptoDB : DbContext {
    public XptoDB()
        : base("name=XptoContext") {
    }

    public virtual DbSet<Client> Client { get; set; }
    public virtual DbSet<Product> Product { get; set; }
    public virtual DbSet<ProductClient> ProductClient { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
    }
  }
}
