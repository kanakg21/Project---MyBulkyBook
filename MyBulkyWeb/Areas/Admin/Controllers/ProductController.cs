using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyBulky.DataAccess.Repository.IRepository;
using MyBulky.Model.Models;
using MyBulky.Model.ViewModels;
using MyBulky.Utility;

namespace MyBulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //injecting to access wwwroot folder for images
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                //projection in EF core
                CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id==0) 
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u=>u.Id==id);
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath=_webHostEnvironment.WebRootPath;
                if(file!=null)
                {
                    //it will give us a random name to image file
                    string filename=Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productpath=Path.Combine(wwwRootPath,@"images\product");

                    //check if the image is present or not
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old Image
                        //TrimStart("\\") function will remove the string present in the bracket
                        var oldImagePath = Path.Combine(wwwRootPath,productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        { 
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    //saving the file into particular location
                    using(var filestream =new FileStream(Path.Combine(productpath,filename),FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }

                    productVM.Product.ImageUrl= @"\images\product\"+filename;
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product Added Successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product Updated Successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category
                 .GetAll().Select(u => new SelectListItem
                 {
                     Text = u.Name,
                     Value = u.Id.ToString()
                 });
                return View(productVM);
            }
        }




        //we have commented edit actions bcoz from now we combine both create and edit actions

        #region comment

        //public IActionResult Create()
        //{
        //    ProductVM productVM = new()
        //    {
        //        CategoryList = _unitOfWork.Category
        //        .GetAll().Select(u => new SelectListItem
        //        {
        //            Text = u.Name,
        //            Value = u.Id.ToString()
        //        }),
        //        Product = new Product()
        //    };
        //    return View(productVM);

        //    //below is the example of Projection in dot net 
        //    //ViewBag.CategoryList = CategoryList;
        //    //ViewData["CategoryList"] = CategoryList;
        //}

        //[HttpPost]
        //public IActionResult Create(ProductVM productVM)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Add(productVM.Product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product Added Successfully";
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        productVM.CategoryList = _unitOfWork.Category
        //         .GetAll().Select(u => new SelectListItem
        //         {
        //             Text = u.Name,
        //             Value = u.Id.ToString()
        //         });
        //        return View(productVM);
        //    }
        //}

        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
        //    //Product? productFromDb=_db.Product.FirstOrDefault(u=>u.Id==id);
        //    //Product? productFromDb=_db.Product.Where(u => u.Id == id).FirstOrDefault();

        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(productFromDb);
        //}

        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product Updated Successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        #endregion comment

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product Deleted Successfully";
            return RedirectToAction("Index");
        }


        //#region API CALLS
        //[HttpGet]
        //public IActionResult GetALL()
        //{
        //    List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
        //    return Json(new {data = objProductList});
        //}

        //#endregion
    }
}
