namespace AIMS_BD_IATI.DAL
{
    using AIMS_BD_IATI.Library;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;

    public partial class AIMS_DB_IATIEntities : DbContext
    {
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }
    }

    [Serializable]
    public partial class Log
    {
        public string LogTypeText { get { return LogType.HasValue ? Enum.GetName(typeof(LoggingType), LogType) : string.Empty; } }
        public string CssClass
        {
            get
            {
                if (LogType == (int)LoggingType.Info) return "label label-info";
                else if (LogType == (int)LoggingType.Warning) return "label label-warning";
                else if (LogType == (int)LoggingType.Error) return "label label-danger";
                else if (LogType == (int)LoggingType.Alert) return "label label-primary";
                else if (LogType == (int)LoggingType.ValidationError) return "label label-danger";
                else if (LogType == (int)LoggingType.FinancialDataMismathed) return "label label-warning";
                else if (LogType == (int)LoggingType.Success) return "label label-success";
                else return "label label-default";
            }
        }
    }

    [Serializable]
    public partial class FieldMappingPreferenceGeneral
    {
    }

    [Serializable]
    public partial class FieldMappingPreferenceDelegated
    {
    }

    [Serializable]
    public partial class FieldMappingPreferenceActivity
    {

    }
}
