using Busilac.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Busilac.ViewModels
{
    public class ManufacturingViewModels
    {
    }

    public class ManufacturingProductionOrdersViewModel
    {
        public ProductManufactureOrders ProductManufactureOrders { get; set; }
        public string OrderDetailsString { get; set; }
        public string OrderDateString { get; set; }
    }

    public class OrderedProductsViewModel
    {
        public Products Product { get; set; }
        public int ProductOrderedTotalCount { get; set; }
    }

    public class OrderedProductsApprovalViewModel
    {
        public ProductManufactureOrders ProductManufactureOrders { get; set; }
        public List<ProductManufactureOrderDetails> ProductManufactureOrderDetailsList { get; set; }
        public List<ProductOrderApproveViewModels> ProductMaterials { get; set; }
        public List<AvailableMaterialsViewModel> AvailableMaterialsViewModel { get; set; }
    }

    public class ProductOrderApproveViewModels
    {
        public Products Product { get; set; }
        public string ProductMaterialsString { get; set; }
        public string ProductMaterialsStringTotals { get; set; }
        public int QuantityOrdered { get; set; }
    }


}