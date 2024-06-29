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

namespace CNPM.Controllers.Product
{
    public class ProductController : Controller
    {
        BookStoreEntities db = new BookStoreEntities();
        public ActionResult Index(string SortOrder, string currentFilter, string SearchString, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(SortOrder) ? "Name_desc" : "";
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }
            ViewBag.CurrentFilter = SearchString;
            var courses = from c in db.Product_title
                          select c;
            if (!string.IsNullOrEmpty(SearchString))
            {
                courses = courses.Where(c => c.name.Contains(SearchString));
            }
            switch (SortOrder)
            {
                case "Name_desc":
                    courses = courses.OrderByDescending(c => c.name);
                    break;
                default:
                    courses = courses.OrderBy(c => c.name);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(courses.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            Product_title product = new Product_title();
            return View(product);
        }
        [HttpPost]
        public ActionResult Create(Product_title pro)
        {
            try
            {
                if (pro.UploadImage != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(pro.UploadImage.FileName);
                    string extend = Path.GetExtension(pro.UploadImage.FileName);
                    fileName = fileName + extend;
                    pro.ImagePro = "~/Content/Image/" + fileName;
                    pro.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/Image/"), fileName));
                    db.Product_title.Add(pro);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult SelectCate()
        {
            Category se_cate = new Category();
            se_cate.listCate = db.Categories.ToList<Category>();
            return PartialView(se_cate);
        }
        public ActionResult SelectPublish()
        {
            CNPM.Models.Publisher se_publish = new Models.Publisher();
            se_publish.listPublisher = db.Publishers.ToList<Models.Publisher>();
            return PartialView(se_publish);
        }
        public ActionResult Detail(int id)
        {
            return View(db.Product_title.Where(s => s.ID == id).FirstOrDefault());
        }
    }
}