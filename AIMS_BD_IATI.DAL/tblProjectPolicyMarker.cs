//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectPolicyMarker
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int PolicyMarkerId { get; set; }
        public Nullable<int> PolicyMarkerVocabularyId { get; set; }
        public int PolicySignificanceId { get; set; }
        public string VocabularyUri { get; set; }
        public string Narrative { get; set; }
        public string NarrativeLocal { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblPolicyMarker tblPolicyMarker { get; set; }
        public virtual tblPolicyMarkerVocabulary tblPolicyMarkerVocabulary { get; set; }
        public virtual tblPolicySignificance tblPolicySignificance { get; set; }
        public virtual tblProjectInfo tblProjectInfo { get; set; }
    }
}
