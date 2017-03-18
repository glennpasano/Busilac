namespace Busilac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncludedNotificationsAndMessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Message = c.String(),
                        isVoid = c.Int(nullable: false),
                        isRead = c.Int(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        NotificationId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        NotificationMessage = c.String(),
                        isRead = c.Int(nullable: false),
                        isVoid = c.Int(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.NotificationId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Notifications", new[] { "User_Id" });
            DropIndex("dbo.Messages", new[] { "User_Id" });
            DropTable("dbo.Notifications");
            DropTable("dbo.Messages");
        }
    }
}
