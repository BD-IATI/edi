
SELECT  P.Id, REPLACE(REPLACE(REPLACE(P.Title, ',', ';'), CHAR(13), ' '), CHAR(10), ' ') AS Title, 
	REPLACE(REPLACE(REPLACE(P.Objective, ',', ';'), CHAR(13), ' '), CHAR(10), ' ') AS Objective, 
	CASE WHEN P.Title = P.Objective  THEN 1 ELSE 0 END AS IsTitleEqualsDesc,
	F.FundSourceName + ' (' + F.Acronym + ')' AS ManagingDP,
	P.IsUnderADP, P.DPAssistance, P.DPAssistanceCurrencyId, P.DPAssistanceUSD, P.FundSourceId, 
	ISNULL(EA.NoOfExeAgency, 0) AS NoOfExeAgency, ISNULL(EA.NoOfExeAgencyGoB,0) AS NoOfExeAgencyGoB, C.NoOfDP, 

	[dbo].[fnGetProjectLastUpdateDate](P.Id, Null,  null) AS ProjectLastUpdateDate,
	COALESCE(P.RevisedProjectCompletionDate, P.PlannedProjectCompletionDate) AS ProjectCompletionDate,
	DATEDIFF(YEAR, GETDATE(), COALESCE(P.RevisedProjectCompletionDate, P.PlannedProjectCompletionDate)) AS NoOfYearLeft,
	
	PD.MaxPlannedDisbursementPeriodToDate,
	DATEDIFF(YEAR, GETDATE(), PD.MaxPlannedDisbursementPeriodToDate) AS NoOfFutureYearPlannedDisbursement,

	ISNULL(C1.NoOfCommitment,0) AS NoOfCommitment,
	ISNULL(C.TotalCommittedUSD,0) AS TotalCommittedUSD, 
	
	ISNULL(P1.NoOfPlannedDisbursements,0) AS NoOfPlannedDisbursements, ISNULL(P3.AVGPlanDisDATEDIFF,0) AS AVGPlanDisDATEDIFF,
	ISNULL(PD.TotalPlanDisb,0) AS TotalPlanDisb, 
	ISNULL(PD.TotalPlanDisbUSD,0) AS TotalPlanDisbUSD, 
	
	ISNULL(D1.NoOfDisbursement,0) AS NoOfDisbursement, ISNULL(D3.AVGDisDATEDIFF,0) AS AVGDisDATEDIFF,
	ISNULL(D2.TotalActualDisb,0) AS TotalActualDisb, 
	ISNULL(D2.TotalActualDisbUSD,0) AS TotalActualDisbUSD,  
	
	ISNULL(S.NoOfSector,0) AS NoOfSector, 
	ISNULL(G.NoOfLocations,0) AS NoOfLocations,
	ISNULL(A.NoOfAttachments,0) AS NoOfAttachments
FROM  tblProjectInfo AS P 
LEFT JOIN tblFundSource F ON P.FundSourceId = F.Id
LEFT OUTER JOIN
(SELECT A.ProjectId, COUNT(*) AS NoOfExeAgency, COUNT(CASE WHEN A.ExecutingAgencyTypeId = 1 THEN 1 END) AS NoOfExeAgencyGoB
	FROM   tblProjectExecutingAgency AS A  GROUP BY A.ProjectId) AS EA ON P.Id = EA.ProjectId
--PlannedDisbursements
LEFT OUTER JOIN
(SELECT  C1.ProjectId, COUNT(C1.Id) AS NoOfCommitment
	FROM tblProjectFundingCommitment AS C1 GROUP BY C1.ProjectId) AS C1 ON P.Id = C1.ProjectId
LEFT OUTER JOIN
(SELECT  C.ProjectId, COUNT(DISTINCT C.FundSourceId) AS NoOfDP, SUM(C.CommittedAmountInUSD) As TotalCommittedUSD
	FROM tblProjectFundingCommitment AS C GROUP BY C.ProjectId) AS C ON P.Id = C.ProjectId
--PlannedDisbursements
LEFT OUTER JOIN
(SELECT  P1.ProjectId, COUNT(P1.Id) AS NoOfPlannedDisbursements
	FROM tblProjectFundingPlannedDisbursement AS P1 GROUP BY P1.ProjectId) AS P1 ON P.Id = P1.ProjectId
LEFT OUTER JOIN
(SELECT P.ProjectId, SUM(P.PlannedDisburseAmount) AS TotalPlanDisb, SUM(P.PlannedDisburseAmountInUSD) AS TotalPlanDisbUSD,
	MAX(COALESCE(P.PlannedDisbursementPeriodToDate,P.PlannedDisbursementPeriodFromDate)) AS MaxPlannedDisbursementPeriodToDate
	FROM  tblProjectFundingPlannedDisbursement AS P GROUP BY P.ProjectId) AS PD ON P.Id = PD.ProjectId
LEFT OUTER JOIN
(SELECT P3.ProjectId, AVG(DATEDIFF(DAY, P3.PlannedDisbursementPeriodFromDate, COALESCE(P3.PlannedDisbursementPeriodToDate , P3.PlannedDisbursementPeriodFromDate))) AS AVGPlanDisDATEDIFF
	FROM tblProjectFundingPlannedDisbursement AS P3 GROUP BY P3.ProjectId) AS P3 ON P.Id = P3.ProjectId
--Disbursements
LEFT OUTER JOIN
(SELECT D1.ProjectId, COUNT(D1.Id) AS NoOfDisbursement
	FROM tblProjectFundingActualDisbursement AS D1 GROUP BY D1.ProjectId) AS D1 ON P.Id = D1.ProjectId
LEFT OUTER JOIN
(SELECT D2.ProjectId, SUM(D2.DisbursedAmount) AS TotalActualDisb, SUM(D2.DisbursedAmountInUSD) AS TotalActualDisbUSD 
	FROM tblProjectFundingActualDisbursement AS D2 GROUP BY D2.ProjectId) AS D2 ON P.Id = D2.ProjectId
LEFT OUTER JOIN
(SELECT D3.ProjectId, AVG(DATEDIFF(DAY, D3.DisbursementDate, COALESCE(D3.DisbursementToDate, D3.DisbursementDate))) AS AVGDisDATEDIFF
	FROM tblProjectFundingActualDisbursement AS D3 GROUP BY D3.ProjectId) AS D3 ON P.Id = D3.ProjectId
--Sector
LEFT OUTER JOIN
(SELECT S.ProjectId, Count(S.SectorId) AS NoOfSector FROM tblProjectSectoralAllocation AS S GROUP BY S.ProjectId) AS S ON P.Id = S.ProjectId
--Location
LEFT OUTER JOIN
(SELECT G.ProjectId, Count(G.Id) AS NoOfLocations FROM tblProjectGeographicAllocation AS G GROUP BY G.ProjectId) AS G ON P.Id = G.ProjectId
--Documents
LEFT OUTER JOIN
(SELECT A.ProjectId, Count(A.Id) AS NoOfAttachments FROM tblProjectAttachments AS A GROUP BY A.ProjectId) AS A ON P.Id = A.ProjectId