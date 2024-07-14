using CNPM.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CNPM.Controllers.User
{
    public class UsersController : Controller
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
            var courses = from c in db.Users
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

        // POST: AdminUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Models.User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                if (db.SaveChanges() > 0)
                {
                    TempData["nofi"] = "Thêm mới thành công";
                    ModelState.Clear();
                }
                return RedirectToAction("Index");
            }

            return View(user);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: AdminUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Models.User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    TempData["nofi"] = "Cập nhật thành công";
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: AdminUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: AdminUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Models.User user = db.Users.Find(id);
            db.Users.Remove(user);
            if (db.SaveChanges() > 0)
            {
                TempData["nofi"] = "Xóa thành công";
            }
            return RedirectToAction("Index");
        }

        public ActionResult PersonalInfo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        public ActionResult EditPersonal(int ?id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult EditPersonal(Models.User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                {
                    TempData["nofi"] = "Cập nhật thành công";
                }
                return RedirectToAction("PersonalInfo", new { id = user.ID });
            }
            return View(user);
        }
    }
}