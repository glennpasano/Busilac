using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    [Authorize(Roles = "Warehouse, Admin")]
    public class WarehouseController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        NotificationsController notification = new NotificationsController();

        public ActionResult Index()
        {
            var homeViewModel = new WarehouseHomeViewModel()
            {
                MaterialsInventory = db.MaterialsInventory
                                        .GroupBy(m => m.Materials)
                                        .Select(group => new MaterialsInventoryViewModel
                                        {
                                            Material = group.Key,
                                            TotalMaterialWeight = group.Sum(x => x.Weight)
                                        }).ToList(),

                ProductInventory = db.ProductInventory
                                        .GroupBy(m => m.Product)
                                        .Select(group => new ProductsInventoryViewModel
                                        {
                                            Products = group.Key,
                                            TotalProductCount = group.Sum(m => m.Quantity)
                                        }).ToList(),
                SupplierList = db.Users.Where(m => m.Roles.Any(x => x.RoleId == "2" && x.RoleId != "1"))
                                        .Select(m => new SupplierListViewModel()
                                        {
                                            Name = m.UserName,
                                            Id = m.Id
                                        }).ToList()
            };

            return View(homeViewModel);
        }


        public ActionResult MaterialSalesOrders()
        {
            var msolvm = new List<MaterialSalesOrdersListViewModel>();

            // Get All Sales Orders that are Delivered (StatusId = 3) and Delivering (StatusId = 1002)
            foreach (var item in db.MaterialsSalesOrders.Where(m => m.StatusId == 1003 || m.StatusId == 1002 || m.StatusId == 4).OrderBy(m => m.StatusId).ToList())
            {
                var solvm = new MaterialSalesOrdersListViewModel();
                solvm.MaterialSalesOrders = item;

                foreach (var detail in db.MaterialsSalesOrdersDetails.Where(m => m.MaterialSalesOrdersId == item.MaterialSalesOrdersId).ToList())
                {
                    solvm.MaterialsList += string.Format("{0}({1}kg), ", detail.Materials.Name, detail.Weight);
                }
                solvm.MaterialsList = solvm.MaterialsList.TrimEnd(',', ' ');

                msolvm.Add(solvm);
            }

            return View(msolvm);
        }

        [HttpGet]
        public ActionResult ApproveDelivery(int id)
        {
            CreateSalesOrderViewModel csovm = new CreateSalesOrderViewModel();

            csovm.MaterialsSalesOrders = db.MaterialsSalesOrders.First(m => m.MaterialSalesOrdersId == id);
            csovm.MaterialsSalesOrdersDetails = db.MaterialsSalesOrdersDetails.Where(m => m.MaterialSalesOrdersId == id).ToList();

            return View(csovm);
        }

        [HttpPost]
        public ActionResult ApproveDelivery(CreateSalesOrderViewModel csovm)
        {
            var mso = db.MaterialsSalesOrders.FirstOrDefault(m => m.MaterialSalesOrdersId == csovm.MaterialsSalesOrders.MaterialSalesOrdersId);

            if (mso != null)
            {
                CheckDiscrepancy(csovm.MaterialsSalesOrders, csovm.MaterialsSalesOrdersDetails);
                UpdateMaterialsInventory(csovm.MaterialsSalesOrdersDetails);

                mso.StatusId = 1003; // On Approval; Change to "Delivered" status
                db.SaveChanges();
            }

            return RedirectToAction("MaterialSalesOrders");
        }

        public ActionResult ManageProductionOrders()
        {
            var productOrdersList = new List<ProductOrdersListViewModel>();

            foreach (var item in db.ProductManufactureOrders.ToList())
            {
                var pmo = new ProductOrdersListViewModel();
                pmo.ProductManufactureOrders = item;

                var prodDetails = db.ProductManufactureOrderDetails
                                    .Where(m => m.ProductManufactureOrderId == item.ProductManufactureOrderId)
                                    .ToList();

                foreach (var details in prodDetails)
                {
                    pmo.ProductsList += string.Format("{0}({1}), ", details.Products.Name, details.Quantity);
                }
                pmo.ProductsList = pmo.ProductsList.TrimEnd(',', ' ');

                productOrdersList.Add(pmo);
            }

            return View(productOrdersList);
        }

        [HttpGet]
        public ActionResult CreateProductionOrder()
        {
            var createProductsVM = new CreateProductOrdersViewModel();
            createProductsVM = CreateProductOrderViewModelData(createProductsVM);

            return View(createProductsVM);
        }

        [HttpPost]
        public ActionResult CreateProductionOrder(CreateProductOrdersViewModel cpovm)
        {

            var pmo = new ProductManufactureOrders();
            pmo.RequestDate = DateTime.Now;
            pmo.StatusId = 1;

            db.ProductManufactureOrders.Add(pmo);

            foreach (var item in cpovm.ProductManufactureOrderDetailsList.Where(m => m.Quantity > 0))
            {
                var pmod = new ProductManufactureOrderDetails();

                // Get Product Materials
                var productMaterials = db.ProductBuildMaterials.Where(m => m.ProductId == item.ProductId).ToList();
                // Check if we have enough materials for the product before proceeding with Order
                foreach (var mats in productMaterials)
                {
                    var materialInventory = db.MaterialsInventory.Where(m => m.MaterialId == mats.MaterialId).ToList();
                    decimal materialInventoryCount;

                    materialInventoryCount = materialInventory.Any() ? materialInventory.Sum(m => m.Weight) : 0;

                    if ((item.Quantity * mats.Quantity) > materialInventoryCount)
                    {
                        ModelState.AddModelError("Error", "Saving Failed! Not enough Materials");
                        // build the product and materials list again.
                        var createProductsVM = new CreateProductOrdersViewModel();
                        createProductsVM = CreateProductOrderViewModelData(createProductsVM);

                        return View(createProductsVM);
                    }

                }

                pmod.ProductManufactureOrders = pmo;
                pmod = item;

                db.ProductManufactureOrderDetails.Add(pmod);
            }

            // Notify and Email Supplier
            // Find all Suppliers
            var suppliers = db.Roles.Where(m => m.Name == "Manufacturing").ToList();

            foreach (var item in suppliers)
            {
                foreach (var user in item.Users)
                {
                    notification.NotifyUser(new Notifications
                    {
                        NotificationMessage = "New Production Order has been assigned to you.",
                        UserId = user.UserId
                    });
                }
            }

            db.SaveChanges();

            return RedirectToAction("ManageProductionOrders");
        }

        [HttpPost]
        public ActionResult OrderMaterial(float price, float weight, int materialId, string supplierId)
        {
            // Create sales order
            var som = new MaterialsSalesOrders()
            {
                OrderDate = DateTime.Now,
                StatusId = 1,
                SupplierId = supplierId
            };
            db.MaterialsSalesOrders.Add(som);

            // Assign material to sales order detail
            var somDetails = new MaterialsSalesOrdersDetails()
            {
                MaterialSalesOrders = som,
                MaterialId = materialId,
                Price = (decimal) price,
                Weight = (decimal) weight
            };
            db.MaterialsSalesOrdersDetails.Add(somDetails);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult OrderProductManufacture(int productId, int quantity)
        {
            if(quantity <= 0) {
                ModelState.AddModelError("ErrorAddProduct", "Invalid Product Quantity");

                return View();
            }

            var pmo = new ProductManufactureOrders()
            {
                StatusId = 1,
                RequestDate = DateTime.Now
            };
            db.ProductManufactureOrders.Add(pmo);

            var pmod = new ProductManufactureOrderDetails()
            {
                ProductManufactureOrders = pmo,
                ProductId = productId,
                Quantity = quantity
            };
            db.ProductManufactureOrderDetails.Add(pmod);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // APIs
        [AllowAnonymous]
        public JsonResult GetMaterial(int materialId)
        {
            return Json(new
            {
                Material = db.Materials.First(m => m.MaterialId == materialId),
                TotalInventory = db.MaterialsInventory.Where(m => m.MaterialId == materialId).Sum(m => m.Weight)
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetProduct(int productId)
        {
            return Json(new
            {
                Name = db.Products.First(m => m.ProductId == productId).Name,
                TotalInventory = db.ProductInventory.Where(m => m.ProductId == productId).Sum(m => m.Quantity),
                NormalQuantity = db.Products.First(m => m.ProductId == productId).NormalLevelQuantity
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetSuppliers()
        {
            return Json(new
            {
                Suppliers = db.Users.Where(m => m.Roles.Any(x => x.RoleId == "2" && x.RoleId != "1"))
                                .Select(m => new {
                                    Name = m.UserName,
                                    Id = m.Id
                                })
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPendingSalesOrders()
        {
            var materialsSalesOrders = new List<MaterialSalesOrdersListViewModel>();

            foreach (var so in db.MaterialsSalesOrders.Where(m => m.StatusId == 1).ToList())
            {
                var msovm = new MaterialSalesOrdersListViewModel();
                var materialsList = "";

                msovm.MaterialSalesOrders = so;

                foreach (var details in db.MaterialsSalesOrdersDetails.Where(m => m.MaterialSalesOrdersId == so.MaterialSalesOrdersId).ToList())
                {
                    materialsList += string.Format("{0} ({1}kg), ", details.Materials.Name, details.Weight);
                }

                msovm.MaterialsList = materialsList.TrimEnd(',', ' ');
                msovm.OrderDateString = so.OrderDate.ToShortDateString();

                materialsSalesOrders.Add(msovm);
            }

            var manufactureOrders = new List<ManufacturingProductionOrdersViewModel>();

            foreach (var mo in db.ProductManufactureOrders.Where(m => m.StatusId == 1).ToList())
            {
                var pso = new ManufacturingProductionOrdersViewModel();
                var productList = "";

                pso.ProductManufactureOrders = mo;

                foreach (var mod in db.ProductManufactureOrderDetails.Where(m => m.ProductManufactureOrderId == mo.ProductManufactureOrderId).ToList())
                {
                    productList += string.Format("{0}({1}), ", mod.Products.Name, mod.Quantity);
                }

                pso.OrderDetailsString = productList.TrimEnd(',', ' ');
                pso.OrderDateString = mo.RequestDate.ToShortDateString();

                manufactureOrders.Add(pso);
            }

            return Json(new
            {
                Materials = materialsSalesOrders,
                Products = manufactureOrders
            }, JsonRequestBehavior.AllowGet);
        }

        #region Helpers

        private CreateProductOrdersViewModel CreateProductOrderViewModelData(CreateProductOrdersViewModel createProductsVM)
        {
            createProductsVM.ProductsAndMaterialsList = new List<ProductMaterialBuildsViewModel>();
            createProductsVM.AvailableMaterialsViewModel = new List<AvailableMaterialsViewModel>();

            foreach (var item in db.Products.Where(m => m.isVoid == 0).ToList())
            {
                var productsBuild = new ProductMaterialBuildsViewModel();
                productsBuild.ProductBuildMaterials = new List<ProductBuildMaterials>();
                productsBuild.Product = item;

                foreach (var mats in db.ProductBuildMaterials.Where(m => m.ProductId == item.ProductId).ToList())
                {
                    productsBuild.ProductBuildMaterials.Add(mats);
                    productsBuild.ProductBuildMaterialsString += string.Format("{0}({1}), ", mats.Materials.Name, mats.Quantity);
                }
                productsBuild.ProductBuildMaterialsString = productsBuild.ProductBuildMaterialsString.TrimEnd(',', ' ');
                createProductsVM.ProductsAndMaterialsList.Add(productsBuild);
            }

            foreach (var item in db.Materials.Where(m => m.isVoid == 0).ToList())
            {
                var availableMaterials = new AvailableMaterialsViewModel();
                var totalInventory = db.MaterialsInventory.Where(m => m.MaterialId == item.MaterialId).ToList();

                availableMaterials.Materials = item;
                availableMaterials.TotalInventoryCount = totalInventory.Any() ? totalInventory.Sum(m => m.Weight) : 0;

                createProductsVM.AvailableMaterialsViewModel.Add(availableMaterials);
            }

            return createProductsVM;
        }

        private void UpdateMaterialsInventory(List<MaterialsSalesOrdersDetails> actualDetails)
        {
            if(actualDetails.Any())
            {
                foreach (var item in actualDetails)
                {
                    var matInventory = new MaterialsInventory();

                    matInventory.MaterialId = item.MaterialId;
                    matInventory.Weight = item.Weight;

                    db.MaterialsInventory.Add(matInventory);
                }

                db.SaveChanges();
            }
        }

        private void CheckDiscrepancy(MaterialsSalesOrders mso, List<MaterialsSalesOrdersDetails> actualDetails)
        {
            // Get current Material Sales Order
            MaterialsSalesOrders salesOrder = db.MaterialsSalesOrders
                                                .Where(m => m.MaterialSalesOrdersId == mso.MaterialSalesOrdersId)
                                                .FirstOrDefault();

            List<MaterialsSalesOrdersDetails> salesOrderDetails = db.MaterialsSalesOrdersDetails
                                                                    .Where(m => m.MaterialSalesOrdersId == mso.MaterialSalesOrdersId)
                                                                    .ToList();

            // If something changed, create new SalesOrder with follow up status
            var newSalesOrder = new MaterialsSalesOrders();
            var newSalesOrdersDetailsList = new List<MaterialsSalesOrdersDetails>();
            
            foreach (var item in salesOrderDetails)
            {
                foreach (var actlItem in actualDetails)
                {
                    if(item.Id == actlItem.Id)
                    {
                        // Check if delivered items were complete
                        if(actlItem.Weight < item.Weight)
                        {
                            var owedValue = item.Weight - actlItem.Weight;
                            var newOrderDetail = new MaterialsSalesOrdersDetails();

                            newOrderDetail.MaterialId = item.MaterialId;
                            newOrderDetail.Weight = owedValue;

                            newSalesOrdersDetailsList.Add(newOrderDetail);
                        }
                    }
                }
            }

            if(newSalesOrdersDetailsList.Any())
            {
                newSalesOrder.OrderDate = DateTime.Now;
                newSalesOrder.StatusId = 1002; // Status = Follow up. Changed to Delivering
                newSalesOrder.SupplierId = salesOrder.SupplierId;

                db.MaterialsSalesOrders.Add(newSalesOrder);

                foreach (var item in newSalesOrdersDetailsList)
                {
                    item.MaterialSalesOrders = newSalesOrder;
                    item.Price = 0;
                    db.MaterialsSalesOrdersDetails.Add(item);
                }

                db.SaveChanges();
            }
        }
    #endregion

    }
}