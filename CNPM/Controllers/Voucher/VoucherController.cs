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

namespace CNPM.Controllers.Voucher
{
    public class VoucherController : Controller
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
            var courses = from c in db.Sale_promotion
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
            Sale_promotion product = new Sale_promotion();
            return View(product);
        }
        [HttpPost]
        public ActionResult Create(Sale_promotion pro)
        {
            var check = db.Sale_promotion.Where(s => s.code == pro.code && s.dateEnd > DateTime.Today).FirstOrDefault();
                if(check==null)
                {
                    if (ModelState.IsValid)
                    {
                        db.Sale_promotion.Add(pro);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }    
            else
            {
                ViewBag.ErrorVoucher = "Voucher đã tồn tại và đang được sử dụng";
            }
            return View(pro);
        }
        public ActionResult Detail(int id)
        {
            return View(db.Sale_promotion.Where(s => s.ID == id).FirstOrDefault());
        }
        public ActionResult Detail4User(int id)
        {
            return View(db.Sale_promotion.Where(s => s.ID == id).FirstOrDefault());
        }
        public ActionResult Edit(int? id)
        {
            var product = db.Sale_promotion.Where(s => s.ID == id).FirstOrDefault();
            if (product == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy đối tượng
            }

            return View(product);
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Sale_promotion sale)
        {
            var check = db.Sale_promotion.Where(s => (s.code == sale.code) && s.ID != sale.ID).FirstOrDefault();
            if (check == null)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(sale).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        TempData["nofi"] = "Cập nhật thành công";
                    }
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.ErrorEditVoucher = "Code đã tồn tại";
            }
            return View(sale);
        }

        public ActionResult PersonalVoucher(int? page)
        {
            int ID = (int)Session["IdUser"];
            var check = db.Sale_promotion
                        .Where(d => d.dateEnd > DateTime.Today && d.dateStart <= DateTime.Today)
                        .OrderBy(c => c.dateEnd);
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(check.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult detailOrder(int? id)
        {
            var check = db.invoice_Pro.SingleOrDefault(s => s.ID == id);
            Session["Sum"] = (int)check.tongTien;
            return View(db.invoice_detail.Where(s => s.invoiceNum == id).ToList());
        }
    }
}