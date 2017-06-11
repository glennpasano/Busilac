namespace Busilac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSupplierIdToMaterialSales : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MaterialsSalesOrders", "SupplierId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.MaterialsSalesOrders", "SupplierId");
            AddForeignKey("dbo.MaterialsSalesOrders", "SupplierId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MaterialsSalesOrders", "SupplierId", "dbo.AspNetUsers");
            DropIndex("dbo.MaterialsSalesOrders", new[] { "SupplierId" });
            DropColumn("dbo.MaterialsSalesOrders", "SupplierId");
        }
    }
}
