using Busilac.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Busilac.ViewModels
{
    public class MaterialsViewModels
    {

    }

    public class CreateMaterialsViewModel
    {
        public IEnumerable<MaterialType> TypesList { get; set; }
        [Required]
        public Materials Material { get; set; }
    }

    public class MaterialsInventoryViewModel
    {
        public Materials Material { get; set; }
        public decimal TotalMaterialWeight { get; set; }

    }
}