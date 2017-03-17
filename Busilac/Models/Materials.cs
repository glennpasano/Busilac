using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Busilac.Models
{
    public class Materials
    {
        [Key]
        public int MaterialId { get; set; }
        [Required]
        public int TypeId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int isVoid { get; set; }
        [Required]
        [Display(Name = "Cost")]
        public decimal CostPerWeight { get; set; }
        [Required]
        [Display(Name = "Critical Level")]
        public decimal CriticalLevelWeight { get; set; }

        public virtual MaterialType Type { get; set; }
    }

    public class MaterialType
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Type")]
        public string TypeName { get; set; }
    }

    public class MaterialsInventory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int MaterialId { get; set; }
        [Required]
        public decimal Weight { get; set; }

        public virtual Materials Materials { get; set; }
    }

    public class MaterialsSalesOrders
    {
        [Key]
        public int MaterialSalesOrdersId { get; set; }
        [Required]

        [Display(Name = "Date Ordered")]
        public DateTime OrderDate { get; set; }
        [Required]
        public int StatusId { get; set; }

        public virtual MaterialsStatus MaterialsStatus { get; set; }
    }

    public class MaterialsSalesOrdersDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int MaterialSalesOrdersId { get; set; }
        [Required]
        public int MaterialId { get; set; }
        [Required]
        public decimal Weight { get; set; }

        public virtual Materials Materials { get; set; }
        public virtual MaterialsSalesOrders MaterialSalesOrders { get; set; }
    }

    public class MaterialsDeliveryReceipt
    {
        [Key]
        public int MaterialsDeliveryReceiptId { get; set; }
        [Required]
        public int MaterialSalesOrdersId { get; set; }
        [Required]

        [Display(Name = "Date Delivered")]
        public DateTime DeliveryDate { get; set; }
        [Required]
        public int StatusId { get; set; }

        public virtual MaterialsStatus MaterialsStatus { get; set; }
    }

    public class MaterialsDeliveryReceiptDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int MaterialsDeliveryReceiptId { get; set; }
        [Required]
        public int MaterialId { get; set; }
        [Required]
        [Display(Name = "Actual Weight")]
        public decimal ActualWeightReceived { get; set; }

        public virtual MaterialsDeliveryReceipt MaterialsDeliveryReceipt { get; set; }
    }

    public class MaterialsStatus
    {
        [Key]
        public int StatusId { get; set; }

        [Display(Name = "Status")]
        public string StatusName { get; set; }
    }


}