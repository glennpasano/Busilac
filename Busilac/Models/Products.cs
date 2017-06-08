using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Busilac.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        public int isVoid { get; set; }
        [Display(Name = "Critical Level")]
        public int CriticalLevelQuantity { get; set; }
        [Display(Name = "Normal Level")]
        public int NormalLevelQuantity { get; set; }
    }

    public class ProductInventory
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual Products Product { get; set; }
    }

    public class ProductBuildMaterials
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int MaterialId { get; set; }
        public int Quantity { get; set; }

        public virtual Products Products { get; set; }
        public virtual Materials Materials { get; set; }
    }

    public class ProductSalesOrders
    {
        [Key]
        public int ProductSalesOrdersId { get; set; }
        [Required]
        [Display(Name = "Date Ordered")]
        public DateTime OrderDate { get; set; }
        public int StatusId { get; set; }
        [Required]
        [Display(Name = "Client")]
        public string ClientName { get; set; }

        public virtual ProductStatus ProductStatus { get; set; }
        public virtual List<ProductSalesOrders> ProductSalesOrdersList { get; set; }
    }

    public class ProductSalesOrderDetails
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductSalesOrdersId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }

        public virtual ProductSalesOrders ProductSalesOrders { get; set; }
        public virtual Products Products { get; set; }
    }

    public class ProductManufactureOrders
    {
        [Key]
        public int ProductManufactureOrderId { get; set; }
        [Required]
        [Display(Name = "Date Requested")]
        public DateTime RequestDate { get; set; }
        public int StatusId { get; set; }
        public int isVoid { get; set; }

        public virtual ProductStatus ProductStatus { get; set; }
    }

    public class ProductManufactureOrderDetails
    {
        [Key]
        public int Id { get; set; }
        public int ProductManufactureOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual ProductManufactureOrders ProductManufactureOrders { get; set; }
        public virtual Products Products { get; set; }
    }

    public class ProductStatus
    {
        [Key]
        public int StatusId { get; set; }
        public string StatusName { get; set; }
    }
}