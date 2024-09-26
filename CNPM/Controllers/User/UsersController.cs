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
            var check = db.Users.Where(s => s.email == user.email || s.phone == user.phone).FirstOrDefault();
            if (check == null)
            {
                if (ModelState.IsValid)
                {
                    db.Users.Add(user);
                    if (db.SaveChanges() > 0)
                    {
                        ModelState.Clear();
                    }
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.ErrorCreateUser = "Email hoặc số điện thoại đã tồn tại";
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
            var check = db.Users.Where(s => (s.email == user.email || s.phone == user.phone) && s.ID!= user.ID).FirstOrDefault();
            if (check == null)
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
            }
            else
            {
                ViewBag.ErrorEditUser = "Email hoặc số điện thoại đã tồn tại";
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
            // Find the user by ID
            Models.User user = db.Users.Find(id);

            // Check if user exists
            if (user == null)
            {
                TempData["nofi"] = "Không tìm thấy người dùng";
                return RedirectToAction("Index");
            }

            // Find all related invoice_Pro entries by the user's ID
            var invoices_Pro = db.invoice_Pro.Where(s => s.ID_cus == id).ToList();

            // If any invoice_Pro exists, delete the related invoice details and the invoices
            if (invoices_Pro.Any())
            {
                // For each invoice, remove associated invoice details
                foreach (var invoice in invoices_Pro)
                {
                    // Find related invoice_detail entries by invoiceNum
                    var invoice_Details = db.invoice_detail.Where(s => s.invoiceNum == invoice.ID).ToList();

                    // Remove all invoice details related to the current invoice
                    db.invoice_detail.RemoveRange(invoice_Details);
                }

                // Remove all invoice_Pro entries related to the user
                db.invoice_Pro.RemoveRange(invoices_Pro);
            }

            // Remove the user
            db.Users.Remove(user);

            // Save changes to the database
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
            var check = db.Users.Where(s => (s.email == user.email || s.phone == user.phone) && s.ID != user.ID).FirstOrDefault();
            if (check == null)
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
            }
            else
            {
                ViewBag.ErrorEditUser = "Email hoặc số điện thoại đã tồn tại";
            }
            return View(user);
        }
    }
}