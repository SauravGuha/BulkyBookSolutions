using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeatureController : Controller
    {

        private IUnitOfWork _unitOfWork;

        public FeatureController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var feature = this._unitOfWork.FeatureRepository.GetAll();
            return View(feature);
        }

        public IActionResult CreateEdit(int? id)
        {
            //Add
            if (id == null)
            {
                return View(new Feature());
            }
            else
            {
                var coverType = _unitOfWork.FeatureRepository.Find(id);
                if (coverType == null)
                {
                    TempData["Error"] = $"No feature found with id={id}.";
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
                var feature = _unitOfWork.FeatureRepository.Find(id);
                if (feature == null)
                {
                    TempData["Error"] = $"Invalid id {id}.";
                }
                else
                {
                    _unitOfWork.FeatureRepository.Delete(feature);
                    _unitOfWork.SaveChanges();
                    TempData["Success"] = $"Deleted {id} successfully.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUpdate(Feature feature)
        {
            var success = true;
            if (feature.Id == 0)
            {
                if (ModelState.IsValid)
                {
                    var existingCoverType = _unitOfWork.FeatureRepository
                        .GetByExpression(e => e.ControllerName == feature.ControllerName);
                    if (existingCoverType != null)
                    {
                        TempData["Error"] = $"Feature {feature.ControllerName} already exists.";
                        success = false;
                    }
                    else
                    {
                        var isValid = this.IsFeatureNameValid(feature);
                        if (isValid)
                        {
                            this._unitOfWork.FeatureRepository.Insert(feature);
                            this._unitOfWork.SaveChanges();
                            TempData["Success"] = $"Feature {feature.ControllerName} created successfully.";
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(Feature.ControllerName), "Name is Invalid");
                            success = false;
                        }
                    }
                }
                else
                    success = false;
            }
            else
            {
                var isValid = this.IsFeatureNameValid(feature);
                if (isValid)
                {
                    this._unitOfWork.FeatureRepository.Update(feature);
                    this._unitOfWork.SaveChanges();
                    TempData["Success"] = $"Feature {feature.ControllerName} updated successfully.";
                }
                else
                {
                    ModelState.AddModelError(nameof(Feature.ControllerName), "Name is Invalid");
                    success = false;
                }
            }
            if (success)
                return RedirectToAction(nameof(Index));
            else
                return View(nameof(CreateEdit), feature);
        }

        private bool IsFeatureNameValid(Feature feature)
        {
            var regexMatch = Regex.Match(feature.ControllerName, @"(\W)");
            return !regexMatch.Success;
        }

    }
}
