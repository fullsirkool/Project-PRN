//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project_PRN.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;
    public partial class Evaluate
    {
        public int evaluateID { get; set; }
        public string evaluateContent { get; set; }
        public int rate { get; set; }
        public System.DateTime date { get; set; }
        public int userID { get; set; }
        public int productID { get; set; }
    
        public virtual Account Account { get; set; }
        [ScriptIgnore]
        public virtual Product Product { get; set; }
    }
}
