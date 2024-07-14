using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CNPM.Models
{
    public partial class CartItem
    {
        public Product product { get; set; }
        public int quantity { get; set; }
    }
    [Table("Cart")]
    public partial class Cart
    {
        List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }
        public void AddProductCart(Product pro, int quan = 1)
        {
            var item = Items.FirstOrDefault(S => S.product.IDBook == pro.Product_title.ID);
            if (item == null)
            {
                items.Add(new CartItem
                {
                    product = pro,
                    quantity = quan
                });
            }
            else
            {
                item.quantity += quan;
            }
        }
        public int TotalQuantity()
        {
            return items.Sum(s => s.quantity);
        }
        public decimal TotalMoney()
        {
            var total = items.Sum(s => s.quantity * s.product.Product_title.giaBia);
            return (decimal)total;
        }
        public void UpdateQuantity(int id, int newQuan)
        {
            var item = items.Find(s => s.product.IDBook == id);
            if (item != null)
            {
                if (items.Find(s => s.product.quantity > newQuan) != null)
                    item.quantity = newQuan;
                else item.quantity = 1;
            }
            if (newQuan == 0)
                RemoveCartItem(id);
        }
        public void RemoveCartItem(int id)
        {
            items.RemoveAll(s => s.product.IDBook == id);
        }
        public void ClearCart()
        {
            items.Clear();
        }
    }
}