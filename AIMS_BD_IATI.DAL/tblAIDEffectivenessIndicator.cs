//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblAIDEffectivenessIndicator
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Nullable<int> AEISurveyYear { get; set; }
        public Nullable<bool> AEINBEPIsAnnualBudgetByCountryLegislature { get; set; }
        public Nullable<bool> AEINBEPIsCountryBudgetExecutionProcedure { get; set; }
        public Nullable<bool> AEINBEPIsCountryTreasurySystem { get; set; }
        public Nullable<bool> AEINBEPIsNotRequiredBankAccount { get; set; }
        public Nullable<bool> AEINFRPIsNotSeparateAccSystem { get; set; }
        public Nullable<bool> AEINFRPIsFinancialRptUsingCountrySystem { get; set; }
        public Nullable<bool> AEINAPIsFundsAuditted { get; set; }
        public Nullable<bool> AEINAPIsNotReqAddAudit { get; set; }
        public Nullable<bool> AEINAPIsNotReqAuditStandard { get; set; }
        public Nullable<bool> AEINAPIsNotReqAuditCycleChange { get; set; }
        public Nullable<bool> AEINPSIsUseGovtProcSystem { get; set; }
        public Nullable<bool> AEIPIUIsUsePIU { get; set; }
        public Nullable<bool> AEIPIUIsPIUsAccToDonors { get; set; }
        public Nullable<bool> AEIPIUIsPIUsAppointExtStaffByDonor { get; set; }
        public Nullable<bool> AEIPIUIsPIUsAppointProfStaffByDonor { get; set; }
        public Nullable<bool> AEIPIUIsPIUsSalaryStrucHigher { get; set; }
        public Nullable<int> AEIResourceTiedTypeId { get; set; }
        public string AEIRemarks { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblAIDEffectivenessResourceTiedType tblAIDEffectivenessResourceTiedType { get; set; }
        public virtual tblProjectInfo tblProjectInfo { get; set; }
    }
}
