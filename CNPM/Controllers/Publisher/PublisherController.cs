using CNPM.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CNPM.Controllers.Publisher
{
    public class PublisherController : Controller
    {

        BookStoreEntities1 db = new BookStoreEntities1();
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
            var courses = from c in db.Publishers
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
            CNPM.Models.Publisher publisher = new Models.Publisher();
            return View(publisher);
        }
        [HttpPost]
        public ActionResult Create(CNPM.Models.Publisher publisher)
        {
            try
            {
                        db.Publishers.Add(publisher);
                        db.SaveChanges();
                        return RedirectToAction("Index","Publisher");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Publisher publisher = db.Publishers.Find(id);
            if (publisher == null)
            {
                return HttpNotFound();
            }
            return View(publisher);
        }
        [HttpPost]
        public ActionResult Edit(Models.Publisher publisher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(publisher).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(publisher);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Publisher publisher = db.Publishers.Find(id);
            if (publisher == null)
            {
                return HttpNotFound();
            }
            return View(publisher);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tìm bản ghi Category cần xóa
            var publisher = db.Publishers.Find(id);
            if (publisher == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra xem có bất kỳ bản ghi nào trong bảng Product trỏ đến Category này hay không
            bool hasReferences = db.Product_title.Any(p => p.publisher == id);

            if (hasReferences)
            {
                // Nếu có tham chiếu, bạn có thể thực hiện xử lý tùy ý, ví dụ: hiển thị thông báo lỗi
                ViewBag.ErrorPublish = "Không thể xóa nhà xuất bản này vì có dữ liệu tham chiếu từ bảng Product.";
                return View(publisher);
            }

            // Nếu không có tham chiếu, thực hiện xóa
            db.Entry(publisher).State = EntityState.Deleted;
            if (db.SaveChanges() > 0)
            {
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}