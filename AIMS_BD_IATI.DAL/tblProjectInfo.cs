//------------------------------------------------------------------------------
// 
//------------------------------------------------------------------------------

namespace AIMS_BD_IATI.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectInfo
    {
        public tblProjectInfo()
        {
            this.tblAIDEffectivenessIndicators = new HashSet<tblAIDEffectivenessIndicator>();
            this.tblProjectAnalyticalWorksMissionInfoes = new HashSet<tblProjectAnalyticalWorksMissionInfo>();
            this.tblProjectAttachments = new HashSet<tblProjectAttachment>();
            this.tblProjectExecutingAgencies = new HashSet<tblProjectExecutingAgency>();
            this.tblProjectFundingActualDisbursements = new HashSet<tblProjectFundingActualDisbursement>();
            this.tblProjectFundingCommitments = new HashSet<tblProjectFundingCommitment>();
            this.tblProjectFundingExpenditures = new HashSet<tblProjectFundingExpenditure>();
            this.tblProjectFundingPlannedDisbursements = new HashSet<tblProjectFundingPlannedDisbursement>();
            this.tblProjectGeographicAllocations = new HashSet<tblProjectGeographicAllocation>();
            this.tblProjectGoBExecutingAgencies = new HashSet<tblProjectGoBExecutingAgency>();
            this.tblProjectNotes = new HashSet<tblProjectNote>();
            this.tblProjectSectoralAllocations = new HashSet<tblProjectSectoralAllocation>();
            this.tblProjectThematicMarkers = new HashSet<tblProjectThematicMarker>();
            this.tblUserProjects = new HashSet<tblUserProject>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string Objective { get; set; }
        public int FundSourceId { get; set; }
        public Nullable<bool> IsProgramBasedApproach { get; set; }
        public Nullable<int> SectorId { get; set; }
        public Nullable<bool> IsUnderADP { get; set; }
        public Nullable<bool> IsCountryWide { get; set; }
        public Nullable<int> ApprovalStatusId { get; set; }
        public Nullable<int> ApprovalAuthorityId { get; set; }
        public string DPProjectNo { get; set; }
        public string MoFProjectCodePart1 { get; set; }
        public string MoFProjectCodePart2 { get; set; }
        public string ADPProjectCode { get; set; }
        public string ADPProjectName { get; set; }
        public string IatiIdentifier { get; set; }
        public int AssistanceTypeId { get; set; }
        public int ProjectTypeId { get; set; }
        public Nullable<bool> IsCofundedProject { get; set; }
        public Nullable<bool> IsPoolFund { get; set; }
        public Nullable<int> AgreementTypeId { get; set; }
        public Nullable<int> AidAgreementId { get; set; }
        public string AidAgreementTitle { get; set; }
        public Nullable<decimal> DPAssistanceInUSD { get; set; }
        public Nullable<decimal> DPAssistance { get; set; }
        public Nullable<int> DPAssistanceCurrencyId { get; set; }
        public Nullable<decimal> DPAssistanceExchangeRateToUSD { get; set; }
        public Nullable<decimal> DPAssistanceUSD { get; set; }
        public Nullable<decimal> GoBSharingBDT { get; set; }
        public Nullable<decimal> GoBSharingExchangeRateToUSD { get; set; }
        public string OtherContributionBy { get; set; }
        public Nullable<decimal> OtherContribution { get; set; }
        public Nullable<int> OtherContributionCurrencyId { get; set; }
        public Nullable<decimal> OtherContributionExchangeRateToUSD { get; set; }
        public Nullable<decimal> OtherContributionInUSD { get; set; }
        public Nullable<System.DateTime> DateOfExchangeRate { get; set; }
        public Nullable<decimal> ProjectCostUSD { get; set; }
        public System.DateTime AgreementSignDate { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> ProjectApprovalDate { get; set; }
        public Nullable<System.DateTime> PlannedProjectStartDate { get; set; }
        public Nullable<System.DateTime> PlannedProjectCompletionDate { get; set; }
        public Nullable<System.DateTime> ActualProjectStartDate { get; set; }
        public Nullable<System.DateTime> RevisedProjectCompletionDate { get; set; }
        public Nullable<int> ImplementationStatusId { get; set; }
        public Nullable<int> RevisionStatusId { get; set; }
        public string ExecutingAgencyType { get; set; }
        public Nullable<int> ExecutingAgencyId { get; set; }
        public string FocalPointDPContactName { get; set; }
        public string FocalPointDPContactTitle { get; set; }
        public string FocalPointDPContactDesignation { get; set; }
        public string FocalPointDPContactTelephone { get; set; }
        public string FocalPointDPContactFax { get; set; }
        public string FocalPointDPContactEmail { get; set; }
        public string FocalPointDPContactAddress { get; set; }
        public string FocalPointGoBContactName { get; set; }
        public string FocalPointGoBContactTitle { get; set; }
        public string FocalPointGoBContactDesignation { get; set; }
        public string FocalPointGoBContactTelephone { get; set; }
        public string FocalPointGoBContactFax { get; set; }
        public string FocalPointGoBContactEmail { get; set; }
        public string FocalPointGoBContactAddress { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public Nullable<System.DateTime> EDate { get; set; }
    
        public virtual tblAgreementType tblAgreementType { get; set; }
        public virtual ICollection<tblAIDEffectivenessIndicator> tblAIDEffectivenessIndicators { get; set; }
        public virtual tblApprovalAuthority tblApprovalAuthority { get; set; }
        public virtual tblApprovalStatu tblApprovalStatu { get; set; }
        public virtual tblAssistanceType tblAssistanceType { get; set; }
        public virtual tblCurrency tblCurrency { get; set; }
        public virtual tblCurrency tblCurrency1 { get; set; }
        public virtual tblFundSource tblFundSource { get; set; }
        public virtual tblImplementationStatu tblImplementationStatu { get; set; }
        public virtual ICollection<tblProjectAnalyticalWorksMissionInfo> tblProjectAnalyticalWorksMissionInfoes { get; set; }
        public virtual ICollection<tblProjectAttachment> tblProjectAttachments { get; set; }
        public virtual ICollection<tblProjectExecutingAgency> tblProjectExecutingAgencies { get; set; }
        public virtual ICollection<tblProjectFundingActualDisbursement> tblProjectFundingActualDisbursements { get; set; }
        public virtual ICollection<tblProjectFundingCommitment> tblProjectFundingCommitments { get; set; }
        public virtual ICollection<tblProjectFundingExpenditure> tblProjectFundingExpenditures { get; set; }
        public virtual ICollection<tblProjectFundingPlannedDisbursement> tblProjectFundingPlannedDisbursements { get; set; }
        public virtual ICollection<tblProjectGeographicAllocation> tblProjectGeographicAllocations { get; set; }
        public virtual ICollection<tblProjectGoBExecutingAgency> tblProjectGoBExecutingAgencies { get; set; }
        public virtual tblProjectType tblProjectType { get; set; }
        public virtual tblRevisionStatu tblRevisionStatu { get; set; }
        public virtual ICollection<tblProjectNote> tblProjectNotes { get; set; }
        public virtual ICollection<tblProjectSectoralAllocation> tblProjectSectoralAllocations { get; set; }
        public virtual ICollection<tblProjectThematicMarker> tblProjectThematicMarkers { get; set; }
        public virtual ICollection<tblUserProject> tblUserProjects { get; set; }
    }
}
