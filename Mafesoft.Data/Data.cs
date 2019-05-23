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
    using Mafesoft.Data.Core;
    using Mafesoft.Data.Core.Column;
    using Mafesoft.Data.Core.Parameter;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Common;

    namespace Core.Collection.Generic
    {
        /// <summary>
        /// Supports Value, whitch creates a new instance of a class as value represents a list value of type T record instance
        /// </summary>
        /// <typeparam name="T">Record type</typeparam>
        public interface IRecords<T> : IRecordHasValue
            where T : Record, new()
        {
            IList<RecordView<T>> Values { get; }
        }

        /// <summary>
        /// Represents a list of TRECORD type.
        /// </summary>
        /// <typeparam name="TRECORD"></typeparam>
        [DataObject(true)]
        public sealed class Records<TRECORD> : IRecords<TRECORD>
            where TRECORD : Record, new()
        {
            private ListRecord<TRECORD> _list = null;

            /// <summary>
            /// Creates a new list Record instance. Providers to NOT load a specifics records from database.
            /// </summary>
            public Records()
            {
                _list = ListRecord<TRECORD>.CreateNewInstance();
                _list.Load(null);
            }

            /// <summary>
            ///  Creates a new list Record instance. Providers to load a specifics records from database.
            /// </summary>
            /// <param name="pTransaction">A specifics transaction</param>
            /// <param name="pParams">Parameters list. This list represents a WHERE on load from db.</param>
            public Records(DbTransaction pTransaction, params RecordParameter[] pParameters)
            {
                _list = ListRecord<TRECORD>.CreateNewInstance(pTransaction, pParameters);
                _list.Load(pTransaction, pParameters);
            }

            /// <summary>
            /// Creates a new list Record instance. Providers to load a specifics records from database.
            /// </summary>
            /// <param name="pTransaction">A specifics transaction</param>
            /// <param name="pCondition">Condition, This represents a WHERE on load from db.</param>
            public Records(DbTransaction pTransaction, Condition pCondition)
            {
                _list = ListRecord<TRECORD>.CreateNewInstance(pTransaction, pCondition);
                _list.Load(pTransaction, pCondition);
            }

            /// <summary>
            /// Creates a new list Record instance. Providers to load a specifics records from database.
            /// </summary>
            /// <param name="pTransaction">A specifics transaction</param>
            /// <param name="pSelectQuery">Select Command Text</param>
            /// <param name="pParameters">arameters list. This list represents a WHERE on load from db.</param>
            public Records(DbTransaction pTransaction, String pSelectQuery, params RecordParameter[] pParameters)
            {
                _list = ListRecord<TRECORD>.CreateNewInstance(pTransaction, pSelectQuery, pParameters);
                _list.Load(pTransaction, pSelectQuery, pParameters);
            }

            /// <summary>
            /// when is true exsits at least a record.
            /// </summary>
            public Boolean HasValue
            {
                get
                {
                    return false;// _list.Items.Count > 0;
                }
            }

            /// <summary>
            /// List of record. Its can be null value.
            /// </summary>
            public IList<RecordView<TRECORD>> Values
            {
                get
                {
                    //if (HasValue)
                    //    return _list.Items;
                    return new List<RecordView<TRECORD>>();
                }
            }

            /// <summary>
            /// Saves a list of record to fisics database.
            /// </summary>
            /// <param name="pTransaction">A specifics transaction</param>
            public void SaveToDb(DbTransaction pTransaction)
            {
                _list.SaveToDb(pTransaction);
            }
        }
    }

    /// <summary>
    /// Field Parameter compare with a value
    /// </summary>
    public enum CompareKind
    {
        None,
        Major,
        Minor,
        Equal,
        NotEqual,
        IsNotNULL,
        IsNULL,
        Like,
        In,
        MajorAndEqual,
        MinorAndEqual,
        IsNullOrEmpty
    }

    /// <summary>
    /// Field's type can be uses.
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// Field is a primary key for this table
        /// </summary>
        PrimaryKey,

        /// <summary>
        /// Field is a external key for this table.
        /// </summary>
        ExternalKey,

        /// <summary>
        /// Field is identity for this table
        /// </summary>
        Identity,

        /// <summary>
        /// Field is a primary key and identity for this table.
        /// </summary>
        PrimaryKeyIdentity,

        /// <summary>
        /// Is normal field for this table
        /// </summary>
        Normal
    }

    /// <summary>
    /// Providers support on this framework
    /// </summary>
    public enum ProviderFactorySupport
    {
        /// <summary>
        /// Data Provider
        /// .Net Framework Data Provider for None
        /// <value>None</value>
        /// </summary>
        None,

        /// <summary>
        /// Odbc Data Provider
        /// .Net Framework Data Provider for Odbc
        /// <value>System.Data.Odbc</value>
        /// </summary>
        Odbc,

        /// <summary>
        /// OleDb Data Provider
        /// .Net Framework Data Provider for OleDb
        /// <value>System.Data.OleDb</value>
        /// </summary>
        OleDb,

        /// <summary>
        /// OracleClient Data Provider
        /// .Net Framework Data Provider for Oracle
        /// <value>System.Data.OracleClient</value>
        /// </summary>
        OracleClient,

        /// <summary>
        /// SqlClient Data Provider
        /// .Net Framework Data Provider for SqlServer
        /// <value>System.Data.SqlClient</value>
        /// </summary>
        SqlClient,

        /// <summary>
        /// Microsoft SQL Server Compact Data Provider
        /// .NET Framework Data Provider for Microsoft SQL Server Compact
        /// <value>System.Data.SqlServerCe.3.5</value>
        /// </summary>
        SqlServerCe
    }

    /// <summary>
    /// Supports Value, whitch creates a new instance of a class as value represents a single value of type T record instance
    /// </summary>
    /// <typeparam name="T">Record type</typeparam>
    public interface IRecord<T> : IRecordHasValue
        where T : Record, new()
    {
        T Value { get; }
    }

    /// <summary>
    /// Supports column, whitch creates a new instance of a class as value represents a single value of column T record instance
    /// </summary>
    public interface IRecord
    {
        object this[String columnName] { get; }

        object this[int columnIndex] { get; }

        object this[RecordColumn column] { get; }
    }

    /// <summary>
    ///  Supports column and values, whitch creates a new instance of a class as value represents a list value T recordview instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRecordView<T> : IRecord
    {
        String[] ItemColumns { get; }

        object[] ItemArray { get; }
    }

    /// <summary>
    /// Supports HasValue, whitch creates a new instance of a class as value represents a valid value of this instance.
    /// </summary>
    public interface IRecordHasValue
    {
        Boolean HasValue { get; }
    }

    /// <summary>
    /// Represents a single TRECORD type.
    /// </summary>
    /// <typeparam name="TRECORD">Record type</typeparam>
    public sealed class Record<TRECORD> : IRecord<TRECORD>
         where TRECORD : Record, new()
    {
        private TRECORD _Value = null;

        /// <summary>
        /// Creates a new Record instance. Providers to NOT load a specifics record from database.
        /// </summary>
        public Record()
        {
            _Value = Record.CreateNewInstance<TRECORD>();
        }

        /// <summary>
        /// Creates a new Record instance. Providers to load a specifics record from database.
        /// </summary>
        /// <param name="pKey">Primary Key value</param>
        public Record(int pIdentity)
        {
            _Value = Record.CreateNewInstance<TRECORD>();
            _Value.Load(null, pIdentity);
            if (!_Value.HasValue)
                _Value = null;
        }

        /// <summary>
        /// Creates a new Record instance. Providers to load a specifics record from database.
        /// </summary>
        /// <param name="pTransaction">A specifics transaction</param>
        /// <param name="pKey">Primary Key value</param>
        public Record(DbTransaction pTransaction, int pIdentity)
        {
            _Value = Record.CreateNewInstance<TRECORD>();
            _Value.Load(pTransaction, pIdentity);
            if (!_Value.HasValue)
                _Value = null;
        }

        /// <summary>
        /// Creates a new Record instance. Providers to load a specifics record from database.
        /// </summary>
        /// <param name="pTransaction">A specifics transaction</param>
        /// <param name="pParams">Parameters list. This list represents a WHERE on load from db.</param>
        public Record(DbTransaction pTransaction, params RecordParameter[] pParams)
        {
            _Value = Record.CreateNewInstance<TRECORD>();
            _Value.Load(pTransaction, pParams);
            if (!_Value.HasValue)
                _Value = null;
        }

        /// <summary>
        /// Creates a new Record instance. Providers to load a specifics record from database.
        /// </summary>
        /// <param name="pTransaction">A specifics transaction</param>
        /// <param name="pCondition">Conditions. This list represents a WHERE on load from db.</param>
        public Record(DbTransaction pTransaction, Condition pCondition)
        {
            _Value = Record.CreateNewInstance<TRECORD>();
            _Value.Load(pTransaction, pCondition);
            if (!_Value.HasValue)
                _Value = null;
        }

        /// <summary>
        /// when is true the record has a valid value.
        /// </summary>
        public Boolean HasValue
        {
            get
            {
                return _Value != null;
            }
        }

        /// <summary>
        /// The value of record. Its can be null value.
        /// </summary>
        public TRECORD Value
        {
            get { return _Value; }
        }
    }
}