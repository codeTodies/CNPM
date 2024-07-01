using Antlr.Runtime.Tree;
using CNPM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Drawing;

namespace CNPM.Controllers.Categories
{
    public class CategoriesController : Controller
    {
        BookStoreEntities1  db = new BookStoreEntities1();
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
            var courses = from c in db.Categories
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
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category category)
        {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");

        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(category);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tìm bản ghi Category cần xóa
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra xem có bất kỳ bản ghi nào trong bảng Product trỏ đến Category này hay không
            bool hasReferences = db.Product_title.Any(p => p.category == id);

            if (hasReferences)
            {
                // Nếu có tham chiếu, bạn có thể thực hiện xử lý tùy ý, ví dụ: hiển thị thông báo lỗi
                ViewBag.ErrorCate = "Không thể xóa Category này vì có dữ liệu tham chiếu từ bảng Product.";
                return View(category);
            }

            // Nếu không có tham chiếu, thực hiện xóa
            db.Entry(category).State = EntityState.Deleted;
            if (db.SaveChanges() > 0)
            {
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}