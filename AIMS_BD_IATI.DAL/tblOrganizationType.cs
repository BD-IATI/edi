//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblOrganizationType
    {
        public tblOrganizationType()
        {
            this.tblUserRegistrationInfoes = new HashSet<tblUserRegistrationInfo>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<tblUserRegistrationInfo> tblUserRegistrationInfoes { get; set; }
    }
}
