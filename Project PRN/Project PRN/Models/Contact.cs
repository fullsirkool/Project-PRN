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
    
    public partial class Contact
    {
        public int userid { get; set; }
        public string email { get; set; }
        public string content { get; set; }
        public System.DateTime date { get; set; }
        public int contactid { get; set; }
        public string subject { get; set; }
    
        public virtual Account Account { get; set; }
    }
}
