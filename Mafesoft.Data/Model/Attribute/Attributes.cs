/// Mafesoft.Data
/// <summary>An abstract accessing to database</summary>
///
///
///                                                                    o o
///                                                                  o     o
///                                                                 _   O  _
///  Copyright(C) 2006                                                \/)\/
///  Federico Mazzanti                                               /\/|
///                                                                     |
///                                                                     \
///  All rights reserved.

using System;

namespace Mafesoft.Data.Core.Attribute
{
    /// <summary>
    /// A Field on the database table
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Field : System.Attribute
    {
        private Int32 _Index = -1;
        private Boolean _IsNullable = false;
        private Boolean _IsRequired = true;
        private String _Name = String.Empty;
        private Type _ObjType = typeof(Object);
        private Int32 _Size = 0;
        private FieldType _Type = FieldType.Normal;

        /// <summary>
        /// Create a Field attribute
        /// </summary>
        /// <param name="pFieldName">Field's name</param>
        public Field(String pFieldName)
            : this(pFieldName, FieldType.Normal, typeof(Object), false, true)
        {
        }

        /// <summary>
        /// Create a Field attribute
        /// </summary>
        /// <param name="pFieldName">Field's name</param>
        /// <param name="pFieldType">Field's type</param>
        public Field(String pFieldName, FieldType pFieldType)
            : this(pFieldName, pFieldType, typeof(Object), false, true)
        {
        }

        /// <summary>
        /// Create a Field attribute
        /// </summary>
        /// <param name="pFieldName">Field's name</param>
        /// <param name="pFieldType">Field's type</param>
        /// <param name="pType">Field's value type</param>
        public Field(String pFieldName, FieldType pFieldType, Type pType)
            : this(pFieldName, pFieldType, pType, false, true)
        {
        }

        /// <summary>
        /// Create a Field attribute
        /// </summary>
        /// <param name="pFieldName">Field's name</param>
        /// <param name="pFieldType">Field's type</param>
        /// <param name="pType">Fiueld's value type</param>
        /// <param name="pIsNullable">Field is can be null value</param>
        /// <param name="pIsRequired">Field is required or not</param>
        public Field(String pFieldName, FieldType pFieldType, Type pType, Boolean pIsNullable, Boolean pIsRequired)
            : base()
        {
            Name = pFieldName;
            Type = pFieldType;
            ObjType = pType;
            IsNullable = pIsNullable;
            IsRequired = pIsRequired;
            Size = 0;
            Index = -1;
        }

        /// <summary>
        /// Field's index
        /// </summary>
        public Int32 Index
        {
            get { return _Index; }
            set { _Index = value; }
        }

        /// <summary>
        /// True when Field is a Indetity. Witch value inserting is increment by database
        /// </summary>
        public Boolean IsIdentity
        {
            get
            {
                return (Type == FieldType.PrimaryKeyIdentity | Type == FieldType.Identity);
            }
        }

        /// <summary>
        /// That field is nullable
        /// </summary>
        public Boolean IsNullable
        {
            get { return _IsNullable; }
            set { _IsNullable = value; }
        }

        /// <summary>
        /// True when Field is a Primary Key
        /// </summary>
        public Boolean IsPrimaryKey
        {
            get
            {
                return (Type == FieldType.PrimaryKey | Type == FieldType.PrimaryKeyIdentity);
            }
        }

        /// <summary>
        /// Field's required for mappaing, when record loaded from db row.
        /// </summary>
        public Boolean IsRequired
        {
            get { return _IsRequired; }
            set { _IsRequired = value; }
        }

        /// <summary>
        /// Field's name. This identical field on database
        /// </summary>
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Field's value type
        /// </summary>
        public Type ObjType
        {
            get { return _ObjType; }
            set { _ObjType = value; }
        }

        /// <summary>
        /// Field's type of size.
        /// </summary>
        public Int32 Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        /// <summary>
        /// Field's type
        /// </summary>
        public FieldType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
    }

    /// <summary>
    /// A ConnectionString attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FieldConnectionString : System.Attribute
    {
        private String _ConnectionString = String.Empty;

        /// <summary>
        /// Create a new instance of ConnectionString
        /// </summary>
        /// <param name="pConnectionString">ConnectionString</param>
        public FieldConnectionString(String pConnectionString)
            : base()
        {
            ConnectionString = pConnectionString;
        }

        /// <summary>
        /// Field's connection string
        /// </summary>
        public String ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }
    }

    /// <summary>
    /// A Filed on the database table on External Key. This is an object that it instanced.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldObject : System.Attribute
    {
        private String _FieldName = String.Empty;
        private Boolean _MustBeLoad = false;

        /// <summary>
        ///  Creata FileObject attribute
        /// </summary>
        /// <param name="pFieldName">Field' name</param>
        public FieldObject(String pFieldName)
            : this(pFieldName, false)
        {
        }

        /// <summary>
        /// Creata FileObject attribute
        /// </summary>
        /// <param name="pFieldName">Field's name</param>
        /// <param name="pMustBeLoad">Field must to be load on startup.</param>
        public FieldObject(String pFieldName, Boolean pMustBeLoad)
        {
            if (String.IsNullOrEmpty(pFieldName))
                throw new NotSupportedException("FieldName's value can't empty or null!");
            FieldName = pFieldName;
            MustBeLoad = pMustBeLoad;
        }

        /// <summary>
        /// Creata FileObject attribute
        /// </summary>
        protected FieldObject()
            : this(null, false)
        {
        }

        /// <summary>
        /// Field's name
        /// </summary>
        public String FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }

        /// <summary>
        /// If this obj must be loaded when the initialing class
        /// </summary>
        public Boolean MustBeLoad
        {
            get { return _MustBeLoad; }
            set { _MustBeLoad = value; }
        }
    }

    /// <summary>
    /// A Provider name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FieldProviderName : System.Attribute
    {
        private String _ProviderName = String.Empty;
        private ProviderFactorySupport _ProviderSupport = ProviderFactorySupport.None;

        /// <summary>
        /// Create a new instance of ProviderName
        /// </summary>
        /// <param name="pProviderSupport">Field's provider factory support</param>
        public FieldProviderName(ProviderFactorySupport pProviderSupport)
            : base()
        {
            ProviderSupport = pProviderSupport;
            ProviderName = Convert.InternalConvert.ConvertProviderToString(pProviderSupport);
        }

        /// <summary>
        /// Field's provider name
        /// </summary>
        public String ProviderName
        {
            get { return _ProviderName; }
            set { _ProviderName = value; }
        }

        /// <summary>
        /// Field's provider factory support
        /// </summary>
        public ProviderFactorySupport ProviderSupport
        {
            get { return _ProviderSupport; }
            set { _ProviderSupport = value; }
        }
    }

    /// <summary>
    /// A Catalog name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class FieldCatalog : System.Attribute
    {
        private String _CatalogName = String.Empty;

        /// <summary>
        /// Create Attribute
        /// </summary>
        /// <param name="pTableName">Catalog's name</param>
        public FieldCatalog(String pCatalogName)
            : base()
        {
            CatalogName = pCatalogName;
        }

        /// <summary>
        /// Table's name
        /// </summary>
        public String CatalogName
        {
            get { return _CatalogName; }
            set { _CatalogName = value; }
        }
    }

    /// <summary>
    /// A Scheme name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class FieldScheme : System.Attribute
    {
        private String _SchemeName = String.Empty;

        /// <summary>
        /// Create Attribute
        /// </summary>
        /// <param name="pTableName">Scheme's name</param>
        public FieldScheme(String pSchemeName)
            : base()
        {
            SchemeName = pSchemeName;
        }

        /// <summary>
        /// Table's name
        /// </summary>
        public String SchemeName
        {
            get { return _SchemeName; }
            set { _SchemeName = value; }
        }
    }

    /// <summary>
    /// A Table name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class FieldTable : System.Attribute
    {
        private String _TableName = String.Empty;

        /// <summary>
        /// Create Attribute
        /// </summary>
        /// <param name="pTableName">Table's name</param>
        public FieldTable(String pTableName)
            : base()
        {
            TableName = pTableName;
        }

        /// <summary>
        /// Table's name
        /// </summary>
        public String TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }
    }
}