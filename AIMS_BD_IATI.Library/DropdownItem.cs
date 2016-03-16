﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS_BD_IATI.Library
{
    public class DPLookupItem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int AimsFundSourceId { get; set; }

    }
    public class FundSourceLookupItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string IATICode { get; set; }
        public string IDnIATICode { get { return ID + "~" + IATICode??""; } }

    }

    public class LookupItem
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }
}
