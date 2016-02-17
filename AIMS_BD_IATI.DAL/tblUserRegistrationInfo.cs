//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblUserRegistrationInfo
    {
        public tblUserRegistrationInfo()
        {
            this.tblUserFundSources = new HashSet<tblUserFundSource>();
            this.tblUserProjects = new HashSet<tblUserProject>();
        }
    
        public int Id { get; set; }
        public string UserFullName { get; set; }
        public int OrganizationTypeId { get; set; }
        public Nullable<int> OrgTypeFundSourceMinistryId { get; set; }
        public string OrganizationName { get; set; }
        public string EMail { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string UserId { get; set; }
        public string UserPassword { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string Status { get; set; }
        public Nullable<int> ProjectPermissionType { get; set; }
        public string PositionName { get; set; }
        public Nullable<int> IsUnderADP { get; set; }
        public Nullable<bool> IsManagingDP { get; set; }
    
        public virtual tblOrganizationType tblOrganizationType { get; set; }
        public virtual ICollection<tblUserFundSource> tblUserFundSources { get; set; }
        public virtual ICollection<tblUserProject> tblUserProjects { get; set; }
    }
}
