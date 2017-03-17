using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    public class MaterialsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Materials
        public ActionResult Index()
        {
            List<Materials> materialsList = db.Materials.Where(m => m.isVoid == 0).ToList();
            
            return View(materialsList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            CreateMaterialsViewModel createMaterialViewModel = new CreateMaterialsViewModel();
            createMaterialViewModel.TypesList = db.MaterialType.ToList();

            return View(createMaterialViewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateMaterialsViewModel createdMaterialsViewModel)
        {
            db.Materials.Add(createdMaterialsViewModel.Material);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Todo: Edit ActionResults

        // Todo: Delete ActionResults

        public ActionResult Inventory()
        {
            List<MaterialsInventoryViewModel> inventoryList = db.MaterialsInventory
                                                                .GroupBy(m => m.Materials)
                                                                .Select(group => new MaterialsInventoryViewModel
                                                                {
                                                                    Material = group.Key,
                                                                    TotalMaterialWeight = group.Sum(x => x.Weight)
                                                                }).ToList();

            return View(inventoryList);
        }
    }
}