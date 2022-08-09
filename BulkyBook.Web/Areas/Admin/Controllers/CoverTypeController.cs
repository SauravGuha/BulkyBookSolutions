using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var coverTypes = this._unitOfWork.CoverTypeRepository.GetAll();
            return View(coverTypes);
        }

        public IActionResult CreateEdit(int? id)
        {
            //Add
            if (id == null)
            {
                return View(new CoverType());
            }
            else
            {
                var coverType = _unitOfWork.CoverTypeRepository.Find(id);
                if (coverType == null)
                {
                    TempData["Error"] = $"No cover found with id={id}.";
                }
                else
                {
                    return View(coverType);
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
                var coverType = _unitOfWork.CoverTypeRepository.Find(id);
                if (coverType == null)
                {
                    TempData["Error"] = $"Invalid id {id}.";
                }
                else
                {
                    _unitOfWork.CoverTypeRepository.Delete(coverType);
                    _unitOfWork.SaveChanges();
                    TempData["Success"] = $"Deleted {id} successfully.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUpdate(CoverType coverType)
        {
            var success = true;
            if (coverType.Id == 0)
            {
                if (ModelState.IsValid)
                {
                    var existingCoverType = _unitOfWork.CoverTypeRepository.GetByExpression(e => e.Name == coverType.Name);
                    if (existingCoverType != null)
                    {
                        TempData["Error"] = $"Category {coverType.Name} already exists.";
                        success = false;
                    }
                    else
                    {
                        var isValid = this.IsCoverTypeNameValid(coverType);
                        if (isValid)
                        {
                            this._unitOfWork.CoverTypeRepository.Insert(coverType);
                            this._unitOfWork.SaveChanges();
                            TempData["Success"] = $"Category {coverType.Name} created successfully.";
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(CoverType.Name), "Name is Invalid");
                            success = false;
                        }
                    }
                }
                else
                    success = false;
            }
            else
            {
                var isValid = this.IsCoverTypeNameValid(coverType);
                if (isValid)
                {
                    this._unitOfWork.CoverTypeRepository.Update(coverType);
                    this._unitOfWork.SaveChanges();
                    TempData["Success"] = $"CoverType {coverType.Name} updated successfully.";
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
                return View(nameof(CreateEdit), coverType);
        }

        private bool IsCoverTypeNameValid(CoverType coverType)
        {
            var regexMatch = Regex.Match(coverType.Name, @"(\W|\s)");
            return !regexMatch.Success;
        }

    }
}
