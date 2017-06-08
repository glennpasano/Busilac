namespace Busilac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNormalWeightToProductAndMaterials : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Materials", "NormalLevelWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "NormalLevelQuantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "NormalLevelQuantity");
            DropColumn("dbo.Materials", "NormalLevelWeight");
        }
    }
}
