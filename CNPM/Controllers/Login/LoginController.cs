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
namespace CNPM.Controllers.Login
{
    public class LoginController : Controller
    {
        BookStoreEntities db = new BookStoreEntities();
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.User user)
        {
            var check = db.Users.Where(s => s.name == user.name && s.password == user.password).FirstOrDefault();
            if (check == null)
            {
                ViewBag.ErrorInfo = "Tên đăng nhập hoặc tài khoản không đúng";
                return View("Login");

            }
            else
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                Session["NameUser"] = user.name;
                Session["UserRole"] = "User";
                Session["PasswordUser"] = user.password;
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Models.User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                if (db.SaveChanges() > 0)
                {
                    ModelState.Clear();
                }
                return RedirectToAction("Login");
            }

            return View(user);
        }
    }
}