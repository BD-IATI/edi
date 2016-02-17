//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectAnalyticalWorksMissionInfo
    {
        public tblProjectAnalyticalWorksMissionInfo()
        {
            this.tblProjectAttachments = new HashSet<tblProjectAttachment>();
        }
    
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string AWMIType { get; set; }
        public string AWMITitle { get; set; }
        public Nullable<bool> AWMIIsJoint { get; set; }
        public string AWMIOtherDPFundSourceId { get; set; }
        public Nullable<int> AWMIGoBLeadMinistryAgencyId { get; set; }
        public Nullable<System.DateTime> AWMIDurationStartDate { get; set; }
        public Nullable<System.DateTime> AWMIDurationCompletionDate { get; set; }
        public string AWMIAuthorDesc { get; set; }
        public string AWMIStudyLocation { get; set; }
        public string AWMIFocalPointContactName { get; set; }
        public string AWMIFocalPointContactTitle { get; set; }
        public string AWMIFocalPointContactDesignation { get; set; }
        public string AWMIFocalPointContactTelephone { get; set; }
        public string AWMIFocalPointContactFax { get; set; }
        public string AWMIFocalPointContactEmail { get; set; }
        public string AWMIFocalPointContactAddress { get; set; }
        public string AWMIRemarks { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
        public string AWMIOtherDPFundSourceName { get; set; }
    
        public virtual tblMinistryAgency tblMinistryAgency { get; set; }
        public virtual tblProjectInfo tblProjectInfo { get; set; }
        public virtual ICollection<tblProjectAttachment> tblProjectAttachments { get; set; }
    }
}
