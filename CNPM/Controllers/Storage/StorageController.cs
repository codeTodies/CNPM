using CNPM.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CNPM.Controllers.Storage
{
    public class StorageController : Controller
    {
        BookStoreEntities db = new BookStoreEntities();
        // GET: Users
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
            var courses = from c in db.Product
                          select c;
            if (!string.IsNullOrEmpty(SearchString))
            {
                courses = courses.Where(c => c.Product_title.name.Contains(SearchString));
            }
            switch (SortOrder)
            {
                case "Name_desc":
                    courses = courses.OrderByDescending(c => c.Product_title.name);
                    break;
                default:
                    courses = courses.OrderBy(c => c.Product_title.name);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(courses.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult Create(int id)
        {
            return View(db.Product.Where(s => s.IDBook == id).FirstOrDefault());
        }

        // POST: AdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(int id, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                // Lấy đối tượng từ cơ sở dữ liệu
                var pro = db.Product.Find(id);

                if (pro != null)
                {
                    // Cập nhật thuộc tính quantity
                    pro.quantity += int.Parse(form["Quantity"]);

                    // Đánh dấu đối tượng là đã được sửa đổi
                    db.Entry(pro).Property(p => p.quantity).IsModified = true;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

    }
}