namespace Busilac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPriceForMaterialAndProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MaterialsPrices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaterialId = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Materials", t => t.MaterialId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.MaterialId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Products", "Price", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MaterialsPrices", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MaterialsPrices", "MaterialId", "dbo.Materials");
            DropIndex("dbo.MaterialsPrices", new[] { "UserId" });
            DropIndex("dbo.MaterialsPrices", new[] { "MaterialId" });
            DropColumn("dbo.Products", "Price");
            DropTable("dbo.MaterialsPrices");
        }
    }
}
