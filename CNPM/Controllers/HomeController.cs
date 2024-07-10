using CNPM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;
using System.Web.UI.WebControls;


namespace CNPM.Controllers
{
    public class HomeController : Controller
    {
        BookStoreEntities database = new BookStoreEntities();
        public ActionResult Index(string SortOrder, string currentFilter, string SearchString, int? page, int? category)
        {
            ViewBag.PriceSortParam = String.IsNullOrEmpty(SortOrder) ? "price_desc" : SortOrder;
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }
            ViewBag.CurrentFilter = SearchString;
            var courses = from c in database.Product_title
                          select c;
            if (!string.IsNullOrEmpty(SearchString))
            {
                courses = courses.Where(c => c.name.Contains(SearchString));
            }
            switch (SortOrder)
            {
                case "price_desc":
                    courses = courses.OrderByDescending(c => c.giaBia);
                    break;
                case "price_asc":
                    courses = courses.OrderBy(c => c.giaBia);
                    break;
                default:
                    // Mặc định sắp xếp theo giảm dần
                    courses = courses.OrderByDescending(c => c.giaBia);
                    break;
            } 
            if(category.HasValue)
            {
                courses = courses.Where(c => c.category == category);
            }    
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(courses.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Signup()
        {

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public PartialViewResult CategoryPartial()
        {
            var cateList = database.Categories.ToList();
            return PartialView(cateList);
        }
    }

}