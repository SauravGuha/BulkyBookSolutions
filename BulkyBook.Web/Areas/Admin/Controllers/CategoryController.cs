using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using static BulkyBook.Common.Constants;

namespace BulkyBook.Web.Controllers
{
    [Authorize(Roles = nameof(Roles.SuperAdmin))]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            IEnumerable<Category> categories = unitOfWork.CategoryRepository.GetAll();
            return View(categories);
        }

        public IActionResult CreateEdit(int? id)
        {
            //Add
            if (id == null)
            {
                var category = new Category();
                return View(category);
            }
            else
            {
                var category = unitOfWork.CategoryRepository.Find(id);
                if (category == null)
                {
                    TempData["Error"] = $"No Category found with id={id}";
                }
                else
                {
                    return View(category);
                }
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = $"Invalid id {id}.";
            }
            else
            {
                var category = unitOfWork.CategoryRepository.Find(id);
                if (category == null)
                {
                    TempData["Error"] = $"Invalid id {id}.";
                }
                else
                {
                    unitOfWork.CategoryRepository.Delete(category);
                    unitOfWork.SaveChanges();
                    TempData["Success"] = $"Deleted {id} successfully.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUpdate(Category category)
        {
            var success = true;
            if (category.Id == 0)
            {
                if (ModelState.IsValid)
                {
                    var existingCategory = unitOfWork.CategoryRepository.GetByExpression(e => e.Name == category.Name);
                    if (existingCategory != null)
                    {
                        TempData["Error"] = $"Category {category.Name} already exists.";
                        success = false;
                    }
                    else
                    {
                        var isValid = this.IsCategoryNameValid(category);
                        if (isValid)
                        {
                            this.unitOfWork.CategoryRepository.Insert(category);
                            this.unitOfWork.SaveChanges();
                            TempData["Success"] = $"Category {category.Name} created successfully.";
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(Category.Name), "Name is Invalid");
                            success = false;
                        }
                    }
                }
                else
                    success = false;
            }
            else
            {
                var isValid = this.IsCategoryNameValid(category);
                if (isValid)
                {
                    this.unitOfWork.CategoryRepository.Update(category);
                    this.unitOfWork.SaveChanges();
                    TempData["Success"] = $"Category {category.Name} updated successfully.";
                }
                else
                {
                    ModelState.AddModelError(nameof(Category.Name), "Name is Invalid");
                    success = false;
                }
            }
            if (success)
                return RedirectToAction(nameof(Index));
            else
                return View(nameof(CreateEdit), category);
        }

        private bool IsCategoryNameValid(Category category)
        {
            var regexMatch = Regex.Match(category.Name, @"(\W|\s)");
            return !regexMatch.Success;
        }
    }
}
