using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web;

namespace CNPM.Models
{
    public class CompositeModel : DbContext
    {
        public DbSet<Category> categories { get; set; }
        public DbSet<invoice_detail> invoice_Details { get; set; }
        public DbSet<invoice_Pro> invoice_Pros { get; set; }
        public DbSet<Product_title> product_Titles { get; set; }
        public DbSet<Publisher> publishers { get; set; }
        public DbSet<Sale_promotion> sale_Promotions { get; set; }
        public DbSet<Staff> staffs { get; set; }
        public DbSet<User> users { get; set; }
    }
}