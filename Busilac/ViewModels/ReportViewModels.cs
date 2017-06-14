using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Busilac.ViewModels;
using Busilac.Models;

namespace Busilac.ViewModels
{
    public class ReportViewModels
    {
    }

    public class OrderedMaterialsReportViewModel
    {
        public List<OrderedMaterialsViewModel> OrderedMaterials { get; set; }
        public List<ReportDropdownViewModel> MaterialsList { get; set; }
    }

    public class OrderedMaterialsViewModel
    {
        public string OrderedDate { get; set; }
        public string MaterialName { get; set; }
        public string MaterialType { get; set; }
        public decimal Weight { get; set; }
    }

    public class SoldProductsReportViewModel
    {
        public List<SoldProductsViewModel> SoldProductsList { get; set; }
        public List<ReportDropdownViewModel> ProductList { get; set; }
    }

    public class SoldProductsViewModel
    {
        public string DateOrdered { get; set; }
        public string ClientName { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
    }

    public class ReportDropdownViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class DonutChartViewModel
    {
        public string label { get; set; }
        public int value { get; set; }
    }

}