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

namespace Mafesoft.Data
{
    /// <summary>
    /// Its represetes a record entity state change type
    /// </summary>
    public enum RecordStatusChangeType
    {
        New,
        Changed,
        Equal
    }
}
namespace Mafesoft.Data.Core
{
    using Mafesoft.Data.Convert;
    using Mafesoft.Data.Core.Attribute;
    using Mafesoft.Data.Core.Column;
    using Mafesoft.Data.Core.Common;
    using Mafesoft.Data.Core.Parameter;
    using Mafesoft.Data.Core.Parameter.Util;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Represents a generic Record
    /// </summary>
    public abstract class Record : RecordMember, IRecord
    {
        private String _DeleteCommandText = String.Empty;
        private String _InsertCommandText = String.Empty;
        private String _SelectCommandText = String.Empty;
        private String _UpdateCommandText = String.Empty;

        /// <summary>
        /// Create new record instance
        /// </summary>
        protected Record()
            : base(null)
        {
            HasValue = false;
            IsNew = true;
        }

        /// <summary>
        /// Create new record instance
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pIdentity">Identity of record value</param>
        protected Record(DbTransaction pTransaction, int pIdentity)
            : base(pTransaction)
        {
            SetMemberValue(FieldIdentityName, pIdentity);
            Load(pTransaction, pIdentity);
        }

        /// <summary>
        /// Create new record instance
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pParameters">List of parameters</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected Record(DbTransaction pTransaction, params RecordParameter[] pParameters)
            : base(pTransaction)
        {
            if (pParameters != null && pParameters.Length > 0)
                Load(pTransaction, pParameters);
        }

        /// <summary>
        /// IsChanged is true if Record entity is changed value. It is use only when record is not new.
        /// </summary>
        [Obsolete("You can use RecordStatusChange property", true)]
        public Boolean IsChanged
        {
            get
            {
                foreach (RecordColumn rc in Columns)
                    if (!IsNew && rc.ColumnOriginValue != null && !rc.ColumnOriginValue.Equals(rc.ColumnValue))
                        return true;
                return false;
            }
        }

        /// <summary>
        /// RecordStatusChange if Record entity is changed value. It is use only when record is not new.
        /// </summary>
        public RecordStatusChangeType RecordStatusChange
        {
            get
            {
                if (IsNew) return RecordStatusChangeType.New;
                foreach (RecordColumn rc in Columns)
                {
                    if (rc.ColumnOriginValue != null && rc.ColumnValue == null)
                        return RecordStatusChangeType.Changed;

                    if (rc.ColumnOriginValue == null && rc.ColumnValue != null)
                        return RecordStatusChangeType.Changed;

                    if (!IsNew && !rc.ColumnOriginValue.Equals(rc.ColumnValue))
                        return RecordStatusChangeType.Changed;
                }
                return RecordStatusChangeType.Equal;
            }
        }

        /// <summary>
        /// DeleteCommandText
        /// </summary>
        internal String DeleteCommandText
        {
            get { return _DeleteCommandText; }
        }

        /// <summary>
        /// InsertCommandText
        /// </summary>
        internal String InsertCommandText
        {
            get { return _InsertCommandText; }
        }

        /// <summary>
        /// SelectCommandText
        /// </summary>
        internal String SelectCommandText
        {
            get { return _SelectCommandText; }
        }

        /// <summary>
        /// UpdateCommandText
        /// </summary>
        internal String UpdateCommandText
        {
            get { return _UpdateCommandText; }
        }

        /// <summary>
        /// Count a number records contained in to specific table.
        /// </summary>
        /// <param name="pTypeClass">Record's type</param>
        /// <returns>Number of records</returns>
        public static int Count<TRECORD>(Type pTypeClass)
            where TRECORD : Record, new()
        {
            String tablename = GetTableName(pTypeClass);
            ForceInitializeRecord(pTypeClass);

            if (!String.IsNullOrEmpty(tablename))
            {
                DbDataAdapter adapter = CreateAdapter<TRECORD>();
                String Query = "SELECT COUNT(*) FROM " + tablename;
                adapter.SelectCommand = CreateCommand<TRECORD>(Query, null);
                //adapter.SelectCommand.CommandText = Query;
                adapter.SelectCommand.Connection = CreateConnection<TRECORD>(null, CreateConnectionString<TRECORD>(pTypeClass));
                adapter.SelectCommand.Connection.Open();
                object dr = adapter.SelectCommand.ExecuteScalar();
                adapter.SelectCommand.Connection.Close();
                return System.Convert.ToInt32(dr);
            }
            return -1;
        }

        /// <summary>
        /// Create a new instance TRECORD's type
        /// </summary>
        /// <typeparam name="TRECORD">Record's type</typeparam>
        /// <param name="pRecord">A reference of TRECORD's type</param>
        /// <param name="pDataRow">DataRow readed from database.</param>
        /// <returns>An instance of record</returns>
        public static Record CreateInstance<TRECORD>(TRECORD pRecord, DataRow pDataRow)
             where TRECORD : Record
        {
            //pRecord.ResetFieldValue(pRecord);
            pRecord.SetFieldsValue(pDataRow);
            return pRecord;
        }

        /// <summary>
        /// Create a new instance from reader
        /// </summary>
        /// <typeparam name="TRECORD"></typeparam>
        /// <param name="pRecord"></param>
        /// <param name="pReader"></param>
        /// <returns></returns>
        public static Record CreateInstance<TRECORD>(TRECORD pRecord, DbDataReader pReader)
             where TRECORD : Record, new()
        {
            DateTime s = DateTime.Now;

            if (pRecord == null)
                pRecord = new TRECORD();

            foreach (String columnName in Current[typeof(TRECORD)].m_ColumnNames)
            {
                String memberName = Current[typeof(TRECORD)].m_ColumnNameToMemberName[columnName];
                pRecord.SetMemberValue(memberName, pReader[columnName]);
            }

            pRecord.IsNew = false;
            pRecord.HasValue = true;

            return pRecord;
        }

        /// <summary>
        /// Create a new instance TRECORD's type
        /// </summary>
        /// <typeparam name="TRECORD">Record's type</typeparam>
        /// <returns>An instance of record</returns>
        public static TRECORD CreateNewInstance<TRECORD>()
           where TRECORD : Record, new()
        {
            return CreateNewInstance<TRECORD>(new RecordInstanceParameter("IsNew", true), new RecordInstanceParameter("HasValue", true));
        }

        /// <summary>
        /// Create a new instance TRECORD's type
        /// </summary>
        /// <typeparam name="TRECORD">Record's type</typeparam>
        /// <param name="pInstanceParameters">List of parameters from initialize when instance is created</param>
        /// <returns>An instance of record</returns>
        public static TRECORD CreateNewInstance<TRECORD>(params RecordInstanceParameter[] pInstanceParameters)
           where TRECORD : Record, new()
        {
            TRECORD item = new TRECORD();
            item.Reset();
            foreach (RecordInstanceParameter parameter in pInstanceParameters)
                item.SetMemberValue(parameter.PropertyName, parameter.Value);

            return item;
        }

        /// <summary>
        /// Delete all record from database, but not reset a index value
        /// </summary>
        /// <param name="pTypeClass">Record's type</param>
        /// <returns>if 0 then deleted all record else -1</returns>
        public static int DeleteAllFromDb<TRECORD>(Type pTypeClass)
            where TRECORD : Record, new()
        {
            String tablename = GetTableName(pTypeClass);
            ForceInitializeRecord(pTypeClass);

            if (!String.IsNullOrEmpty(tablename))
            {
                //CreateConnectionString();
                DbDataAdapter adapter = CreateAdapter<TRECORD>();
                String Query = "DELETE FROM " + tablename;
                adapter.DeleteCommand = CreateCommand<TRECORD>(Query, null);
                adapter.DeleteCommand.CommandText = Query;
                adapter.DeleteCommand.Connection = CreateConnection<TRECORD>(null, CreateConnectionString<TRECORD>(pTypeClass));
                adapter.DeleteCommand.Connection.Open();
                adapter.DeleteCommand.ExecuteNonQuery();
                adapter.DeleteCommand.Connection.Close();
                return 0;
            }
            return -1;
        }

        /// <summary>
        /// Get a list of record column's name
        /// </summary>
        /// <param name="pTypeClass">Record's type</param>
        /// <returns>List of RecordColumn</returns>
        public static RecordColumn[] GetRecordColumns(Type pTypeClass)
        {
            List<RecordColumn> list = new List<RecordColumn>();
            MemberInfo[] members = pTypeClass.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (PropertyInfo member in members)
            {
                if (member.MemberType == MemberTypes.Property)
                {
                    object[] objs = member.GetCustomAttributes(true);
                    foreach (Object obj in objs)
                    {
                        if (obj is Field)
                        {
                            Field field = obj as Field;
                            list.Add(new RecordColumn(field.Name, member.Name));
                        }
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Delete all record from database, but reset a index value and new index's value is 1.
        /// </summary>
        /// <param name="pTypeClass">Record's type</param>
        /// <returns>if 0 then deleted all record else -1</returns>
        public static int TruncateAllFromDb<TRECORD>(Type pTypeClass)
            where TRECORD : Record, new()
        {
            String tablename = GetTableName(pTypeClass);
            ForceInitializeRecord(pTypeClass);

            if (!String.IsNullOrEmpty(tablename))
            {
                //CreateConnectionString();
                DbDataAdapter adapter = CreateAdapter<TRECORD>();
                String Query = "TRUNCATE TABLE " + tablename;
                adapter.DeleteCommand = CreateCommand<TRECORD>(Query, null);
                adapter.DeleteCommand.CommandText = Query;
                adapter.DeleteCommand.Connection = CreateConnection<TRECORD>(null, CreateConnectionString<TRECORD>(pTypeClass));
                adapter.DeleteCommand.Connection.Open();
                adapter.DeleteCommand.ExecuteNonQuery();
                adapter.DeleteCommand.Connection.Close();
                return 0;
            }
            return -1;
        }

        /// <summary>
        /// Delete record from database
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        public void DeleteFromDb(DbTransaction pTransaction)
        {
            DeleteFromDbBefore(pTransaction);
            if (!IsNew)
            {
                InternalDeleteComamnd(pTransaction);
                ResetFieldValue(this);
            }
            DeleteFromDbAfter(pTransaction);
        }

        /// <summary>
        /// Delete logical record from database
        /// </summary>
        /// <param name="pFieldName">Property field name</param>
        /// <param name="pValue">Set to Value</param>
        /// <param name="pTransaction">A specific transaction</param>
        public void DeleteLogicallyFromDb(String pFieldName, object pValue, DbTransaction pTransaction)
        {
            DeleteFromDbBefore(pTransaction);
            if (!IsNew)
            {
                SetMemberValue(pFieldName, pValue);
                this.SaveToDb(pTransaction);
            }
            DeleteFromDbAfter(pTransaction);
        }

        /// <summary>
        /// Loading record from database
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pIdentity">Identity value</param>
        public void Load(DbTransaction pTransaction, int pIdentity)
        {
            LoadBefore(pTransaction);
            SetMemberValue(FieldIdentityName, pIdentity);
            BuildSelectCommand();
            InternalSelectCommand(pTransaction);
            LoadAfter(pTransaction);
        }

        /// <summary>
        /// Loading record from database
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pParameters">List of parameters</param>
        public void Load(DbTransaction pTransaction, params RecordParameter[] pParameters)
        {
            LoadBefore(pTransaction);
            BuildSelectCommand(pParameters);
            InternalSelectCommand(pTransaction, pParameters);
            LoadAfter(pTransaction);
        }

        /// <summary>
        /// Loading record from database
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pConditions">List of conditions</param>
        public void Load(DbTransaction pTransaction, Condition pConditions)
        {
            LoadBefore(pTransaction);
            List<RecordParameter> list = new List<RecordParameter>();
            BuildSelectCommand(pTransaction, pConditions, list);
            InternalSelectCommand(pTransaction, list.ToArray());
            LoadAfter(pTransaction);
        }

        /// <summary>
        /// Reset record's members value
        /// </summary>
        public void Reset()
        {
            ResetBefore();
            ResetFieldValue(this);
            ResetAfter();
        }

        /// <summary>
        /// Save record to database
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        public void SaveToDb(DbTransaction pTransaction)
        {
            SaveToDbBefore(pTransaction);

            if (!IsNew && !HasValue)
                throw new RecordValidException("Record is not New and it's a value valid!");

            if (IsNew)
                InternalInsertCommand(pTransaction);
            else
                InternalUpdateCommand(pTransaction);

            SaveToDbAfter(pTransaction);
        }

        /// <summary>
        /// Build delete query
        /// </summary>
        /// <param name="pParams">List of parameters</param>
        internal void BuildDeleteCommand(DbTransaction transaction, params RecordParameter[] pParams)
        {
            if (RecordConnection == null)
                RecordConnection = CreateConnection(transaction, this.ChooseCurrentConnectionString());

            String DeleteFormat = "DELETE FROM {0} WHERE {1}";
            String Where = String.Empty;

            foreach (String memberName in Current[this.GetType()].m_Members)
            {
                MemberInfo member = GetType().GetMember(memberName)[0];
                object field = member.GetCustomAttributes(typeof(Field), true)[0];

                if (field is Field & (field as Field).IsPrimaryKey)
                {
                    if (!String.IsNullOrEmpty(Where))
                        Where += " AND ";

                    Where += String.Format("[{0}]={1}",
                            (field as Field).Name,
                             InternalConvert.ConvertQueryArgument(RecordConnection, (field as Field).Name));
                }
            }
            _DeleteCommandText = String.Format(DeleteFormat, TableName, Where);
        }

        /// <summary>
        /// Build insert query
        /// </summary>
        /// <param name="pParams">List of parameters</param>
        internal void BuildInsertCommand(DbTransaction transaction, params RecordParameter[] pParams)
        {
            if (RecordConnection == null)
                RecordConnection = CreateConnection(transaction, this.ChooseCurrentConnectionString());
            String InsertFormat = "INSERT INTO {0}({1}) VALUES ({2})";
            String Attrs = String.Empty;
            String Values = String.Empty;
            foreach (String memberName in Current[GetType()].m_Members)
            {
                MemberInfo member = GetType().GetMember(memberName)[0];
                object field = member.GetCustomAttributes(typeof(Field), true)[0];
                if (field is Field)
                {
                    if (!(field as Field).IsIdentity)
                    {
                        if (!String.IsNullOrEmpty(Attrs))
                            Attrs += ", ";
                        Attrs += String.Format("[{0}]", (field as Field).Name);

                        if (!String.IsNullOrEmpty(Values))
                            Values += ", ";
                        Values += String.Format("{0}",
                            InternalConvert.ConvertQueryArgument(RecordConnection, (field as Field).Name)
                        );
                    }
                }
            }
            _InsertCommandText = String.Format(InsertFormat, TableName, Attrs, Values);
        }

        /// <summary>
        /// Build update query
        /// </summary>
        /// <param name="pParams">List of parameters</param>
        internal void BuildUpdateCommand(DbTransaction transaction, params RecordParameter[] pParams)
        {
            if (RecordConnection == null)
                RecordConnection = CreateConnection(transaction, this.ChooseCurrentConnectionString());
            String UpdateFormat = "UPDATE {0} SET {1} WHERE {2}";
            String Sets = String.Empty;
            String Where = String.Empty;

            foreach (String memberName in Current[GetType()].m_Members)
            {
                MemberInfo member = GetType().GetMember(memberName)[0];
                object field = member.GetCustomAttributes(typeof(Field), true)[0];
                if (field is Field)
                {
                    if (!((field as Field).IsIdentity | (field as Field).IsPrimaryKey))
                    {
                        if (!String.IsNullOrEmpty(Sets))
                            Sets += ", ";
                        Sets += String.Format("[{0}]={1}",
                            (field as Field).Name,
                             InternalConvert.ConvertQueryArgument(RecordConnection, (field as Field).Name));
                    }

                    if ((field as Field).IsPrimaryKey)
                    {
                        if (!String.IsNullOrEmpty(Where))
                            Where += " AND ";
                        Where += String.Format("[{0}]={1}",
                            (field as Field).Name,
                             InternalConvert.ConvertQueryArgument(RecordConnection, (field as Field).Name));
                    }
                }
            }
            _UpdateCommandText = String.Format(UpdateFormat, TableName, Sets, Where);
        }

        /// <summary>
        /// Build a Where conditional for select query
        /// </summary>
        /// <param name="pParams">List of parameters</param>
        /// <returns>A string's format ([par1]=valu) AND ([par1] = value) </returns>
        internal String BuildWhere(params RecordParameter[] pParams)
        {
            String Where = String.Empty;
            if (pParams != null && pParams.Length > 0)
            {
                int pIndex = 0;
                foreach (RecordParameter param in pParams)
                {
                    param.ParameterField = param.ParameterName.Clone() as String;
                    param.ParameterName = String.Format("p{0}", pIndex);
                    param.OwnerParameter.ParameterName = String.Format("p{0}", pIndex);
                    if (!String.IsNullOrEmpty(Where))
                        Where += " AND ";
                    Where += CompareKindToString.SignWithFormat(RecordConnection, param, param.CompareKindExpression);
                    pIndex++;
                }
            }
            else
            {
                Where = "(1=1)";
            }
            return Where;
        }

        /// <summary>
        /// Build runtime member of instance
        /// </summary>
        //internal override void Initialize(DbTransaction pTransaction)
        //{
        //if (Current == null)
        //{
        //    Current = new RecordContext();
        //    Current.Add(GetType(), new MemberContext());
        //}
        //InitializeCustomAttributes(GetType());
        //InitializeMembers(GetType());
        ////BuildRecordColumns();
        //}

        /// <summary>
        /// After deleted record form database
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        protected virtual void DeleteFromDbAfter(DbTransaction pTransaction)
        {
        }

        /// <summary>
        /// Before deleting record from database
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        protected virtual void DeleteFromDbBefore(DbTransaction pTransaction)
        {
        }

        /// <summary>
        /// Fill member's value from reader
        /// </summary>
        /// <param name="reader"></param>
        protected void FillMemberFromReader(DbDataReader reader)
        {
            foreach (RecordColumn rc in this.Columns)
            {
                String memberName = Current[GetType()].m_ColumnNameToMemberName[rc.ColumnName];
                try
                {
                    this.SetMemberValue(memberName, reader[rc.ColumnName]);
                }
                catch (Exception e)
                {
                    throw new RecordException(memberName + ":" + e.Message);
                }
                rc.ColumnOriginValue = reader[rc.ColumnName];
            }

            foreach (String columnName in Current[GetType()].m_ColumnNames)
            {
            }
            this.IsNew = false;
            this.HasValue = true;
        }

        /// <summary>
        /// After loaded a record
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        protected virtual void LoadAfter(DbTransaction pTransaction)
        {
        }

        /// <summary>
        /// Before loading a record
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        protected virtual void LoadBefore(DbTransaction pTransaction)
        {
        }

        /// <summary>
        /// After reseted record's members value
        /// </summary>
        protected virtual void ResetAfter()
        {
        }

        /// <summary>
        /// Before resetting a record members value
        /// </summary>
        protected virtual void ResetBefore()
        {
        }

        /// <summary>
        /// After saveed record to database
        /// </summary>
        /// <param name="pTransaction"></param>
        protected virtual void SaveToDbAfter(DbTransaction pTransaction)
        {
        }

        /// <summary>
        /// Before saving record to database
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        protected virtual void SaveToDbBefore(DbTransaction pTransaction)
        {
        }

        /// <summary>
        /// Build select query
        /// </summary>
        /// <param name="pParams">List of parameters</param>
        private void BuildSelectCommand(params RecordParameter[] pParams)
        {
            if (RecordConnection == null)
                RecordConnection = CreateConnection(null, this.ChooseCurrentConnectionString());

            String SelectFormat = "SELECT {0} FROM {1} {2}";
            String Attrs = String.Empty;
            String Where = String.Empty;

            foreach (String filed in Current[GetType()].m_Members) //foreach (MemberInfo member in Current[this.GetType()].Attributes)
            {
                MemberInfo member = GetType().GetMember(filed)[0];
                object field = member.GetCustomAttributes(typeof(Field), true)[0];

                if (!String.IsNullOrEmpty(Attrs))
                    Attrs += ", ";
                Attrs += String.Format("[{0}]", (field as Field).Name);

                if (field is Field & (field as Field).IsPrimaryKey)
                {
                    if (!String.IsNullOrEmpty(Where))
                        Where += " AND ";
                    Where += String.Format("[{0}]={1}",
                        (field as Field).Name,
                        InternalConvert.ConvertQueryArgument(RecordConnection, (field as Field).Name)
                    );
                }
            }

            if (pParams != null && pParams.Length > 0)
                Where = String.Format("WHERE ({0})", BuildWhere(pParams));
            else
                Where = String.Format("WHERE ({0})", Where);

            _SelectCommandText = String.Format(SelectFormat, Attrs, TableName, Where);
        }

        private void BuildSelectCommand(DbTransaction transaction, Condition condition, List<RecordParameter> listOfParameters)
        {
            if (RecordConnection == null)
                RecordConnection = CreateConnection(transaction, this.ChooseCurrentConnectionString());

            String SelectFormat = "SELECT {0} FROM {1} {2}";
            String Attrs = String.Empty;
            String Where = String.Empty;

            foreach (String filed in Current[GetType()].m_Members) //foreach (MemberInfo member in Current[this.GetType()].Attributes)
            {
                MemberInfo member = GetType().GetMember(filed)[0];
                object field = member.GetCustomAttributes(typeof(Field), true)[0];

                if (!String.IsNullOrEmpty(Attrs))
                    Attrs += ", ";
                Attrs += String.Format("[{0}]", (field as Field).Name);

                if (field is Field & (field as Field).IsPrimaryKey)
                {
                    if (!String.IsNullOrEmpty(Where))
                        Where += " AND ";
                    Where += String.Format("[{0}]={1}",
                        (field as Field).Name,
                        InternalConvert.ConvertQueryArgument(RecordConnection, (field as Field).Name)
                    );
                }
            }

            if (condition != null)
                Where = String.Format("WHERE {0}", condition.Build(listOfParameters)).Replace("  ", " ");
            else
                Where = String.Format("WHERE ({0})", Where);

            _SelectCommandText = String.Format(SelectFormat, Attrs, TableName, Where);
        }

        /// <summary>
        /// Execute a Delete command
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pParams">List of parameters</param>
        private void InternalDeleteComamnd(DbTransaction pTransaction, params RecordParameter[] pParams)
        {
            BuildDeleteCommand(pTransaction, pParams);

            DbCommand DeleteCommand = CreateCommand(pTransaction);
            if (pTransaction != null)
                DeleteCommand.Transaction = pTransaction;
            DeleteCommand.CommandText = _DeleteCommandText;

            Debug.WriteLine(_DeleteCommandText);

            if (RecordConnection == null) RecordConnection = CreateConnection(pTransaction, ChooseCurrentConnectionString());
            DeleteCommand.Connection = pTransaction == null ? RecordConnection : pTransaction.Connection;

            if (String.IsNullOrEmpty(DeleteCommand.Connection.ConnectionString))
                DeleteCommand.Connection.ConnectionString = ChooseCurrentConnectionString();// String.IsNullOrEmpty(ConnectionString) ? ConnectionString : this.ConnectionString;

            foreach (String memberName in Current[GetType()].m_Members)
            {
                MemberInfo member = GetType().GetMember(memberName)[0];
                object field = member.GetCustomAttributes(typeof(Field), true)[0];

                if (field is Field & (field as Field).IsPrimaryKey)
                    DeleteCommand.Parameters.Add(((RecordParameter)CreateParameter((field as Field).Name, GetMemberValue(member.Name))).OwnerParameter);
            }

            OnDeleting(new RecordSourceCommandEventArgs(DeleteCommand));

            try
            {
                if (pTransaction == null)
                    DeleteCommand.Connection.Open();

                if (pTransaction != null)
                    DeleteCommand.Connection = pTransaction.Connection;

                int rs = DeleteCommand.ExecuteNonQuery();

                if (pTransaction == null)
                    DeleteCommand.Connection.Close();

                OnDeleted(new RecordSourceStatusEventArgs(DeleteCommand, 1, null));
            }
            catch (Exception e)
            {
                OnDeleted(new RecordSourceStatusEventArgs(null, 0, e));
                throw new RecordConnectionException(e.Message);
            }
            finally
            {
                if (pTransaction == null && DeleteCommand.Connection.State == ConnectionState.Open)
                    DeleteCommand.Connection.Close();
            }
        }

        /// <summary>
        /// Execute a Insert command
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pParams">List of parameters</param>
        private void InternalInsertCommand(DbTransaction pTransaction, params RecordParameter[] pParams)
        {
            BuildInsertCommand(pTransaction, pParams);

            if (!IsNew)
                return;

            DbCommand InsertCommand = CreateCommand(pTransaction);
            if (pTransaction != null)
                InsertCommand.Transaction = pTransaction;
            InsertCommand.CommandText = _InsertCommandText;

            System.Reflection.PropertyInfo p = PropertyIdentityName == null ? null : this.GetType().GetProperty(PropertyIdentityName);

            bool isguid = p != null && (p.PropertyType == typeof(Guid) | p.PropertyType == typeof(Guid?));

            if (p != null)
            {
                String outputInserted = " OUTPUT inserted." + FieldIdentityName + " VALUES ";
                InsertCommand.CommandText = InsertCommand.CommandText.Replace("VALUES", outputInserted);
            }
            Debug.WriteLine(_InsertCommandText);

            if (RecordConnection == null) RecordConnection = CreateConnection(pTransaction, ChooseCurrentConnectionString());
            InsertCommand.Connection = pTransaction == null ? RecordConnection : pTransaction.Connection;

            if (String.IsNullOrEmpty(InsertCommand.Connection.ConnectionString))
                InsertCommand.Connection.ConnectionString = ChooseCurrentConnectionString();// String.IsNullOrEmpty(ConnectionString) ? SharedConnectionString : this.ConnectionString;

            foreach (String memberName in Current[GetType()].m_Members)
            {
                MemberInfo member = GetType().GetMember(memberName)[0];
                object field = member.GetCustomAttributes(typeof(Field), true)[0];

                if (field is Field & !(field as Field).IsIdentity)
                    InsertCommand.Parameters.Add(((RecordParameter)CreateParameter((field as Field).Name, GetMemberValue(member.Name))).OwnerParameter);
            }

            OnInserting(new RecordSourceCommandEventArgs(InsertCommand));
            try
            {
                if (pTransaction == null)
                    InsertCommand.Connection.Open();

                object dr = InsertCommand.ExecuteScalar();
                //object dr = InsertCommand.ExecuteNonQuery();
                if (p != null)
                {
                    Type pt = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    if (dr != null)
                        dr = System.Convert.ChangeType(dr, pt);
                }
                if (dr != DBNull.Value && !String.IsNullOrEmpty(FieldIdentityName))
                    SetMemberValue(FieldIdentityName, dr);

                IsNew = false;
                HasValue = true;

                if (pTransaction == null)
                    InsertCommand.Connection.Close();

                OnInserted(new RecordSourceStatusEventArgs(InsertCommand, 1, null));
            }
            catch (Exception e)
            {
                OnInserted(new RecordSourceStatusEventArgs(null, 0, e));
                throw new RecordConnectionException(e.Message);
            }
            finally
            {
                if (pTransaction == null && InsertCommand.Connection.State == ConnectionState.Open)
                    InsertCommand.Connection.Close();
            }
        }

        /// <summary>
        /// Execute a Select command
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pParams">List of parameters</param>
        private void InternalSelectCommand(DbTransaction pTransaction, params RecordParameter[] pParams)
        {
            if (String.IsNullOrEmpty(_SelectCommandText))
                throw new RecordQueryNullException("Select Query not be empty or null value!");

            DbCommand SelectCommand = CreateCommand(pTransaction);
            if (pTransaction != null)
                SelectCommand.Transaction = pTransaction;
            SelectCommand.CommandText = _SelectCommandText;

            Debug.WriteLine(_SelectCommandText);

            if (RecordConnection == null) RecordConnection = CreateConnection(pTransaction, ChooseCurrentConnectionString());
            SelectCommand.Connection = pTransaction == null ? RecordConnection : pTransaction.Connection;

            if (String.IsNullOrEmpty(SelectCommand.Connection.ConnectionString))
                SelectCommand.Connection.ConnectionString = ChooseCurrentConnectionString();// String.IsNullOrEmpty(ConnectionString) ? SharedConnectionString : this.ConnectionString;

            if (pParams != null && pParams.Length > 0)
                foreach (RecordParameter par in pParams)
                    SelectCommand.Parameters.Add(par.OwnerParameter);
            else
            {
                foreach (String memberName in Current[GetType()].m_Members)
                {
                    MemberInfo member = GetType().GetMember(memberName)[0];
                    object field = member.GetCustomAttributes(typeof(Field), true)[0];
                    if (field is Field & (field as Field).IsPrimaryKey)
                        SelectCommand.Parameters.Add(((RecordParameter)CreateParameter((field as Field).Name, GetMemberValue(member.Name))).OwnerParameter);
                }
            }

            OnSelecting(new RecordSourceCommandEventArgs(SelectCommand));

            try
            {
                if (pTransaction == null)
                    if (SelectCommand.Connection.State != ConnectionState.Open)
                        SelectCommand.Connection.Open();

                DbDataReader reader = SelectCommand.ExecuteReader();

                if (reader.Read())
                {
                    this.FillMemberFromReader(reader);
                    // OnDataItemBound(new RecordSourceItemBoundEventArgs(this));
                    HasValue = true;
                }
                else
                {
                    HasValue = false;
                    IsNew = true;
                }

                OnSelected(new RecordSourceStatusEventArgs(SelectCommand, HasValue ? 1 : 0, null));
                //OnDataItemBound(new RecordSourceItemBoundEventArgs(this));

                if (!reader.IsClosed)
                    reader.Close();

                if (pTransaction == null)
                    SelectCommand.Connection.Close();
            }
            catch (Exception e)
            {
                OnSelected(new RecordSourceStatusEventArgs(null, 0, e));
                throw new RecordConnectionException(e.Message);
            }
            finally
            {
                if (pTransaction == null && SelectCommand.Connection.State == ConnectionState.Open)
                    SelectCommand.Connection.Close();
            }
        }

        /// <summary>
        /// Execute a Update command
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pParams">List of parameters</param>
        private void InternalUpdateCommand(DbTransaction pTransaction, params RecordParameter[] pParams)
        {
            BuildUpdateCommand(pTransaction, pParams);

            DbCommand UpdateCommand = CreateCommand(pTransaction);
            if (pTransaction != null)
                UpdateCommand.Transaction = pTransaction;
            UpdateCommand.CommandText = _UpdateCommandText;

            Debug.WriteLine(_UpdateCommandText);

            if (RecordConnection == null) RecordConnection = CreateConnection(pTransaction, ChooseCurrentConnectionString());
            UpdateCommand.Connection = pTransaction == null ? RecordConnection : pTransaction.Connection;

            if (String.IsNullOrEmpty(UpdateCommand.Connection.ConnectionString))
                UpdateCommand.Connection.ConnectionString = ChooseCurrentConnectionString();// String.IsNullOrEmpty(ConnectionString) ? SharedConnectionString : this.ConnectionString;

            foreach (String memberName in Current[GetType()].m_Members)
            {
                MemberInfo member = GetType().GetMember(memberName)[0];
                object field = member.GetCustomAttributes(typeof(Field), true)[0];

                String name = member.Name;
                object value = GetMemberValue(member.Name);

                if (field is Field && (field as Field).Type != FieldType.Identity)
                    UpdateCommand.Parameters.Add(((RecordParameter)CreateParameter((field as Field).Name, GetMemberValue(member.Name))).OwnerParameter);
            }

            OnUpdating(new RecordSourceCommandEventArgs(UpdateCommand));

            try
            {
                if (pTransaction == null)
                    UpdateCommand.Connection.Open();

                if (pTransaction != null)
                    UpdateCommand.Connection = pTransaction.Connection;

                int rs = UpdateCommand.ExecuteNonQuery();

                if (pTransaction == null)
                    UpdateCommand.Connection.Close();

                OnUpdated(new RecordSourceStatusEventArgs(UpdateCommand, 1, null));
            }
            catch (Exception e)
            {
                OnUpdated(new RecordSourceStatusEventArgs(UpdateCommand, 1, null));
                throw new RecordConnectionException(e.Message);
            }
            finally
            {
                if (pTransaction == null && UpdateCommand.Connection.State == ConnectionState.Open)
                    UpdateCommand.Connection.Close();
            }
        }

        /// <summary>
        /// Represent a directly query command on database.
        /// </summary>
        public static class DirectlyCommand
        {
            /// <summary>
            /// Execute a NonQuery
            /// </summary>
            /// <param name="query">Query</param>
            /// <param name="transaction">tranaction</param>
            /// <param name="parameters">List of parameters</param>
            /// <returns></returns>
            public static int ExecuteNonQuery(String query, DbConnection connection, DbTransaction transaction, params DbParameter[] parameters)
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Connection = connection;
                if (transaction != null)
                    command.Connection = transaction.Connection;
                command.Parameters.AddRange(parameters);
                int result = command.ExecuteNonQuery();
                return result;
            }

            /// <summary>
            /// Execute Reader query
            /// </summary>
            /// <param name="query">Query</param>
            /// <param name="transaction">Tranaction</param>
            /// <param name="parameters">List of parameters</param>
            /// <returns></returns>
            public static DbDataReader ExecuteReader(String query, DbConnection connection, DbTransaction transaction, params DbParameter[] parameters)
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Connection = connection;
                if (transaction != null)
                    command.Connection = transaction.Connection;
                command.Parameters.AddRange(parameters);
                DbDataReader result = command.ExecuteReader();
                return result;
            }

            /// <summary>
            /// Execute Reader query
            /// </summary>
            /// <param name="query">Query</param>
            /// <param name="commandBehaivor">Command Behavior</param>
            /// <param name="transaction">Tranaction</param>
            /// <param name="parameters">List of parameters</param>
            /// <returns></returns>
            public static DbDataReader ExecuteReader(String query, CommandBehavior commandBehaivor, DbConnection connection, DbTransaction transaction, params DbParameter[] parameters)
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Connection = connection;
                if (transaction != null)
                    command.Connection = transaction.Connection;
                command.Parameters.AddRange(parameters);
                DbDataReader result = command.ExecuteReader(commandBehaivor);
                return result;
            }

            /// <summary>
            /// Execute Scaler query
            /// </summary>
            /// <param name="query">Query</param>
            /// <param name="transaction">Transaction</param>
            /// <param name="parameters">List of parameters</param>
            /// <returns></returns>
            public static object ExecuteScalar(String query, DbConnection connection, DbTransaction transaction, params DbParameter[] parameters)
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.Connection = connection;
                if (transaction != null)
                    command.Connection = transaction.Connection;
                command.Parameters.AddRange(parameters);
                object result = command.ExecuteScalar();
                return result;
            }
        }

        /// <summary>
        /// Rapresent a store procedure command
        /// </summary>
        public static class StoreProcedureCommand
        {
            public static int ExecuteNonQueryStoreProcedure(String storeProcedureName, DbConnection connection, DbTransaction transaction, params DbParameter[] parameters)
            {
                int result = 0;
                DbCommand command = connection.CreateCommand();
                command.CommandText = storeProcedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = connection;
                if (transaction != null)
                    command.Connection = transaction.Connection;
                result = command.ExecuteNonQuery();
                return result;
            }

            public static DbDataReader ExecuteReaderStoreProcedure(String storeProcedureName, DbConnection connection, DbTransaction transaction, params DbParameter[] parameters)
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = storeProcedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = connection;
                if (transaction != null)
                    command.Connection = transaction.Connection;
                command.Parameters.AddRange(parameters);
                DbDataReader result = command.ExecuteReader();
                return result;
            }

            public static List<T> ExecuteReaderStoreProcedure<T>(String storeProcedureName, DbConnection connection, DbTransaction transaction, params DbParameter[] parameters)
               where T : Record, new()
            {
                List<T> list = new List<T>();
                DbDataReader reader = ExecuteReaderStoreProcedure(storeProcedureName, connection, transaction, parameters);
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        T item = new T();
                        item.FillMemberFromReader(reader);
                        list.Add(item);
                    }
                return list;
            }

            public static object ExecuteScalarStoreProcedure(String storeProcedureName, DbConnection connection, DbTransaction transaction, params DbParameter[] parameters)
            {
                DbCommand command = connection.CreateCommand();
                command.CommandText = storeProcedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = connection;
                if (transaction != null)
                    command.Connection = transaction.Connection;
                command.Parameters.AddRange(parameters);
                object result = command.ExecuteScalar();
                return result;
            }
        }
    }
}