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
    
    public partial class tbllog
    {
        public int log_id { get; set; }
        public string ref_table { get; set; }
        public string ref_id { get; set; }
        public System.DateTime updated_date_time { get; set; }
        public string action_type { get; set; }
    }
}
