//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectADPInfo
    {
        public int Id { get; set; }
        public string ADPInfoProjectCode { get; set; }
        public string ADPInfoProjectName { get; set; }
        public string ADPInfoMoFProjectCodePart1 { get; set; }
        public string ADPInfoMoFProjectCodePart2 { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    }
}
