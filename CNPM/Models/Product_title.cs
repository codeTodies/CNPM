//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CNPM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;
    using System.Linq;
    using System.Web;

    public partial class Product_title
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product_title()
        {
            this.invoice_detail = new HashSet<invoice_detail>();
            this.ImagePro = "~/Content/Image/bbtmh.jpg";
        }
    
        public int ID { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string name { get; set; }

        [Required(ErrorMessage = "Publishing year is required.")]
        public int? publishing_year { get; set; }

        [Required(ErrorMessage = "Publisher is required.")]
        public int? publisher { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int? category { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        public string author { get; set; }

        [Required(ErrorMessage = "Price (giaBia) is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price (giaBia) must be a non-negative number.")]
        public double? giaBia { get; set; }
        public string ImagePro { get; set; }

        [NotMapped]
        public HttpPostedFileBase UploadImage { get; set; }
        public virtual Category Category1 { get; set; }
        public List<Product_title> ListProT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<invoice_detail> invoice_detail { get; set; }
        public virtual Product Product { get; set; }
        public virtual Publisher Publisher1 { get; set; }
    }
}
