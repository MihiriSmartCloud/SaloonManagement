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
    
    public partial class tblbarber_service
    {
        public int barber_service_id { get; set; }
        public int barber_id { get; set; }
        public int service_id { get; set; }
    
        public virtual tblbarber tblbarber { get; set; }
        public virtual tblservice tblservice { get; set; }
    }
}