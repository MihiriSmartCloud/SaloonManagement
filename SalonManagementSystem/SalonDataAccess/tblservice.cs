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
    
    public partial class tblservice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblservice()
        {
            this.tblservice_booked = new HashSet<tblservice_booked>();
            this.tblbarber_service = new HashSet<tblbarber_service>();
        }
    
        public int service_id { get; set; }
        public string service_name { get; set; }
        public int salon_id { get; set; }
        public decimal price { get; set; }
        public int duration { get; set; }
    
        public virtual tblsalon tblsalon { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblservice_booked> tblservice_booked { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblbarber_service> tblbarber_service { get; set; }
    }
}
