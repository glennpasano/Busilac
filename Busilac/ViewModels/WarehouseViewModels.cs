using Busilac.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Busilac.ViewModels
{
    public class WarehouseViewModels
    {
    }

    public class WarehouseHomeViewModel
    {
        public List<MaterialsInventoryViewModel> MaterialsInventory { get; set; }
        public List<ProductsInventoryViewModel> ProductInventory { get; set; }
    }

    public class ProductOrdersListViewModel
    {
        public ProductManufactureOrders ProductManufactureOrders { get; set; }
        public string ProductsList { get; set; }
    }

    public class CreateProductOrdersViewModel
    {
        public ProductManufactureOrders ProductManufactureOrders { get; set; }
        public List<ProductManufactureOrderDetails> ProductManufactureOrderDetailsList { get; set; }

        public List<ProductMaterialBuildsViewModel> ProductsAndMaterialsList { get; set; }
        public List<AvailableMaterialsViewModel> AvailableMaterialsViewModel { get; set; }
    }

    public class ProductMaterialBuildsViewModel
    {
        public Products Product { get; set; }
        public List<ProductBuildMaterials> ProductBuildMaterials { get; set; }
        public string ProductBuildMaterialsString { get; set; }
    }

    public class AvailableMaterialsViewModel
    {
        public Materials Materials { get; set; }
        public decimal TotalInventoryCount { get; set; }
    }
}