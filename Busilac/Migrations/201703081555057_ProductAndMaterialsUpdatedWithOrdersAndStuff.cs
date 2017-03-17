namespace Busilac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductAndMaterialsUpdatedWithOrdersAndStuff : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Materials");
            DropPrimaryKey("dbo.Products");
            DropColumn("dbo.Materials", "Id");
            DropColumn("dbo.Products", "Id");
            CreateTable(
                "dbo.MaterialsDeliveryReceipts",
                c => new
                    {
                        MaterialsDeliveryReceiptId = c.Int(nullable: false, identity: true),
                        MaterialSalesOrdersId = c.Int(nullable: false),
                        DeliveryDate = c.DateTime(nullable: false),
                        StatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaterialsDeliveryReceiptId)
                .ForeignKey("dbo.MaterialsStatus", t => t.StatusId, cascadeDelete: true)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.MaterialsStatus",
                c => new
                    {
                        StatusId = c.Int(nullable: false, identity: true),
                        StatusName = c.String(),
                    })
                .PrimaryKey(t => t.StatusId);
            
            CreateTable(
                "dbo.MaterialsDeliveryReceiptDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaterialsDeliveryReceiptId = c.Int(nullable: false),
                        MaterialId = c.Int(nullable: false),
                        ActualWeightReceived = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MaterialsDeliveryReceipts", t => t.MaterialsDeliveryReceiptId, cascadeDelete: true)
                .Index(t => t.MaterialsDeliveryReceiptId);
            
            CreateTable(
                "dbo.MaterialsInventories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaterialId = c.Int(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Materials", t => t.MaterialId, cascadeDelete: true)
                .Index(t => t.MaterialId);
            
            CreateTable(
                "dbo.MaterialsSalesOrders",
                c => new
                    {
                        MaterialSalesOrdersId = c.Int(nullable: false, identity: true),
                        OrderDate = c.DateTime(nullable: false),
                        StatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaterialSalesOrdersId)
                .ForeignKey("dbo.MaterialsStatus", t => t.StatusId, cascadeDelete: true)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.MaterialsSalesOrdersDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaterialSalesOrdersId = c.Int(nullable: false),
                        MaterialId = c.Int(nullable: false),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Materials", t => t.MaterialId, cascadeDelete: true)
                .ForeignKey("dbo.MaterialsSalesOrders", t => t.MaterialSalesOrdersId, cascadeDelete: true)
                .Index(t => t.MaterialSalesOrdersId)
                .Index(t => t.MaterialId);
            
            CreateTable(
                "dbo.ProductBuildMaterials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        MaterialId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Materials", t => t.MaterialId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.MaterialId);
            
            CreateTable(
                "dbo.ProductInventories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductManufactureOrderDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductManufactureOrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductManufactureOrders", t => t.ProductManufactureOrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductManufactureOrderId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductManufactureOrders",
                c => new
                    {
                        ProductManufactureOrderId = c.Int(nullable: false, identity: true),
                        RequestDate = c.DateTime(nullable: false),
                        StatusId = c.Int(nullable: false),
                        isVoid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductManufactureOrderId)
                .ForeignKey("dbo.ProductStatus", t => t.StatusId, cascadeDelete: true)
                .Index(t => t.StatusId);
            
            CreateTable(
                "dbo.ProductStatus",
                c => new
                    {
                        StatusId = c.Int(nullable: false, identity: true),
                        StatusName = c.String(),
                    })
                .PrimaryKey(t => t.StatusId);
            
            CreateTable(
                "dbo.ProductSalesOrderDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ProductSalesOrdersId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.ProductSalesOrders", t => t.ProductSalesOrdersId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.ProductSalesOrdersId);
            
            CreateTable(
                "dbo.ProductSalesOrders",
                c => new
                    {
                        ProductSalesOrdersId = c.Int(nullable: false, identity: true),
                        OrderDate = c.DateTime(nullable: false),
                        StatusId = c.Int(nullable: false),
                        ClientName = c.String(),
                        ProductSalesOrders_ProductSalesOrdersId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductSalesOrdersId)
                .ForeignKey("dbo.ProductSalesOrders", t => t.ProductSalesOrders_ProductSalesOrdersId)
                .ForeignKey("dbo.ProductStatus", t => t.StatusId, cascadeDelete: true)
                .Index(t => t.StatusId)
                .Index(t => t.ProductSalesOrders_ProductSalesOrdersId);
            
            AddColumn("dbo.Materials", "MaterialId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Materials", "isVoid", c => c.Int(nullable: false));
            AddColumn("dbo.Materials", "CostPerGram", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Materials", "CriticalLevelWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "ProductId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Products", "isVoid", c => c.Int(nullable: false));
            AddColumn("dbo.Products", "CriticalLevelQuantity", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Materials", "MaterialId");
            AddPrimaryKey("dbo.Products", "ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Materials", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.ProductSalesOrderDetails", "ProductSalesOrdersId", "dbo.ProductSalesOrders");
            DropForeignKey("dbo.ProductSalesOrders", "StatusId", "dbo.ProductStatus");
            DropForeignKey("dbo.ProductSalesOrders", "ProductSalesOrders_ProductSalesOrdersId", "dbo.ProductSalesOrders");
            DropForeignKey("dbo.ProductSalesOrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductManufactureOrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductManufactureOrderDetails", "ProductManufactureOrderId", "dbo.ProductManufactureOrders");
            DropForeignKey("dbo.ProductManufactureOrders", "StatusId", "dbo.ProductStatus");
            DropForeignKey("dbo.ProductInventories", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductBuildMaterials", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductBuildMaterials", "MaterialId", "dbo.Materials");
            DropForeignKey("dbo.MaterialsSalesOrdersDetails", "MaterialSalesOrdersId", "dbo.MaterialsSalesOrders");
            DropForeignKey("dbo.MaterialsSalesOrdersDetails", "MaterialId", "dbo.Materials");
            DropForeignKey("dbo.MaterialsSalesOrders", "StatusId", "dbo.MaterialsStatus");
            DropForeignKey("dbo.MaterialsInventories", "MaterialId", "dbo.Materials");
            DropForeignKey("dbo.MaterialsDeliveryReceiptDetails", "MaterialsDeliveryReceiptId", "dbo.MaterialsDeliveryReceipts");
            DropForeignKey("dbo.MaterialsDeliveryReceipts", "StatusId", "dbo.MaterialsStatus");
            DropIndex("dbo.ProductSalesOrders", new[] { "ProductSalesOrders_ProductSalesOrdersId" });
            DropIndex("dbo.ProductSalesOrders", new[] { "StatusId" });
            DropIndex("dbo.ProductSalesOrderDetails", new[] { "ProductSalesOrdersId" });
            DropIndex("dbo.ProductSalesOrderDetails", new[] { "ProductId" });
            DropIndex("dbo.ProductManufactureOrders", new[] { "StatusId" });
            DropIndex("dbo.ProductManufactureOrderDetails", new[] { "ProductId" });
            DropIndex("dbo.ProductManufactureOrderDetails", new[] { "ProductManufactureOrderId" });
            DropIndex("dbo.ProductInventories", new[] { "ProductId" });
            DropIndex("dbo.ProductBuildMaterials", new[] { "MaterialId" });
            DropIndex("dbo.ProductBuildMaterials", new[] { "ProductId" });
            DropIndex("dbo.MaterialsSalesOrdersDetails", new[] { "MaterialId" });
            DropIndex("dbo.MaterialsSalesOrdersDetails", new[] { "MaterialSalesOrdersId" });
            DropIndex("dbo.MaterialsSalesOrders", new[] { "StatusId" });
            DropIndex("dbo.MaterialsInventories", new[] { "MaterialId" });
            DropIndex("dbo.MaterialsDeliveryReceiptDetails", new[] { "MaterialsDeliveryReceiptId" });
            DropIndex("dbo.MaterialsDeliveryReceipts", new[] { "StatusId" });
            DropPrimaryKey("dbo.Products");
            DropPrimaryKey("dbo.Materials");
            DropColumn("dbo.Products", "CriticalLevelQuantity");
            DropColumn("dbo.Products", "isVoid");
            DropColumn("dbo.Products", "ProductId");
            DropColumn("dbo.Materials", "CriticalLevelWeight");
            DropColumn("dbo.Materials", "CostPerGram");
            DropColumn("dbo.Materials", "isVoid");
            DropColumn("dbo.Materials", "MaterialId");
            DropTable("dbo.ProductSalesOrders");
            DropTable("dbo.ProductSalesOrderDetails");
            DropTable("dbo.ProductStatus");
            DropTable("dbo.ProductManufactureOrders");
            DropTable("dbo.ProductManufactureOrderDetails");
            DropTable("dbo.ProductInventories");
            DropTable("dbo.ProductBuildMaterials");
            DropTable("dbo.MaterialsSalesOrdersDetails");
            DropTable("dbo.MaterialsSalesOrders");
            DropTable("dbo.MaterialsInventories");
            DropTable("dbo.MaterialsDeliveryReceiptDetails");
            DropTable("dbo.MaterialsStatus");
            DropTable("dbo.MaterialsDeliveryReceipts");
            AddPrimaryKey("dbo.Products", "Id");
            AddPrimaryKey("dbo.Materials", "Id");
        }
    }
}
