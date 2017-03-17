namespace Busilac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedMaterialsColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Materials", "CostPerWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Materials", "CostPerGram");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Materials", "CostPerGram", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Materials", "CostPerWeight");
        }
    }
}
