//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SalonDataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblsalon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblsalon()
        {
            this.tblbarbers = new HashSet<tblbarber>();
            this.tblcurrent_appointments = new HashSet<tblcurrent_appointments>();
            this.tblcustomers = new HashSet<tblcustomer>();
            this.tblservices = new HashSet<tblservice>();
            this.tblappointments = new HashSet<tblappointment>();
            this.tblinvoices = new HashSet<tblinvoice>();
        }
    
        public int salon_id { get; set; }
        public int owner_id { get; set; }
        public string salon_name { get; set; }
        public string salon_location { get; set; }
        public int? contact_no { get; set; }
        public string email { get; set; }
        public int seating_capacity { get; set; }
        public System.TimeSpan opening_time { get; set; }
        public System.TimeSpan closing_time { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblbarber> tblbarbers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblcurrent_appointments> tblcurrent_appointments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblcustomer> tblcustomers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblservice> tblservices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblappointment> tblappointments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblinvoice> tblinvoices { get; set; }
        public virtual tblshop_owner tblshop_owner { get; set; }
    }
}
