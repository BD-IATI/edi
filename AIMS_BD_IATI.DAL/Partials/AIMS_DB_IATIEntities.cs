namespace AIMS_BD_IATI.DAL
{
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
