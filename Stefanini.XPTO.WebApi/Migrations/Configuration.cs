namespace Stefanini.XPTO.WebApi.Migrations {
  using Stefanini.XPTO.WebApi.Models;
  using System;
  using System.Data.Entity;
  using System.Data.Entity.Migrations;
  using System.Linq;

  internal sealed class Configuration : DbMigrationsConfiguration<Stefanini.XPTO.WebApi.DAL.XptoContext> {
    public Configuration() {
      AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(Stefanini.XPTO.WebApi.DAL.XptoContext context) {
      context.Clients.AddOrUpdate(x => x.ID,
        new Client { ID = 11, FirstName = "Raul", LastName = "Pires", Gender = "Masculino", BirthDate = DateTime.Parse("1990-04-09"), Email = "raulsp@ymail.com", Active = 1 },
        new Client { ID = 12, FirstName = "Cinthia", LastName = "Aguillera", Gender = "Feminino", BirthDate = DateTime.Parse("1997-06-10"), Email = "cacosta1@stefanini.com", Active = 1 }
      );

      context.Products.AddOrUpdate(x => x.ID,
          new Product() { ID = 1050, Name = "Produto X" },
          new Product() { ID = 1051, Name = "Produto Z" }
      );

      context.ProductsClient.AddOrUpdate(x => x.ID,
          new ProductClient() { ClientID = 11, ProductID = 1050 }
      );
    }
  }
}
