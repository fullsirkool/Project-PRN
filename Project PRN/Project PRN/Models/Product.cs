//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project_PRN.Models {
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;
    using System.Configuration;

    public partial class Product {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product() {
            this.Evaluates = new HashSet<Evaluate>();
            this.Bills = new HashSet<Bill>();
            this.Carts = new HashSet<Cart>();
        }

        public int productID { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string shortDescription { get; set; }
        public string image { get; set; }
        public int rate { get; set; }
        public int calculateRate()
        {
            int total = 0;
            foreach (Evaluate e in Evaluates)
            {
                total += e.rate;
            }
            if (Evaluates.Count > 0)
            {
                return total / Evaluates.Count;
            }
            else
            {
                return 0;
            }
        }

        public string fullImagePath() {
            string path = ConfigurationManager.ConnectionStrings["imagePath"].ToString();
            return $"{path}{image}";
        }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public int sold { get; set; }
        public System.DateTime postTime { get; set; }
        public int categoriesID { get; set; }
        public int userID { get; set; }

        public virtual Account Account { get; set; }
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Evaluate> Evaluates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [ScriptIgnore]
        public virtual ICollection<Bill> Bills { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [ScriptIgnore]
        public virtual ICollection<Cart> Carts { get; set; }
    }
}
