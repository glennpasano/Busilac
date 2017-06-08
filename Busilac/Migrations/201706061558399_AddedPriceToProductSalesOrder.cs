namespace Busilac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPriceToProductSalesOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductSalesOrderDetails", "Price", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductSalesOrderDetails", "Price");
        }
    }
}
