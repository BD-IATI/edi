
USE [AIMS_DB_IATI]
GO
/****** Object:  XmlSchemaCollection [dbo].[MyCollection]    Script Date: 06-12-16 19.15.17 ******/
CREATE XML SCHEMA COLLECTION [dbo].[MyCollection] AS N'<xsd:schema xmlns:xsd="http://www.w3.org/2001/XMLSchema"><xsd:import namespace="http://www.w3.org/XML/1998/namespace" /><xsd:attribute name="value-date" type="xsd:date" /><xsd:element name="comment" type="textRequiredType" /><xsd:element name="description"><xsd:complexType><xsd:complexContent><xsd:restriction base="xsd:anyType"><xsd:sequence><xsd:element ref="narrative" maxOccurs="unbounded" /><xsd:any namespace="##other" processContents="lax" minOccurs="0" maxOccurs="unbounded" /></xsd:sequence><xsd:anyAttribute namespace="##other" processContents="lax" /></xsd:restriction></xsd:complexContent></xsd:complexType></xsd:element><xsd:element name="narrative"><xsd:complexType><xsd:simpleContent><xsd:extension base="xsd:string"><xsd:attribute ref="xml:lang" /><xsd:anyAttribute namespace="##other" processContents="lax" /></xsd:extension></xsd:simpleContent></xsd:complexType></xsd:element><xsd:element name="reporting-org"><xsd:complexType><xsd:complexContent><xsd:restriction base="xsd:anyType"><xsd:sequence><xsd:element ref="narrative" maxOccurs="unbounded" /><xsd:any namespace="##other" processContents="lax" minOccurs="0" maxOccurs="unbounded" /></xsd:sequence><xsd:attribute name="ref" type="xsd:string" use="required" /><xsd:attribute name="type" type="xsd:string" use="required" /><xsd:attribute name="secondary-reporter" type="xsd:boolean" /><xsd:anyAttribute namespace="##other" processContents="lax" /></xsd:restriction></xsd:complexContent></xsd:complexType></xsd:element><xsd:element name="title" type="textRequiredType" /><xsd:complexType name="textRequiredType"><xsd:complexContent><xsd:restriction base="xsd:anyType"><xsd:sequence><xsd:element ref="narrative" maxOccurs="unbounded" /><xsd:any namespace="##other" processContents="lax" minOccurs="0" maxOccurs="unbounded" /></xsd:sequence><xsd:anyAttribute namespace="##other" processContents="lax" /></xsd:restriction></xsd:complexContent></xsd:complexType><xsd:complexType name="textType"><xsd:complexContent><xsd:restriction base="xsd:anyType"><xsd:sequence><xsd:element ref="narrative" minOccurs="0" maxOccurs="unbounded" /><xsd:any namespace="##other" processContents="lax" minOccurs="0" maxOccurs="unbounded" /></xsd:sequence><xsd:anyAttribute namespace="##other" processContents="lax" /></xsd:restriction></xsd:complexContent></xsd:complexType></xsd:schema>'
GO
/****** Object:  Table [dbo].[Activity]    Script Date: 06-12-16 19.15.17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Activity](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrgId] [nvarchar](50) NULL,
	[IatiIdentifier] [nvarchar](50) NULL,
	[IatiActivity] [xml] NULL,
	[DownloadDate] [datetime] NULL,
	[IatiActivityPrev] [xml] NULL,
	[DownloadDatePrev] [datetime] NULL,
	[Hierarchy] [int] NULL,
	[ParentHierarchy] [int] NULL,
	[IsInclude] [bit] NOT NULL,
	[AssignedOrgId] [nvarchar](50) NULL,
	[AssignedDate] [datetime] NULL,
	[ProjectId] [int] NULL,
	[MappedProjectId] [int] NULL,
	[MappedTrustFundId] [int] NULL,
	[IsIgnore] [bit] NOT NULL,
 CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ExchangeRateFederal]    Script Date: 06-12-16 19.15.17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExchangeRateFederal](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NULL,
	[Rate] [decimal](18, 6) NULL,
	[Currency] [nvarchar](50) NULL,
	[Frequency] [nvarchar](50) NULL,
	[InsertDate] [datetime] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FieldMappingPreferenceActivity]    Script Date: 06-12-16 19.15.17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldMappingPreferenceActivity](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IatiIdentifier] [nvarchar](50) NULL,
	[ProjectId] [int] NULL,
	[FieldName] [nvarchar](50) NOT NULL,
	[IsSourceIATI] [bit] NOT NULL,
 CONSTRAINT [PK_ActivityFieldMap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FieldMappingPreferenceDelegated]    Script Date: 06-12-16 19.15.17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldMappingPreferenceDelegated](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[IatiIdentifier] [nvarchar](50) NULL,
	[FieldName] [nvarchar](50) NULL,
	[IsInclude] [bit] NULL,
 CONSTRAINT [PK_FieldMappingPreferenceDelegated] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[FieldMappingPreferenceGeneral]    Script Date: 06-12-16 19.15.17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldMappingPreferenceGeneral](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrgId] [nvarchar](100) NULL,
	[FundSourceId] [int] NULL,
	[FieldName] [nvarchar](100) NOT NULL,
	[IsSourceIATI] [bit] NOT NULL,
 CONSTRAINT [PK_GeneralFieldMapPreference] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Log]    Script Date: 06-12-16 19.15.17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LogType] [int] NULL,
	[OrgId] [nvarchar](50) NULL,
	[IatiIdentifier] [nvarchar](50) NULL,
	[ProjectId] [int] NULL,
	[Message] [nvarchar](500) NULL,
	[ExceptionObj] [nvarchar](max) NULL,
	[DateTime] [datetime] NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[Activity] ADD  CONSTRAINT [DF_Activity_IsInclude]  DEFAULT ((1)) FOR [IsInclude]
GO
ALTER TABLE [dbo].[Activity] ADD  CONSTRAINT [DF_Activity_IsIgnore]  DEFAULT ((0)) FOR [IsIgnore]
GO
ALTER TABLE [dbo].[ExchangeRateFederal] ADD  CONSTRAINT [DF_ExchangeRateFederal_InsertDate]  DEFAULT (getdate()) FOR [InsertDate]
GO
ALTER TABLE [dbo].[Log] ADD  CONSTRAINT [DF_Log_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Used for co-finance projects' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Activity', @level2type=N'COLUMN',@level2name=N'MappedProjectId'
GO
USE [master]
GO
ALTER DATABASE [AIMS_DB_IATI] SET  READ_WRITE 
GO
