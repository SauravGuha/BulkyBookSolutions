using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using static BulkyBook.Common.Constants;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{nameof(Roles.SuperAdmin)},{nameof(Roles.Admin)}")]
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            IEnumerable<Company> companies = _unitOfWork.CompanyRepository.GetAll();
            return View(companies);
        }

        public IActionResult CreateEdit(int? id)
        {
            //Add
            if (id == null)
            {
                var company = new Company();
                return View(company);
            }
            else
            {
                var company = _unitOfWork.CompanyRepository.Find(id);
                if (company == null)
                {
                    TempData["Error"] = $"No Company found with id={id}";
                }
                else
                {
                    return View(company);
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
                var company = _unitOfWork.CompanyRepository.Find(id);
                if (company == null)
                {
                    TempData["Error"] = $"Invalid id {id}.";
                }
                else
                {
                    _unitOfWork.CompanyRepository.Delete(company);
                    _unitOfWork.SaveChanges();
                    TempData["Success"] = $"Deleted {id} successfully.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUpdate(Company company)
        {
            var success = true;
            if (company.Id == 0)
            {
                if (ModelState.IsValid)
                {
                    var existingCompany = _unitOfWork.CompanyRepository.GetByExpression(e => e.Name == company.Name);
                    if (existingCompany != null)
                    {
                        TempData["Error"] = $"Company {company.Name} already exists.";
                        success = false;
                    }
                    else
                    {
                        var isValid = this.IsCompanyNameValid(company);
                        if (isValid)
                        {
                            this._unitOfWork.CompanyRepository.Insert(company);
                            this._unitOfWork.SaveChanges();
                            TempData["Success"] = $"Company {company.Name} created successfully.";
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
                var isValid = this.IsCompanyNameValid(company);
                if (isValid)
                {
                    this._unitOfWork.CompanyRepository.Update(company);
                    this._unitOfWork.SaveChanges();
                    TempData["Success"] = $"Company {company.Name} updated successfully.";
                }
                else
                {
                    ModelState.AddModelError(nameof(Company.Name), "Name is Invalid");
                    success = false;
                }
            }
            if (success)
                return RedirectToAction(nameof(Index));
            else
                return View(nameof(CreateEdit), company);
        }

        private bool IsCompanyNameValid(Company company)
        {
            var regexMatch = Regex.Match(company.Name, @"([~|!])");
            return !regexMatch.Success;
        }
    }
}
