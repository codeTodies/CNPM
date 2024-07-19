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
            ViewBag.Cate = new SelectList(db.Categories.OrderBy(r => r.name), "ID", "name", product.category);
            ViewBag.Publish = new SelectList(db.Publishers.OrderBy(r => r.name), "ID", "name", product.publisher);
            return View(product);
        }
        [HttpPost]
        public ActionResult Create(Product_title pro)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    if (pro.UploadImage != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(pro.UploadImage.FileName);
                        string extend = Path.GetExtension(pro.UploadImage.FileName);
                        fileName = fileName + extend;
                        pro.ImagePro = "~/Content/Image/" + fileName;
                        pro.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/Image/"), fileName));
                        db.Product_title.Add(pro);
                        CNPM.Models.Product product = new CNPM.Models.Product();
                        product.IDBook = pro.ID;
                        product.quantity = 0;
                        db.Product.Add(product);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }    
                return View(pro);
            }
            catch
            {
                return View(pro);
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
        public ActionResult Edit(int? id)
        {
            var product = db.Product_title.Where(s => s.ID == id).FirstOrDefault();
            ViewBag.Cate = new SelectList(db.Categories.OrderBy(r => r.name), "ID", "name", product.category);
            ViewBag.Publish = new SelectList(db.Publishers.OrderBy(r => r.name), "ID", "name", product.publisher);
            Session["imgPath"] = product.ImagePro;
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
        public ActionResult Edit(Product_title product)
        {
            if (ModelState.IsValid)
            {

                if (product.UploadImage != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(product.UploadImage.FileName);
                    string extend = Path.GetExtension(product.UploadImage.FileName);
                    fileName = fileName + extend;
                    product.ImagePro = "~/image/" + fileName;
                    if (extend.ToLower() == ".jpg" || extend.ToLower() == ".jpeg" || extend.ToLower() == ".png")
                    {
                        if (product.UploadImage.ContentLength <= 6000000)
                        {
                            db.Entry(product).State = EntityState.Modified;

                            string oldImgPath = Request.MapPath(Session["imgPath"].ToString());
                            if (db.SaveChanges() > 0)
                            {
                                product.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/image/"), fileName));
                                if (System.IO.File.Exists(oldImgPath))
                                {
                                    System.IO.File.Delete(oldImgPath);
                                }
                                TempData["nofi"] = "Cập nhật thành công";
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ViewBag.nofi = "File Size must be Equal or less than 6mb";
                        }
                    }
                    else
                    {
                        ViewBag.nofi = "Invalid File Type";
                    }
                }
                else
                {
                    product.ImagePro = Session["imgPath"].ToString();
                    db.Entry(product).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        TempData["nofi"] = "Cập nhật thành công";
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product_title product = db.Product_title.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Regions/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var pro = db.Product.Where(s => s.IDBook == id).FirstOrDefault();
            var product = db.Product_title.Where(s => s.ID == id).FirstOrDefault();
            if (product == null || pro == null)
            {
                return HttpNotFound();
            }
            string curImg = Request.MapPath(product.ImagePro);
            db.Entry(pro).State = EntityState.Deleted;
            db.Entry(product).State = EntityState.Deleted;
            if (db.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(curImg))
                {
                    System.IO.File.Delete(curImg);
                }
                TempData["nofi"] = "Xóa thành công";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}