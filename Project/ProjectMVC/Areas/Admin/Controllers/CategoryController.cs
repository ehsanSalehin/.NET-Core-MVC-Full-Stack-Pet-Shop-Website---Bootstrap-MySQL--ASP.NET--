using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.DataAccess.Data;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using Project.Utilities;


namespace ProjectMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Details1.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _db;
        public CategoryController(IUnitOfWork db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Category.GetAll().ToList();
            return View(objCategoryList);
        }
        public IActionResult Create()
        {
            return View();
        }
        //post method for create:
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "the display order can not be simmilar to the name"); //asp-for = name ==>it should be name here
            }
            if (obj.Name != null && obj.Name.ToLower() == "test") //if they enter test for name or don't enter anything
            {
                ModelState.AddModelError("", "test is an invalid");
            }
            if (ModelState.IsValid)
            {
                _db.Category.Add(obj);
                _db.Save();
                TempData["success"] = "Category Created";
                return RedirectToAction("Index");
            }
            return View();
        }


        //edit
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category categoryDb = _db.Category.Get(u => u.Id == id);
            if (categoryDb == null)
            {
                return NotFound();
            }
            return View(categoryDb);
        }
        //post method for edit
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Add(obj);
                _db.Save();
                TempData["success"] = "Category Updated";
                return RedirectToAction("Index");
            }
            return View();
        }

        //delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category categoryDb = _db.Category.Get(u => u.Id == id);
            if (categoryDb == null)
            {
                return NotFound();
            }
            return View(categoryDb);
        }
        //post method for delete
        [HttpPost, ActionName("delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? obj = _db.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Category.Remove(obj);
            _db.Save();
            TempData["success"] = "Category Deleted";
            return RedirectToAction("Index");

        }

    }
}
