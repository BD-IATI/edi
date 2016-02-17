//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectAttachment
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int DocumentCategoryId { get; set; }
        public Nullable<int> AnalyticalWorkMissionId { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public string AttachmentNo { get; set; }
        public string AttachmentTitle { get; set; }
        public string AttachmentFileName { get; set; }
        public string AttachmentMetaData { get; set; }
        public string AttachmentFilePath { get; set; }
        public string AttachmentFileURL { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblDocumentCategory tblDocumentCategory { get; set; }
        public virtual tblDocumentType tblDocumentType { get; set; }
        public virtual tblProjectAnalyticalWorksMissionInfo tblProjectAnalyticalWorksMissionInfo { get; set; }
        public virtual tblProjectInfo tblProjectInfo { get; set; }
    }
}
