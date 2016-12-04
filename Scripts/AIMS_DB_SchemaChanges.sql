USE [AIMS_DB_Demo]
GO

ALTER TABLE tblProjectAttachments ALTER COLUMN AttachmentTitle VARCHAR(2000);
ALTER TABLE tblProjectAttachments ALTER COLUMN AttachmentFileName VARCHAR(2000);