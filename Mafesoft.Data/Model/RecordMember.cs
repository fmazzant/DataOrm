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

using ColumnName = System.Collections.Generic.List<System.String>;
using ColumnNameToMemberName = System.Collections.Generic.Dictionary<System.String, System.String>;

using FieldName = System.Collections.Generic.List<System.String>;
using FieldNameExternKey = System.Collections.Generic.List<System.String>;
using FieldNameIdentity = System.Collections.Generic.List<System.String>;
using FieldNameKey = System.Collections.Generic.List<System.String>;
using FieldNameObject = System.Collections.Generic.List<System.String>;
using FieldNameToMemberName = System.Collections.Generic.Dictionary<System.String, System.String>;
using MemberName = System.Collections.Generic.List<System.String>;
using RecordContext = System.Collections.Generic.Dictionary<System.Type, Mafesoft.Data.Core.MemberContext>;

namespace Mafesoft.Data.Core
{
    using Mafesoft.Data.Core.Attribute;
    using Mafesoft.Data.Core.Collection.Common;
    using Mafesoft.Data.Core.Column;
    using Mafesoft.Data.Core.Common;
    using Mafesoft.Data.Core.Parameter;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Reflection;

    /// <summary>
    /// Represents a pair of Member and Field.
    /// </summary>
    public class Pair
    {
        private Field _Field = null;
        private MemberInfo _Member = null;

        /// <summary>
        /// Create a new instance of Pair
        /// </summary>
        /// <param name="pMember">Member</param>
        /// <param name="pField">Field</param>
        public Pair(MemberInfo pMember, Field pField)
        {
            _Member = pMember;
            _Field = pField;
        }

        public MemberInfo Member { get { return _Member; } }

        public Field MemberFiled { get { return _Field; } }
    }

    /// <summary>
    /// Instance Member of Context
    /// </summary>
    internal class MemberContext
    {
        public String ConnectionString = null;
        public DbProviderFactory ProviderFactory = null;

        public String CatalogName = String.Empty;
        public String SchemeName = String.Empty;
        public String TableName = String.Empty;

        public ColumnName m_ColumnNames = new ColumnName();
        public MemberName m_Members = new MemberName();
        public FieldName m_FieldNames = new FieldName();
        public FieldNameKey m_FieldNameKeys = new FieldNameKey();
        public FieldNameExternKey m_FieldNameExternKeys = new FieldNameExternKey();
        public FieldNameIdentity m_FieldNameIdentites = new FieldNameIdentity();
        public FieldNameObject m_FieldNameObjects = new FieldNameObject();

        public ColumnNameToMemberName m_ColumnNameToMemberName = new ColumnNameToMemberName();
        public FieldNameToMemberName m_FieldNameToMemberName = new FieldNameToMemberName();

        public List<RecordColumn> m_Columns = new List<RecordColumn>();
        public Dictionary<string, PropertyInfo> m_propertyMap;

        /// <summary>
        /// Clear a context
        /// </summary>
        public void Clear()
        {
            ConnectionString = String.Empty;
            ProviderFactory = null;
            TableName = String.Empty;

            m_ColumnNames.Clear();
            m_Members.Clear();
            m_FieldNames.Clear();
            m_FieldNameKeys.Clear();
            m_FieldNameExternKeys.Clear();
            m_FieldNameIdentites.Clear();

            m_ColumnNameToMemberName.Clear();
            m_FieldNameToMemberName.Clear();
            m_Columns.Clear();
        }

        /// <summary>
        /// PropertyIdentityName's name
        /// </summary>
        public string PropertyIdentityName { get; set; }

        /// <summary>
        /// FieldIdentityName's name
        /// </summary>
        public string FieldIdentityName { get; set; }

        /// <summary>
        /// return enumerable column's names
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<String> GetColumns(Type type)
        {
            foreach (String rc in RecordMember.Current[type].m_ColumnNames)
                yield return rc;
            yield break;
        }
    }

    /// <summary>
    /// Abstract class to represent a record's members and build it.
    /// </summary>
    [FieldTable(null)]
    public abstract class RecordMember : RecordBase
    {
        internal Type RecordType = null;
        private Boolean _HasValue = true;
        private Boolean _IsNew = true;
        private List<RecordColumn> _Columns = new List<RecordColumn>();

        internal object[] ItemArray = null;
        internal ListRecord ListRecordParent;

        internal static RecordContext Current { get; set; }

        public abstract String ConnectionString { get; }

        public abstract DbProviderFactory ProviderFactory { get; }

        /// <summary>
        /// Create a new RecordMember instance.
        /// </summary>
        /// <param name="pTransaction"></param>
        protected RecordMember(DbTransaction pTransaction)
            : base(pTransaction)
        {
        }

        /// <summary>
        /// Is true when record has a valid value.
        /// </summary>
        public Boolean HasValue
        {
            get { return _HasValue; }
            set { _HasValue = value; }
        }

        /// <summary>
        /// Is true when record is a new record for database.
        /// </summary>
        public Boolean IsNew
        {
            get { return _IsNew; }
            set { _IsNew = value; }
        }

        /// <summary>
        /// TableName is table's name property.
        /// </summary>
        public virtual String TableName
        {
            get
            {
                if (!String.IsNullOrEmpty(Current[RecordType].CatalogName) && !String.IsNullOrEmpty(Current[RecordType].SchemeName))
                {
                    String tableName = String.Format("{0}.{1}.{2}", Current[RecordType].CatalogName, Current[RecordType].SchemeName, Current[RecordType].TableName);
                    return tableName;
                }
                return Current[RecordType].TableName;
            }
        }

        /// <summary>
        /// List of Columns in this record
        /// </summary>
        public List<RecordColumn> Columns
        {
            get { return _Columns; }// Current[RecordType].Columns; }
        }

        /// <summary>
        /// Dictionary of complete attributes list
        /// </summary>
        internal Dictionary<String, Field> AllAttributes
        {
            get { return null; }// Current[RecordType].AllAttributes; }
        }

        /// <summary>
        /// List of generic field in this record
        /// </summary>
        internal List<MemberInfo> Members
        {
            get { return null; }// Current[RecordType].Attributes; }
        }

        /// <summary>
        /// List of ExternKeys in this record
        /// </summary>
        internal List<MemberInfo> ExternKeys
        {
            get { return null; }// Current[RecordType].ExternKeys; }
        }

        /// <summary>
        /// FieldIdentityName is a Identity's name
        /// </summary>
        internal String FieldIdentityName
        {
            get { return Current[RecordType].FieldIdentityName; }
        }

        /// <summary>
        /// PropertyIdentityname is a Identity name of property
        /// </summary>
        internal String PropertyIdentityName
        {
            get { return Current[RecordType].PropertyIdentityName; }
        }

        /// <summary>
        /// List of Identities in this record
        /// </summary>
        internal List<MemberInfo> Identities
        {
            get { return null; }// Current[RecordType].Identities; }
        }

        /// <summary>
        /// List of Key field in this record
        /// </summary>
        internal List<MemberInfo> Keys
        {
            get { return null; }//Current[RecordType].Keys; }
        }

        /// <summary>
        /// FieldKeyName is a key's name
        /// </summary>
        //protected String FieldKeyName
        //{
        //    get { return Current[RecordType].FieldKeyName; }
        //}

        #region Initialize

        /// <summary>
        /// Force initialize a record.This method providers initialize ConnectionString and ProviderFactory instance.
        /// </summary>
        /// <param name="pTypeClass"></param>
        [Obsolete("ForceInitializeRecord will deprecated.", false)]
        protected static void ForceInitializeRecord(Type pTypeClass)
        {
            object[] members = pTypeClass.GetCustomAttributes(typeof(FieldProviderName), true);
            foreach (object obj in members)
            {
                if (obj is FieldProviderName)
                {
                    FieldProviderName ft = obj as FieldProviderName;
                    //SharedProviderFactory = CreateProviderFactoty(ft.ProviderSupport);
                    break;
                }
            }
            members = pTypeClass.GetCustomAttributes(typeof(FieldConnectionString), true);
            foreach (object obj in members)
            {
                if (obj is FieldConnectionString)
                {
                    FieldConnectionString ft = obj as FieldConnectionString;
                    pTypeClass.GetField("").SetValue(null, ft.ConnectionString);
                    //ConnectionString = ft.ConnectionString;
                    break;
                }
            }
        }

        /// <summary>
        /// Initialize base class, it creates a provider factory and connection
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        internal override void Initialize(DbTransaction pTransaction)
        {
            RecordType = GetType();
            InitializeContext();
        }

        internal void InitializeContext()
        {
            if (Current == null) Current = new RecordContext();

            Type currentType = GetType();
            InitializeMembers(RecordType);
        }

        /// <summary>
        /// Build a table name, record provider factory, connection string and initialize a base information for execution
        /// a generic query.
        /// </summary>
        /// <param name="pType">Type of record</param>
        internal void InitializeCustomAttributes(Type pType)
        {
        }

        /// <summary>
        /// Build a list of memeber key, attributes, externkeys and identities
        /// </summary>
        /// <param name="pType"></param>
        internal void InitializeMembers(Type pType)
        {
            lock (RecordType)
            {
                RecordType = pType;
                if (Current.ContainsKey(pType))
                {
                    //ConnectionString = Current[pType].ConnectionString;
                    RecordConnectionString = Current[pType].ConnectionString;
                    RecordProviderFactory = Current[pType].ProviderFactory;

                    if (String.IsNullOrEmpty(RecordConnectionString)) RecordConnectionString = ConnectionString;
                    if (RecordProviderFactory == null) RecordProviderFactory = ProviderFactory;// Record.SharedProviderFactory;

                    _Columns = new List<RecordColumn>(Current[pType].m_Columns);
                    foreach (RecordColumn rc in _Columns) rc.Record = this;
                }
                else
                {
                    MemberContext mc = new MemberContext();
                    Current.Add(pType, mc);
                    Current[pType].Clear();

                    object[] customs = pType.GetCustomAttributes(true);
                    foreach (object obj in customs)
                    {
                        if (obj is FieldCatalog) Current[pType].CatalogName = String.Format("[{0}]", (obj as FieldCatalog).CatalogName);
                        if (obj is FieldScheme) Current[pType].SchemeName = String.Format("[{0}]", (obj as FieldScheme).SchemeName);
                        if (obj is FieldTable) Current[pType].TableName = String.Format("[{0}]", (obj as FieldTable).TableName);
                        if (obj is FieldConnectionString) Current[pType].ConnectionString = (obj as FieldConnectionString).ConnectionString;
                        if (obj is FieldProviderName) Current[pType].ProviderFactory = CreateProviderFactoty((obj as FieldProviderName).ProviderSupport);
                    }
                    //ConnectionString = Current[pType].ConnectionString;
                    RecordConnectionString = Current[pType].ConnectionString;
                    RecordProviderFactory = Current[pType].ProviderFactory;
                    //if (String.IsNullOrEmpty(SharedConnectionString)) SharedConnectionString = RecordConnectionString;
                    //if (SharedProviderFactory == null) SharedProviderFactory = RecordProviderFactory;

                    //if (String.IsNullOrEmpty(RecordConnectionString)) RecordConnectionString = ConnectionString;
                    if (RecordProviderFactory == null) RecordProviderFactory = ProviderFactory;// SharedProviderFactory;
                    // if (String.IsNullOrEmpty(RecordConnectionString)) RecordConnectionString = CreateConnectionString(pType);
                    if (RecordProviderFactory == null) RecordProviderFactory = ProviderFactory;// Record.SharedProviderFactory;
                    if (RecordProviderFactory != null) RecordConnection = RecordProviderFactory.CreateConnection();

                    //if (String.IsNullOrEmpty(RecordConnectionString)) throw new RecordConnectionStringNullException("ConnectionString is null value");
                    //if (ProviderFactory == null) throw new RecordProviderFactoryNullException("ProviderFactory is null value");
                    //if (RecordConnection == null) throw new RecordConnectionNullException("Connection is null value");

                    MemberInfo[] members = pType.GetProperties(RecordBindingFlags);

                    foreach (PropertyInfo member in members)
                    {
                        if (member.MemberType == MemberTypes.Property)
                        {
                            object[] objs = member.GetCustomAttributes(typeof(Field), true);
                            foreach (Object obj in objs)
                            {
                                if (obj is Field)
                                {
                                    Field field = obj as Field;
                                    Current[pType].m_ColumnNames.Add(field.Name);
                                    Current[pType].m_Members.Add(member.Name);
                                    Current[pType].m_FieldNames.Add(field.Name);

                                    if (field.IsPrimaryKey)
                                    {
                                        Current[pType].m_FieldNameKeys.Add(field.Name);
                                    }
                                    if (field.IsIdentity)
                                    {
                                        Current[pType].m_FieldNameIdentites.Add(field.Name);
                                        Current[pType].PropertyIdentityName = member.Name;
                                        Current[pType].FieldIdentityName = field.Name;
                                    }
                                    if (field.Type == FieldType.ExternalKey)
                                    {
                                        Current[pType].m_FieldNameExternKeys.Add(field.Name);
                                    }
                                    Current[pType].m_ColumnNameToMemberName.Add(field.Name.TrimEnd(), member.Name.TrimEnd());
                                    Current[pType].m_FieldNameToMemberName.Add(field.Name.TrimEnd(), member.Name.TrimEnd());
                                    RecordColumn rc = new RecordColumn(field.Name.TrimEnd(), member.Name.TrimEnd(), field.ObjType, field, this);
                                    Current[pType].m_Columns.Add(rc);
                                    _Columns.Add(rc);
                                }
                                if (obj is FieldObject)
                                {
                                    //FieldObject field = obj as FieldObject;
                                    //Current[pType].m_FieldNameObjects.Add(field.FieldName);
                                    //Current[pType].m_ColumnNameToMemberName.Add(field.FieldName, member.Name);
                                    //Current[pType].m_FieldNameToMemberName.Add(field.FieldName, member.Name);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion Initialize

        /// <summary>
        /// Chooses the current connection string.
        /// </summary>
        /// <returns></returns>
        protected String ChooseCurrentConnectionString()
        {
            if (!String.IsNullOrEmpty(this.ConnectionString)) return this.ConnectionString;
            if (!String.IsNullOrEmpty(this.RecordConnectionString)) return this.RecordConnectionString;
            throw new RecordConnectionNullException("ConnectionString is missing.");
        }

        /// <summary>
        /// Create a new connection instance
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <returns></returns>
        public DbConnection CreateConnection(DbTransaction pTransaction, String pConnectionString)
        {
            if (pConnectionString == null)
                throw new RecordConnectionStringNullException("ConnectionString is null!"); ;
            if (pTransaction != null)
                return pTransaction.Connection;
            DbConnection conn = ProviderFactory.CreateConnection();
            conn.ConnectionString = pConnectionString;
            return conn;
        }

        /// <summary>
        /// Create a new command instance
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <returns></returns>
        public DbCommand CreateCommand(DbTransaction pTransaction)
        {
            return ProviderFactory.CreateCommand();
        }

        /// <summary>
        /// Create a new parameter instance
        /// </summary>
        /// <param name="pParameterName">Parameter's name</param>
        /// <param name="pCompareKind">Compare operation</param>
        /// <param name="pValue">Parameter's value</param>
        /// <returns>Parameter</returns>
        protected RecordParameter CreateParameter(String pParameterName, CompareKind pCompareKind, object pValue)
        {
            if (ProviderFactory == null)
            {
                throw new RecordProviderFactoryNullException("Provider Factory is null");
            }

            DbParameter parameter = (DbParameter)ProviderFactory.CreateParameter();
            parameter.ParameterName = pParameterName;
            parameter.Value = pValue != null ? pValue : String.Empty;
            RecordParameter rparameter = new RecordParameter(parameter, pCompareKind);
            return rparameter;
        }

        /// <summary>
        /// Create a new parameter instance
        /// </summary>
        /// <param name="pParameterName">Parameter's name</param>
        /// <param name="pValue">Parameter's value</param>
        /// <returns>Parameter</returns>
        protected RecordParameter CreateParameter(String pParameterName, object pValue)
        {
            return CreateParameter(pParameterName, CompareKind.Equal, pValue);
        }

        /// <summary>
        /// Execute a statment with transaction
        /// </summary>
        /// <param name="handler">Handler</param>
        public void ExecuteStatmentWithTransaction(StatmentHandler handler)
        {
            ExecuteStatmentWithTransaction(null, handler);
        }

        /// <summary>
        /// Execute a statment with transaction
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="handler">Handler</param>
        public void ExecuteStatmentWithTransaction(DbConnection pConnection, StatmentHandler handler)
        {
            if (pConnection == null)
                pConnection = CreateConnection(null, this.ChooseCurrentConnectionString());

            if (pConnection == null)
                throw new RecordConnectionNullException("You enabled context and init it for this execution");

            if (handler == null)
                throw new RecordHandlerNullException();

            try
            {
                pConnection.Open();
                DbTransaction transaction = pConnection.BeginTransaction();
                try
                {
                    handler(transaction);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new TargetInvocationException(e);
                }
                finally
                {
                    pConnection.Close();
                }
            }
            catch (Exception e)
            {
                throw new TargetInvocationException(e);
            }
        }

        /// <summary>
        /// Executes the statment with transaction.
        /// </summary>
        /// <param name="pConnection">The p connection.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="completed">The completed.</param>
        public void ExecuteStatmentWithTransaction(DbConnection pConnection, StatmentHandler handler, StatmentCompletedHandler completed)
        {
            this.ExecuteStatmentWithTransaction(pConnection, handler);
            completed();
        }

        /// <summary>
        /// Executes the statment with transaction.
        /// </summary>
        /// <param name="pConnection">The p connection.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="completed">The completed.</param>
        /// <param name="error">The error.</param>
        public void ExecuteStatmentWithTransaction(DbConnection pConnection, StatmentHandler handler, StatmentCompletedHandler completed, StatmentErrorHandler error)
        {
            try
            {
                this.ExecuteStatmentWithTransaction(pConnection, handler);
                completed();
            }
            catch (Exception e)
            {
                error(e);
            }
        }

        /// <summary>
        /// Reset o clear a field's value
        /// </summary>
        /// <param name="pRecord">Record</param>
        internal void ResetFieldValue(Record pRecord)
        {
            _IsNew = true;
            _HasValue = false;
            foreach (String memberName in Current[this.GetType()].m_Members)
                SetMemberValue(memberName, null);
        }

        /// <summary>
        /// Set or Get a value from record's column
        /// </summary>
        /// <param name="index">Column's index</param>
        /// <returns></returns>
        public object this[int index]
        {
            get
            {
                String name = Current[GetType()].m_ColumnNameToMemberName[Columns[index].ColumnName];
                return GetMemberValue(name);
            }
            set
            {
                String name = Current[GetType()].m_ColumnNameToMemberName[Columns[index].ColumnName];
                if (Columns[index].ColumnType == value.GetType())
                    SetMemberValue(name, value);
                else
                    throw new NotSupportedException(Columns[index].ColumnType + " with " + value.GetType());
            }
        }

        /// <summary>
        /// Set or Get a value from record's column
        /// </summary>
        /// <param name="columnName">Column's name</param>
        /// <returns></returns>
        public object this[String columnName]
        {
            get
            {
                String name = Current[GetType()].m_ColumnNameToMemberName[columnName];
                return GetMemberValue(name);
            }
            set
            {
                String name = Current[GetType()].m_ColumnNameToMemberName[columnName];
                SetMemberValue(name, value);
            }
        }

        /// <summary>
        /// Set or Get a value from record's column
        /// </summary>
        /// <param name="column">RecordColumn instance of column</param>
        /// <returns></returns>
        public object this[RecordColumn column]
        {
            get
            {
                String name = Current[GetType()].m_ColumnNameToMemberName[column.ColumnName];
                return GetMemberValue(name);
            }
            set
            {
                String name = Current[GetType()].m_ColumnNameToMemberName[column.ColumnName];
                SetMemberValue(name, value);
                //column.ColumnOriginValue = value;
            }
        }

        /// <summary>
        /// Get value from Member name
        /// </summary>
        /// <param name="pMemberName"></param>
        /// <returns></returns>
        protected object GetMemberValue(String pMemberName)
        {
            Object Value = null;
            if (Current[GetType()].m_Members.Contains(pMemberName))
            {
                PropertyInfo property = GetType().GetProperty(pMemberName);

                Value = property.GetValue(this, null);

                if (Value == null)
                    return DBNull.Value;

                return Value;
            }
            throw new NotSupportedException(pMemberName + " is not exist!");
        }

        /// <summary>
        /// Get value from MemberInfo instance
        /// </summary>
        /// <param name="pMember"></param>
        /// <returns></returns>
        [Obsolete("", true)]
        protected object GetMemberValue(MemberInfo pMember)
        {
            Object Value = null;

            if (pMember == null)
                return Value;

            if (pMember is PropertyInfo)
                Value = (pMember as PropertyInfo).GetValue(this, null);

            if (pMember is FieldInfo)
                Value = (pMember as FieldInfo).GetValue(this);

            if (Value == null)
                return DBNull.Value;

            return Value;
        }

        /// <summary>
        /// Set field's value by DataRow
        /// </summary>
        /// <param name="pDataRow"></param>
        protected void SetFieldsValue(DataRow pDataRow)
        {
            foreach (String memberName in Current[GetType()].m_Members)
            {
                MemberInfo member = GetType().GetMember(memberName)[0];
                object val = null;
                if (pDataRow is DataRow)
                    val = ((DataRow)pDataRow)[Current[this.GetType()].m_ColumnNameToMemberName[member.Name]];
                else if (pDataRow is IDataReader)
                    val = ((IDataReader)pDataRow)[Current[this.GetType()].m_ColumnNameToMemberName[member.Name]];
                if (val == DBNull.Value)
                    val = null;
                SetMemberValue(member.Name, val);
            }
            _IsNew = false;
            _HasValue = true;

            //foreach (MemberInfo member in _FieldObjects)
            //    SetObjectOnLoad(member);
        }

        /// <summary>
        /// Set field's value from memeber's name and member's value
        /// </summary>
        /// <param name="pMemberName">Member's name</param>
        /// <param name="pValue">Member's value</param>
        protected void SetMemberValue(String pMemberName, object pValue)
        {
            PropertyInfo property = GetType().GetProperty(pMemberName);

            if (property == null)
                return;
            if (pValue == null)
                return;
            if (pValue == DBNull.Value)
                return;
            property.SetValue(this, pValue, null);
            return;
        }

        /// <summary>
        /// Set field's value from memeberInfo's and member's value
        /// </summary>
        /// <param name="pMember">MemberInfo instance</param>
        /// <param name="pValue">Member's value</param>
        [Obsolete("", true)]
        protected void SetMemberValue(MemberInfo pMember, object pValue)
        {
            if (pValue == null)
                return;
            if (pValue == DBNull.Value)
                return;

            if (pMember is PropertyInfo)
                (pMember as PropertyInfo).SetValue(this, pValue, null);

            if (pMember is FieldInfo)
                (pMember as FieldInfo).SetValue(this, pValue);

            //_Columns[pMember.Name].ColumnOriginValue = pValue;
        }

        /// <summary>
        /// Set a FieldObject value and load it when MustBeLoad's value is true.
        /// </summary>
        [Obsolete("SetObjectOnLoad not active in this moment!", false)]
        protected void SetObjectOnLoad(MemberInfo pMemberInfo)
        {
            if (pMemberInfo.MemberType == MemberTypes.Property)
            {
                FieldObject[] obj = pMemberInfo.GetCustomAttributes(typeof(FieldObject), true) as FieldObject[];

                foreach (FieldObject field in obj)
                {
                    if (field.MustBeLoad)
                    {
                        object value = GetMemberValue(field.FieldName);
                        if (value == null)
                            throw new NullReferenceException("External Key is null!");

                        if (value != DBNull.Value)
                        {
                            object oValue = Activator.CreateInstance((pMemberInfo as PropertyInfo).PropertyType, new object[] { value });
                            SetMemberValue(pMemberInfo, oValue);
                        }
                    }
                }
            }
        }
    }
}