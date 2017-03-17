namespace Busilac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRequiredFieldsProducts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.ProductSalesOrders", "ClientName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductSalesOrders", "ClientName", c => c.String());
            AlterColumn("dbo.Products", "Name", c => c.String());
        }
    }
}
