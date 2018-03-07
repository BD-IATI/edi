//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblNGOCSO
    {
        public int Id { get; set; }
        public string NGOOrganizationName { get; set; }
        public int NGOOrganizationTypeId { get; set; }
        public string Address { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string WebURL { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonTelephone { get; set; }
        public string ContactPersonEMail { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string NameLocal { get; set; }
        public string Remarks { get; set; }
    
        public virtual tblNGOOrganizationType tblNGOOrganizationType { get; set; }
    }
}
