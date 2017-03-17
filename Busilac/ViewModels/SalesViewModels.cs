﻿using Busilac.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Busilac.ViewModels
{
    public class SalesViewModels
    {
    }

    public class MaterialSalesOrdersListViewModel
    {
        public MaterialsSalesOrders MaterialSalesOrders { get; set; }
        public string MaterialsList { get; set; }
    }

    public class CreateSalesOrderViewModel
    {
        public MaterialsSalesOrders MaterialsSalesOrders { get; set; }
        public List<MaterialsSalesOrdersDetails> MaterialsSalesOrdersDetails { get; set; }
        public List<Materials> MaterialsList { get; set; }
    }
}