using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Models;
using BulkyBook.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using static BulkyBook.Common.Constants;

namespace BulkyBook.Web.Areas.Admin.Controllers
{
    [Authorize(Roles =nameof(Roles.SuperAdmin))]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this._unitOfWork = unitOfWork;
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var products = this._unitOfWork.ProductRepository.GetAll(nameof(Product.Category), nameof(Product.CoverType));
            return View(products);
        }

        public IActionResult CreateEdit(int? id)
        {
            //Add
            var productViewModel = new ProductViewModel()
            {
                CategoryLists = this._unitOfWork.CategoryRepository.GetAll()
                                            .Select(e => new SelectListItem()
                                            {
                                                Text = e.Name,
                                                Value = e.Id.ToString()
                                            }),
                CoverTypeLists = this._unitOfWork.CoverTypeRepository.GetAll()
                                                .Select(e => new SelectListItem()
                                                {
                                                    Text = e.Name,
                                                    Value = e.Id.ToString()
                                                }),
                Product = new Product()
            };
            //Update
            if (id != null)
            {
                var product = _unitOfWork.ProductRepository
                    .GetByExpression(e => e.Id == id, nameof(Product.Category), nameof(Product.CoverType));
                if (product == null)
                {
                    TempData["Error"] = $"No product found with id={id}.";
                }
                else
                {
                    productViewModel.Product = product;
                }
            }
            return View(productViewModel);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = $"Invalid id {id}.";
            }
            else
            {
                var product = _unitOfWork.ProductRepository.Find(id);
                if (product == null)
                {
                    TempData["Error"] = $"Invalid id {id}.";
                }
                else
                {
                    this.DeleteProductImage(product.ImageUrl);
                    _unitOfWork.ProductRepository.Delete(product);
                    _unitOfWork.SaveChanges();
                    TempData["Success"] = $"Deleted {product.Title} successfully.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUpdate(ProductViewModel productViewModel, IFormFile? imageUrl)
        {
            var success = true;
            if (productViewModel.Product.Id == 0)
            {
                if (ModelState.IsValid)
                {
                    var existingCoverType = _unitOfWork.ProductRepository.GetByExpression(e => e.Title == productViewModel.Product.Title);
                    if (existingCoverType != null)
                    {
                        TempData["Error"] = $"Product {productViewModel.Product.Title} already exists.";
                        success = false;
                    }
                    else
                    {
                        var isValid = this.IsProductNameValid(productViewModel.Product);
                        if (isValid)
                        {
                            this.SaveImageFile(imageUrl, productViewModel);
                            //Insert the product into repo
                            this._unitOfWork.ProductRepository.Insert(productViewModel.Product);
                            this._unitOfWork.SaveChanges();
                            TempData["Success"] = $"Product {productViewModel.Product.Title} created successfully.";
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(Product.Title), "Name is Invalid");
                            success = false;
                        }
                    }
                }
                else
                    success = false;
            }
            else
            {
                var isValid = this.IsProductNameValid(productViewModel.Product);
                if (isValid)
                {
                    //If new image is provided then only remove the existing one
                    if (imageUrl != null)
                    {
                        this.DeleteProductImage(productViewModel.Product.ImageUrl);
                        this.SaveImageFile(imageUrl, productViewModel);
                    }
                    this._unitOfWork.ProductRepository.Update(productViewModel.Product);
                    this._unitOfWork.SaveChanges();
                    TempData["Success"] = $"Product {productViewModel.Product.Title} updated successfully.";
                }
                else
                {
                    ModelState.AddModelError(nameof(Product.Title), "Name is Invalid");
                    success = false;
                }
            }
            if (success)
                return RedirectToAction(nameof(Index));
            else
            {
                productViewModel.CategoryLists = this._unitOfWork.CategoryRepository.GetAll()
                            .Select(e => new SelectListItem()
                            {
                                Text = e.Name,
                                Value = e.Id.ToString()
                            });
                productViewModel.CoverTypeLists = this._unitOfWork.CoverTypeRepository.GetAll()
                                                .Select(e => new SelectListItem()
                                                {
                                                    Text = e.Name,
                                                    Value = e.Id.ToString()
                                                });
                return View(nameof(CreateEdit), productViewModel.Product.Id);
            }
        }

        private void SaveImageFile(IFormFile? imageUrl, ProductViewModel productViewModel)
        {
            //Save the file data for the product.
            if (imageUrl != null)
            {
                var wwwrootPath = this._webHostEnvironment.WebRootPath;
                var imageSubPath = Path.Combine("images", "product", $"{Guid.NewGuid()}.{imageUrl.FileName.Split(".")[1]}");
                var productImageFilePath = Path.Combine(wwwrootPath, imageSubPath);
                using (var fileStream = new FileStream(productImageFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    imageUrl.CopyTo(fileStream);
                    fileStream.Flush();
                }
                productViewModel.Product.ImageUrl = imageSubPath;
            }
        }

        private void DeleteProductImage(string imageUrl)
        {
            var contentRootPath = Path.Combine(this._webHostEnvironment.WebRootPath, imageUrl);
            System.IO.File.Delete(contentRootPath);
        }

        private bool IsProductNameValid(Product product)
        {
            var regexMatch = Regex.Match(product.Title, @"([~|!])");
            return !regexMatch.Success;
        }
    }
}
