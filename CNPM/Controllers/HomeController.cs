using CNPM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CNPM.Controllers
{
    public class HomeController : Controller
    {
        BookStoreEntities1 database = new BookStoreEntities1();
        public ActionResult Index()
        {
            return View(database.Product_title.ToList());
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
    }

}