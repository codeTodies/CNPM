using CNPM.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CNPM.Controllers.ShoppingCart
{
    public class ShoppingCartController : Controller
    {
        BookStoreEntities database = new BookStoreEntities();
        // GET: ShoppingCart
        public ActionResult ShowCart()
        {
            if (Session["Cart"] == null)
            {
                return View("EmptyCart");
            }
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
        public ActionResult AddToCart(int id)
        {
            if (Session["NameUser"] == null)
            {
                return RedirectToAction("Login", "Login");
            }    
            else
            {
                var pro = database.Product.SingleOrDefault(s => s.IDBook == id);
                if (pro != null)
                {
                    GetCart().AddProductCart(pro);
                }
                return RedirectToAction("ShowCart", "ShoppingCart");
            }    
        }
        public ActionResult AddToCarts(int id)
        {
            if (Session["NameUser"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
           else
            {
                var pro = database.Product.SingleOrDefault(s => s.IDBook == id);
                if (pro != null)
                {
                    GetCart().AddProductCart(pro);
                }
                return RedirectToAction("Detail", "Product", new { id = id });
            }    
        }
        public ActionResult UpdateCartQuantity(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            int id_pro = int.Parse(form["idPro"]);
            int quantity = int.Parse(form["cartQuantity"]);
            cart.UpdateQuantity(id_pro, quantity);
            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.RemoveCartItem(id);
            return RedirectToAction("ShowCart", "ShoppingCart");
        }
        public PartialViewResult BagCart()
        {
            int total_quantity_item = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
            {
                total_quantity_item = cart.TotalQuantity();
            }
            ViewBag.QuantityCart = total_quantity_item;
            return PartialView("BagCart");
        }
        public ActionResult ThanhToan()
        {
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }
        public ActionResult InfoUser()
        {
            if (Session["IdUser"] != null )
            {
                int ID = (int)Session["IdUser"];
                // Use userId as needed
                var user = database.Users.Where(s => s.ID == ID ).FirstOrDefault();
                return PartialView(user);
            }
            else
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy đối tượng
            }
        }
        public ActionResult CheckOut(FormCollection form)
        {
            try
            {
                int ID = (int)Session["IdUser"];
                Cart cart = Session["Cart"] as Cart;
                invoice_Pro order = new invoice_Pro();
                order.dateOrder = DateTime.Now;
                order.addressDeli = form["Address"];
                order.ID_cus = ID;
                database.invoice_Pro.Add(order);
                foreach (var item in cart.Items)
                {
                    invoice_detail _order_detail = new invoice_detail();
                    _order_detail.invoiceNum = order.ID;
                    _order_detail.ID_product = item.product.IDBook;
                    _order_detail.price = (int)item.product.Product_title.giaBia;
                    _order_detail.quantity = item.quantity;
                    database.invoice_detail.Add(_order_detail);
                    foreach (var p in database.Product.Where(s => s.IDBook == _order_detail.ID_product))
                    {
                        var update_quan_pro = p.quantity - item.quantity;
                        p.quantity = update_quan_pro;
                    }
                }
                database.SaveChanges();
                cart.ClearCart();
                return RedirectToAction("CheckOutSuccess", "ShoppingCart");
            }
            catch
            {
                return Content("Error, please check info again!");
            }
        }
        public ActionResult CheckOutSuccess()
        {
            return View();
        }
        public ActionResult PersonalCart(int? page)
        {
            int ID = (int)Session["IdUser"];
            var check = from d in database.invoice_detail
                        where d.invoice_Pro.ID_cus == ID
                        select d;
            check = check.OrderBy(c => c.invoice_Pro.dateOrder);
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(check.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult detailOrder(int?id)
        {
            return View(database.invoice_detail.Where(s => s.ID == id).ToList());
        }
    }
}