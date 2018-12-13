namespace Stefanini.XPTO.WebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        LastName = c.String(),
                        FirstName = c.String(),
                        Gender = c.String(),
                        Email = c.String(),
                        Active = c.Int(nullable: false),
                        BirthDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ProductClient",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ClientID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Client", t => t.ClientID, cascadeDelete: true)
                .ForeignKey("dbo.Product", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ClientID)
                .Index(t => t.ProductID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductClient", "ProductID", "dbo.Product");
            DropForeignKey("dbo.ProductClient", "ClientID", "dbo.Client");
            DropIndex("dbo.ProductClient", new[] { "ProductID" });
            DropIndex("dbo.ProductClient", new[] { "ClientID" });
            DropTable("dbo.ProductClient");
            DropTable("dbo.Product");
            DropTable("dbo.Client");
        }
    }
}
