//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class CommonConfigType
    {
        public string TableName { get; set; }
        public string DisplayName { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public int ModuleId { get; set; }
    }
}
