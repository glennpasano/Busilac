namespace Busilac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMaterialType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MaterialTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Materials", "TypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Materials", "TypeId");
            AddForeignKey("dbo.Materials", "TypeId", "dbo.MaterialTypes", "Id", cascadeDelete: true);
            DropColumn("dbo.Materials", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Materials", "Type", c => c.String());
            DropForeignKey("dbo.Materials", "TypeId", "dbo.MaterialTypes");
            DropIndex("dbo.Materials", new[] { "TypeId" });
            DropColumn("dbo.Materials", "TypeId");
            DropTable("dbo.MaterialTypes");
        }
    }
}
