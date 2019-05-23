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
    using Mafesoft.Data.Core.Collection;
    using Mafesoft.Data.Core.Collection.Common;
    using Mafesoft.Data.Core.Column;
    using Mafesoft.Data.Core.Common;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Reflection;

    public static class Mapper<T>
        // We can only use reference types
    where T : Record, new()
    {
        static Mapper()
        {
            if (Record.Current[typeof(T)].m_propertyMap == null)
            {
                Record.Current[typeof(T)].m_propertyMap = new Dictionary<string, PropertyInfo>();
                foreach (PropertyInfo rc in typeof(T).GetProperties())
                    if (!Record.Current[typeof(T)].m_propertyMap.ContainsKey(rc.Name))
                        Record.Current[typeof(T)].m_propertyMap.Add(rc.Name, rc);
            }
        }

        public static void Map(RecordView<T> source, T destination)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (destination == null)
                throw new ArgumentNullException("destination");

            foreach (var kv in source.ItemColumns)
            {
                PropertyInfo p;
                string name = Record.Current[typeof(T)].m_ColumnNameToMemberName[kv.Trim()];
                if (Record.Current[typeof(T)].m_propertyMap.TryGetValue(name, out p))
                {
                    var propType = p.PropertyType;
                    if (kv == null)
                    {
                        if (!propType.IsByRef && propType.Name != "Nullable`1")
                        {
                            throw new ArgumentException("not nullable");
                        }
                    }
                    if (source[kv] == DBNull.Value)
                    {
                        destination[name] = null;
                        continue;
                    }
                    p.SetValue(destination, source[kv], null);
                    //destination[name] = source[kv];
                }
            }
        }
    }

    /// <summary>
    /// Rappresents a abstract RecordView entity.
    /// </summary>
    public abstract class RecordView : RecordBase
    {
        protected RecordView()
            : base(null)
        {
        }
    }

    /// <summary>
    /// Represents a generic RecordView with T generic object definition.
    /// </summary>
    public class RecordView<T> : RecordView, IRecordView<T>
        where T : Record, new()
    {
        private int index = -1;
        private bool IsNew = true;
        private bool HasValue = false;
        private List<String> Columns = new List<String>();
        private object[] _ItemArray = null;
        private ListRecord<T> _Parent;

        /// <summary>
        /// REturns a ListRecord to contains the RecordView
        /// </summary>
        public ListRecord<T> ListRecord
        {
            get { return _Parent; }
        }

        protected RecordView() :
            base()
        {
        }

        /// <summary>
        /// Initialize RecordView
        /// </summary>
        /// <param name="pTransaction">Transaction</param>
        internal override void Initialize(DbTransaction pTransaction)
        {
        }

        /// <summary>
        /// Implicit cast into T object definition
        /// </summary>
        /// <param name="recordView">RecordView object</param>
        /// <returns>T object definition</returns>
        public static implicit operator T(RecordView<T> recordView)
        {
            T record = new T();
            record.IsNew = recordView.IsNew;
            record.HasValue = recordView.HasValue;
            record.ListRecordParent = recordView._Parent as ListRecord;

            Mapper<T>.Map(recordView, record);

            return record;
        }

        /// <summary>
        /// Explicit cast into RecotdView
        /// </summary>
        /// <param name="record">T object</param>
        /// <returns>RecordView</returns>
        public static explicit operator RecordView<T>(T record)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// List of Columns
        /// </summary>
        public String[] ItemColumns
        {
            get { return Columns.ToArray(); }// _Parent.InternalQueryColumnsList.ToArray(); }
        }

        /// <summary>
        /// List of Values.
        /// </summary>
        public object[] ItemArray
        {
            get { return _ItemArray; }
            internal set
            {
                _ItemArray = value;
            }
        }

        /// <summary>
        /// Column's value with column's name
        /// </summary>
        /// <param name="columnName">Column's name</param>
        /// <returns>value</returns>
        public object this[String columnName]
        {
            get
            {
                return _ItemArray[Columns.IndexOf(columnName)];//_ItemArray[_Parent.InternalQueryColumnsList.IndexOf(columnName)];
            }
        }

        /// <summary>
        /// Column's value with column's index
        /// </summary>
        /// <param name="columnIndex">column's index</param>
        /// <returns>value</returns>
        public object this[int columnIndex]
        {
            get
            {
                return ItemColumns[columnIndex];
            }
        }

        /// <summary>
        /// Column's value with RecordColumn
        /// </summary>
        /// <param name="column">column</param>
        /// <returns>value</returns>
        public object this[RecordColumn column]
        {
            get
            {
                return _ItemArray[_Parent.InternalQueryColumnsList.IndexOf(column.ColumnName)];
            }
        }

        /// <summary>
        /// Create a enumerable RecordView list
        /// </summary>
        /// <typeparam name="TRECORD"></typeparam>
        /// <param name="pReader">Data reader</param>
        /// <param name="columns">List of columns</param>
        /// <returns>Enumerable RecordView instances</returns>
        internal static IEnumerable<RecordView<TRECORD>> GetRecordViewItems<TRECORD>(DbDataReader pReader, ListRecord<TRECORD> listRecord, List<String> columns)
           where TRECORD : Record, new()
        {
            while (pReader.Read())
            {
                RecordView<TRECORD> recordView = new RecordView<TRECORD>();
                recordView.IsNew = false;
                recordView.HasValue = true;
                recordView.Columns = columns;
                recordView.ItemArray = new object[pReader.FieldCount];
                recordView.index = pReader.GetValues(recordView.ItemArray);

                yield return recordView;
            }
            yield break;
        }

        internal static IEnumerable<RecordView<TRECORD>> GetRecordViewItems<TRECORD>(DbDataReader pReader, ListRecord<TRECORD> listRecord)
           where TRECORD : Record, new()
        {
            while (pReader.Read())
            {
                RecordView<TRECORD> recordView = new RecordView<TRECORD>();
                recordView.IsNew = false;
                recordView.HasValue = true;
                recordView.ItemArray = new object[pReader.FieldCount];
                recordView.index = pReader.GetValues(recordView.ItemArray);
                recordView._Parent = listRecord;
                yield return recordView;
            }
            yield break;
        }
    }
}