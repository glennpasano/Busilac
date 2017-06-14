using Busilac.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Busilac.ViewModels
{

    public class ProductViewModel
    {
        public Products Products { get; set; }
        [Display(Name = "Materials")]
        public string BuildMaterialsList { get; set; }
    }

    public class CreateProductsViewModel
    {
        public Products Products { get; set; }
        public List<ProductBuildMaterialViewModel> ProductBuildMaterials { get; set; }
        public List<Materials> MaterialsList { get; set; }
    }

    public class ProductBuildMaterialViewModel
    {
        [Required]
        public int MaterialId { get; set; }
        [Required]
        [Range(0, 99999999, ErrorMessage = "Must be greater than 0")]
        public int Quantity { get; set; }
    }

    public class ProductsInventoryViewModel
    {
        public Products Products { get; set; }
        public double TotalProductCount { get; set; }
    }
}