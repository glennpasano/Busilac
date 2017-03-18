namespace Busilac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRecipientIdToMessages : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Messages", name: "User_Id", newName: "RecipientId");
            RenameIndex(table: "dbo.Messages", name: "IX_User_Id", newName: "IX_RecipientId");
            AddColumn("dbo.Messages", "SenderId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Messages", "SenderId");
            AddForeignKey("dbo.Messages", "SenderId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Messages", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "UserId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Messages", "SenderId", "dbo.AspNetUsers");
            DropIndex("dbo.Messages", new[] { "SenderId" });
            DropColumn("dbo.Messages", "SenderId");
            RenameIndex(table: "dbo.Messages", name: "IX_RecipientId", newName: "IX_User_Id");
            RenameColumn(table: "dbo.Messages", name: "RecipientId", newName: "User_Id");
        }
    }
}
