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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }

        public IActionResult Upsert(int? id)
        {
            
            if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                Company companyobj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyobj);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company companyobj)
        {
            if (ModelState.IsValid)
            {
                if (companyobj.Id == 0)
                {
                    _unitOfWork.Company.Add(companyobj);
                    TempData["success"] = "Company Added Successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(companyobj);
                    TempData["success"] = "Company Updated Successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View(companyobj);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company? companyFromDb = _unitOfWork.Company.Get(u => u.Id == id);

            if (companyFromDb == null)
            {
                return NotFound();
            }
            return View(companyFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Company? obj = _unitOfWork.Company.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Company Deleted Successfully";
            return RedirectToAction("Index");
        }

    }
}
