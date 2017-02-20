use AIMS_DB;

CREATE VIEW UserActivity AS

SELECT F.FundSourceName + ' (' +  F.Acronym + ')' AS ManagingDP, 
	T.*
	FROM (
	SELECT Id AS RecordId, 'Project Info' AS RecordInfo, Id AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectInfo
	UNION ALL
	SELECT Id AS RecordId, 'Executing Agency' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectExecutingAgency
	UNION ALL
	SELECT Id AS RecordId, 'Commitment' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectFundingCommitment
	UNION ALL
	SELECT Id AS RecordId, 'Planned Disbursement' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectFundingPlannedDisbursement
	UNION ALL
	SELECT Id AS RecordId, 'Actual Disbursement' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectFundingActualDisbursement
	UNION ALL
	SELECT Id AS RecordId, 'Expenditure' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectFundingExpenditure
	UNION ALL
	SELECT Id AS RecordId, 'Geographic Allocation' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectGeographicAllocation
	UNION ALL
	SELECT Id AS RecordId, 'Sectoral Allocation' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectSectoralAllocation
	UNION ALL
	SELECT Id AS RecordId, 'Thematic Marker' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectThematicMarker
	UNION ALL
	SELECT Id AS RecordId, 'Analytical Works MissionInfo' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectAnalyticalWorksMissionInfo
	UNION ALL
	SELECT Id AS RecordId, 'Effectiveness Indicators' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblAIDEffectivenessIndicators
	UNION ALL		
	SELECT Id AS RecordId, 'Attachments' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectAttachments
	UNION ALL
	SELECT Id AS RecordId, 'Note' AS RecordInfo, ProjectId AS ProjectId, IDate AS InsertDate, IUser AS InsertUser, EDate AS LastUpdateDate, EUser AS UpdateUser FROM tblProjectNote

) T
LEFT JOIN tblProjectInfo P ON T.ProjectId = P.Id
LEFT JOIN tblFundSource F ON P.FundSourceId = F.Id
--ORDER BY F.FundSourceName, F.Acronym, T.ProjectId