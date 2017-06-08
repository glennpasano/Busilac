using Busilac.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Busilac.ViewModels
{
    public class ClientOrdersViewModels
    {
        public List<ProductSalesOrderDetailsViewModels> ProductSalesOrderDetails { get; set; }
        public List<MaterialsInventoryViewModel> MaterialsInventory { get; set; }
    }

    public class ProductSalesOrderDetailsViewModels
    {
        public ProductSalesOrders ProductSalesOrders { get; set; }
        public string ProductListString { get; set; }
    }

    public class CreateProductOrdersViewModels
    {
        public ProductSalesOrders ProductSalesOrder { get; set; }
        public List<ProductSalesOrderDetails> ProductSalesOrderDetails { get; set; }
        public List<Products> ProductList { get; set; }
    }

    public class ListProductOrdersViewModel
    {
        public ProductSalesOrders ProductSalesOrder { get; set; }
        public List<ProductSalesOrderDetails> ProductSalesOrderDetails { get; set; }
        public string OrderDateString { get; set; }
    }

    public class ApproveProductOrdersViewModels
    {
        public ProductSalesOrders ProductSalesOrder { get; set; }
        public List<ProductSalesOrderDetails> ProductSalesOrderDetails { get; set; }
        public List<ProductsInventoryViewModel> ProductsInventory { get; set; }
    }
}