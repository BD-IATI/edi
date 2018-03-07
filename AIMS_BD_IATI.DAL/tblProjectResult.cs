//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectResult
    {
        public tblProjectResult()
        {
            this.tblProjectResultDetails = new HashSet<tblProjectResultDetail>();
        }
    
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ResultIndicatorTypeId { get; set; }
        public string IndicatorTitle { get; set; }
        public string IndicatorTitleLocal { get; set; }
        public Nullable<int> BaselineYear { get; set; }
        public string BaselineValue { get; set; }
        public string Description { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblProjectInfo tblProjectInfo { get; set; }
        public virtual tblResultIndicatorType tblResultIndicatorType { get; set; }
        public virtual ICollection<tblProjectResultDetail> tblProjectResultDetails { get; set; }
    }
}
