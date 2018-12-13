namespace Stefanini.XPTO.WinForms.Models {
  using System;
  using System.Data.Entity;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Linq;

  public partial class XptoDB : DbContext {
    public XptoDB()
        : base("name=XptoDB") {
    }

    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductClient> ProductClients { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
    }
  }
}
