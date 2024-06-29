using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Web;

namespace CNPM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Publisher
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Publisher()
        {
            this.Product_title = new HashSet<Product_title>();
        }
        [Key]
        public int ID { get; set; }
        public string name { get; set; }
        [NotMapped]
        public List<Publisher> listPublisher { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product_title> Product_title { get; set; }
    }
}
