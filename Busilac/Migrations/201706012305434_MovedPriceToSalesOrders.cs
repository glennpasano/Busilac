namespace Busilac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedPriceToSalesOrders : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MaterialsPrices", "MaterialId", "dbo.Materials");
            DropForeignKey("dbo.MaterialsPrices", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.MaterialsPrices", new[] { "MaterialId" });
            DropIndex("dbo.MaterialsPrices", new[] { "UserId" });
            AddColumn("dbo.MaterialsSalesOrdersDetails", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Materials", "CostPerWeight");
            DropTable("dbo.MaterialsPrices");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Materials", "CostPerWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.MaterialsSalesOrdersDetails", "Price");
            CreateIndex("dbo.MaterialsPrices", "UserId");
            CreateIndex("dbo.MaterialsPrices", "MaterialId");
            AddForeignKey("dbo.MaterialsPrices", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.MaterialsPrices", "MaterialId", "dbo.Materials", "MaterialId", cascadeDelete: true);
        }
    }
}
