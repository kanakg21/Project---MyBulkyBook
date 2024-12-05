using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyBulkyRazorWeb.Data;
using MyBulkyRazorWeb.Model;

namespace MyBulkyRazorWeb.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Category Category { get; set; }

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if (id != null && id != 0)
            {
                Category = _db.Categories.Find(id);
            }
        }

        public IActionResult OnPost()
        {
            Category? obj= _db.Categories.Find(Category.Id);
            if (obj != null)
            {
                _db.Categories.Remove(obj);
                _db.SaveChanges();
                return RedirectToPage("Index");
            }
            return Page();  
        }
    }
}
