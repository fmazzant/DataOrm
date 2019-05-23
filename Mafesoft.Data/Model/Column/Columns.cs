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

using Mafesoft.Data.Core.Attribute;
using System;

namespace Mafesoft.Data.Core.Column
{
    /// <summary>
    /// Represents a record's column instance
    /// </summary>
    public class RecordColumn
    {
        private Field _ColumnField = null;
        private String _MemeberName = null;
        private String _ColumnName = null;
        private Type _ColumnType = null;
        private Int32 _Index = 0;
        private Object _OriginValue;
        private RecordMember _Record = null;
        private Int32 _Size = 0;

        /// <summary>
        ///  Create a new instance of Column
        /// </summary>
        public RecordColumn()
        {
            _ColumnName = String.Empty;
            _ColumnType = typeof(Object);
        }

        /// <summary>
        /// Create a new instance of Column
        /// </summary>
        /// <param name="pColumnName">Column's name</param>
        public RecordColumn(String pColumnName, String pMemberName)
        {
            _ColumnName = pColumnName;
            _MemeberName = pMemberName;
            _ColumnType = typeof(Object);
        }

        /// <summary>
        /// Create a new instance of column
        /// </summary>
        /// <param name="pColumnName">Column's name</param>
        /// <param name="pColumnType">Column value's type</param>
        public RecordColumn(String pColumnName, String pMemberName, Type pColumnType)
            : this(pColumnName, pMemberName)
        {
            _ColumnType = pColumnType;
        }

        /// <summary>
        /// Create a new instance of column
        /// </summary>
        /// <param name="pColumnName">Column's name</param>
        /// <param name="pColumnType">Column value's type</param>
        /// <param name="pColumnField">Column's field</param>
        /// <param name="pSize">Column's size</param>
        /// <param name="pRecord">Record contains this column</param>
        public RecordColumn(String pColumnName, String pMemberName, Type pColumnType, Field pColumnField, RecordMember pRecord)
            : this(pColumnName, pMemberName, pColumnType)
        {
            _ColumnField = pColumnField;
            _Size = pColumnField.Size;
            _Record = pRecord;
            _Index = pColumnField.Index != -1 ? pColumnField.Index : pRecord.Columns.Count;
        }

        /// <summary>
        /// Column's field
        /// </summary>
        public Field ColumnField { get { return _ColumnField; } }

        /// <summary>
        /// Column's index
        /// </summary>
        public Int32 ColumnIndex { get { return _Index; } }

        /// <summary>
        /// Column's name
        /// </summary>
        public String ColumnName { get { return _ColumnName; } }

        /// <summary>
        /// Property's name
        /// </summary>
        public String MemberName { get { return _MemeberName; } }

        /// <summary>
        /// Column's origin value.
        /// </summary>
        public object ColumnOriginValue
        {
            get { return _OriginValue; }
            internal set { _OriginValue = value; }
        }

        /// <summary>
        /// Column's size
        /// </summary>
        public Int32 ColumnSize { get { return _Size; } }

        /// <summary>
        /// Column value's type
        /// </summary>
        public Type ColumnType { get { return _ColumnType; } }

        /// <summary>
        /// Column's value, returns null when record is not initialized else returns a column's value
        /// </summary>
        public object ColumnValue
        {
            get
            {
                if (Record == null)
                    return null;
                return Record[ColumnName];
            }
            set
            {
                Record[ColumnName] = value;
            }
        }

        /// <summary>
        /// Column's changed value after loaded
        /// </summary>
        public Boolean IsChanged
        {
            get { return ColumnOriginValue != ColumnValue; }
        }

        /// <summary>
        /// Retrurns true when this column is readonly.
        /// </summary>
        public Boolean ReadOnly { get { return _Record == null; } }

        /// <summary>
        /// Record contains this column
        /// </summary>
        internal RecordMember Record
        {
            get { return _Record; }
            set { _Record = value; }
        }

        /// <summary>
        /// Returns true when objs are equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return ColumnName == ((RecordColumn)obj).ColumnName;
        }

        /// <summary>
        /// Returns HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents this.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}={1}", ColumnName, ColumnValue);
        }
    }
}