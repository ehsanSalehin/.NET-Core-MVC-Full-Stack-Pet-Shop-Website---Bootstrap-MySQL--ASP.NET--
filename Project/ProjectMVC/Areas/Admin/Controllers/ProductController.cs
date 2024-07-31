using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.DataAccess.Data;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using Project.Models.ViewModels;
using Project.Utilities;


namespace ProjectMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Details1.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _db;
        private readonly IWebHostEnvironment _webHostEnvirenment;
        public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvirenment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _db.Product.GetAll(includeProperties:"Category").ToList();

            return View(objProductList);
        }
        public IActionResult Create(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _db.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            ProductVM productVM = new()
            {
                Category = CategoryList,
                Product = new Product()
            };
            if (id == null || id ==0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _db.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }
        //post method for create:
        [HttpPost]
        public IActionResult Create(ProductVM obj, IFormFile? file)
        {
            
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvirenment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"Images\Product");
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Product.Image = @"\Images\Product\" + fileName;

                }
                _db.Product.Add(obj.Product);
                _db.Save();
                TempData["success"] = "Product Created";
                return RedirectToAction("Index");
            }
            else
            {
                obj.Category = _db.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }
        }


        //edit
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product productDb = _db.Product.Get(u => u.Id == id, includeProperties: "Category");
            if (productDb == null)
            {
                return NotFound();
            }
            IEnumerable<SelectListItem> CategoryList = _db.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            ProductVM productVM = new ProductVM
            {
                Product = productDb,
                Category = CategoryList
            };

            return View(productVM);
        }
        //post method for edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // Check if the CategoryId exists
                var categoryExists = _db.Category.GetAll().Any(c => c.Id == obj.Product.CategoryId);
                if (!categoryExists)
                {
                    ModelState.AddModelError("Product.CategoryId", "Selected category does not exist.");
                    obj.Category = _db.Category.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                    return View(obj);
                }

                // File handling code
                string wwwRootPath = _webHostEnvirenment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"Images\Product");

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(obj.Product.Image))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, obj.Product.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save new image
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    obj.Product.Image = @"\Images\Product\" + fileName;
                }

                _db.Product.Update(obj.Product);
                _db.Save();
                TempData["success"] = "Product Updated Successfully";
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            obj.Category = _db.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }

        //delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product productDb = _db.Product.Get(u => u.Id == id);
            if (productDb == null)
            {
                return NotFound();
            }
            return View(productDb);
        }
        //post method for delete
        [HttpPost, ActionName("delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? obj = _db.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Product.Remove(obj);
            _db.Save();
            TempData["success"] = "Product Deleted";
            return RedirectToAction("Index");

        }

    }
}
