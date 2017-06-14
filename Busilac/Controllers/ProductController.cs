using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Busilac.Controllers
{
    [Authorize(Roles = "Admin, Admin")]
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Product
        public ActionResult Index()
        {
            var productList = new List<ProductViewModel>();

            foreach (var item in db.Products.Where(m => m.isVoid == 0).ToList())
            {
                var product = new ProductViewModel();

                product.Products = item;

                foreach (var buildItem in db.ProductBuildMaterials.Where(m => m.ProductId == item.ProductId).ToList())
                {
                    product.BuildMaterialsList += string.Format("{0}, ", buildItem.Materials.Name);
                }

                product.BuildMaterialsList = product.BuildMaterialsList.Length > 0 ? product.BuildMaterialsList.TrimEnd(',', ' ') : "";

                productList.Add(product);
            }

            return View(productList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var createProductViewModel = new CreateProductsViewModel();
            createProductViewModel.MaterialsList = db.Materials.Where(m => m.isVoid == 0).ToList();

            return View(createProductViewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateProductsViewModel createProductsViewModel)
        {
            if (ModelState.IsValid && createProductsViewModel.ProductBuildMaterials.Any(m => m.Quantity > 0 &&
                createProductsViewModel.Products.NormalLevelQuantity > createProductsViewModel.Products.CriticalLevelQuantity))
            {
                var product = new Products();

                product = createProductsViewModel.Products;
                db.Products.Add(product);

                foreach (var item in createProductsViewModel.ProductBuildMaterials.Where(m => m.Quantity > 0).ToList())
                {
                    var pbm = new ProductBuildMaterials()
                    {
                        Products = product,
                        MaterialId = item.MaterialId,
                        Quantity = item.Quantity
                    };
                    db.ProductBuildMaterials.Add(pbm);
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("error", "Error saving! Please double check inputs. Negative values are not allowed.");
            createProductsViewModel.MaterialsList = db.Materials.Where(m => m.isVoid == 0).ToList();

            return View(createProductsViewModel);
        }

        // Todo: Edit 
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var createProductViewModel = new CreateProductsViewModel();
            createProductViewModel.Products = db.Products.Where(m => m.ProductId == id).First();
            createProductViewModel.ProductBuildMaterials = db.ProductBuildMaterials.Where(m => m.ProductId == id)
                                                               .Select(m => new ProductBuildMaterialViewModel
                                                               {
                                                                   Quantity = m.Quantity,
                                                                   MaterialId = m.MaterialId
                                                               }).ToList();
            createProductViewModel.MaterialsList = db.Materials.Where(m => m.isVoid == 0).ToList();

            return View(createProductViewModel);
        }

        [HttpPost]
        public ActionResult Edit(CreateProductsViewModel createProductsViewModel)
        {
            var product = new Products();

            product = createProductsViewModel.Products;
            db.Products.Add(product);

            foreach (var item in createProductsViewModel.ProductBuildMaterials.Where(m => m.Quantity > 0))
            {
                var pbm = new ProductBuildMaterials();
                pbm.Products = product;
                pbm.MaterialId = item.MaterialId;
                pbm.Quantity = item.Quantity;

                db.ProductBuildMaterials.Add(pbm);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Todo: Delete Products

        public ActionResult Inventory()
        {
            List<ProductsInventoryViewModel> inventoryList = db.ProductInventory
                                                                .GroupBy(m => m.Product)
                                                                .Select(group => new ProductsInventoryViewModel
                                                                {
                                                                    Products = group.Key,
                                                                    TotalProductCount = group.Sum(x => x.Quantity)
                                                                }).ToList();

            return View(inventoryList);
        }
    }
}