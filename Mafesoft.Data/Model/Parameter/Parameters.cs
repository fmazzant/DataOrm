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

using Mafesoft.Data.Convert;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace Mafesoft.Data.Core.Parameter
{
    namespace Util
    {
        /// <summary>
        /// Represent a class for converts a CompareKind in to string
        /// </summary>
        public sealed class CompareKindToString
        {
            /// <summary>
            /// Return true if Sign is with parameter, false else
            /// </summary>
            /// <param name="CompareKind"></param>
            /// <returns></returns>
            public static Boolean IsSignWithAddingParameter(CompareKind CompareKind)
            {
                switch (CompareKind)
                {
                    case CompareKind.IsNULL:
                    case CompareKind.IsNotNULL:
                    case CompareKind.Like:
                    case CompareKind.In:
                    case CompareKind.IsNullOrEmpty:
                        return false;

                    case CompareKind.None:
                    case CompareKind.Equal:
                    case CompareKind.Major:
                    case CompareKind.NotEqual:
                    case CompareKind.Minor:
                    case CompareKind.MajorAndEqual:
                    case CompareKind.MinorAndEqual:
                    default:
                        return true;
                }
            }

            /// <summary>
            /// Converter from CompareKind to String
            /// </summary>
            /// <param name="pCompareKind">CompareKind</param>
            /// <returns>CompareKind converted into string</returns>
            [Obsolete("Sign will be obsolete method", false)]
            public static String Sign(CompareKind pCompareKind)
            {
                switch (pCompareKind)
                {
                    case CompareKind.None: return " = ";
                    case CompareKind.Equal: return " = "; ;
                    case CompareKind.IsNULL: return " IS NULL ";
                    case CompareKind.Major: return " > ";
                    case CompareKind.Minor: return " < ";
                    case CompareKind.IsNotNULL: return "IS NOT NULL";
                    case CompareKind.NotEqual: return " <> ";
                    case CompareKind.Like: return " LIKE ";
                    case CompareKind.In: return " IN ";
                    case CompareKind.MajorAndEqual: return " >= ";
                    case CompareKind.MinorAndEqual: return " <= ";
                    default: return " = ";
                }
            }

            /// <summary>
            ///  Converter from CompareKind to String
            /// </summary>
            /// <param name="parameter">Parameter</param>
            /// <param name="CompareKind">Compare Type</param>
            /// <returns>CompareKind converted into string with format</returns>
            public static String SignWithFormat(RecordParameter parameter, CompareKind CompareKind)
            {
                return SignWithFormat(null, parameter, CompareKind);
            }

            /// <summary>
            ///  Converter from CompareKind to String
            /// </summary>
            /// <param name="connection">Connection</param>
            /// <param name="parameter">Parameter</param>
            /// <param name="CompareKind">Compare Type</param>
            /// <returns>CompareKind converted into string with format</returns>
            public static String SignWithFormat(DbConnection connection, RecordParameter parameter, CompareKind CompareKind)
            {
                //if (connection == null)
                //    connection = Record.SharedProviderFactory.CreateConnection();
                String pName = InternalConvert.ConvertQueryArgument(connection, parameter.ParameterName);
                switch (CompareKind)
                {
                    case CompareKind.None:
                        return String.Format(" ([{0}] = {1}) ", parameter.ParameterField, pName);

                    case CompareKind.Equal:
                        return String.Format(" ([{0}] = {1}) ", parameter.ParameterField, pName);

                    case CompareKind.IsNULL:
                        return String.Format(" ([{0}] IS NULL) ", parameter.ParameterField);

                    case CompareKind.Major:
                        return String.Format(" ([{0}] > {1}) ", parameter.ParameterField, pName);

                    case CompareKind.Minor:
                        return String.Format(" ([{0}] < {1}) ", parameter.ParameterField, pName);

                    case CompareKind.IsNotNULL:
                        return String.Format(" ([{0}] IS NOT NULL) ", parameter.ParameterField);

                    case CompareKind.NotEqual:
                        return String.Format(" ([{0}] <> {1}) ", parameter.ParameterField, pName);

                    case CompareKind.Like:
                        return String.Format(" ([{0}] LIKE '%{1}%') ", parameter.ParameterField, parameter.Value);

                    case CompareKind.In:
                        if (parameter.Value is IList)
                        {
                            String listFieldValue = "";
                            foreach (var item in (IList)parameter.Value)
                                listFieldValue += "," + item;
                            return String.Format(" ([{0}] IN ({1})) ", parameter.ParameterField, listFieldValue.Substring(1));
                        }
                        else
                        {
                            return String.Format(" ([{0}] IN ({1})) ", parameter.ParameterField, parameter.Value);
                        }
                    case CompareKind.MajorAndEqual:
                        return String.Format(" ([{0}] >= {1}) ", parameter.ParameterField, pName);

                    case CompareKind.MinorAndEqual:
                        return String.Format(" ([{0}] <= {1}) ", parameter.ParameterField, pName);

                    case CompareKind.IsNullOrEmpty:
                        return String.Format(" (([{0}] IS NULL) OR ([{0}] = '')) ", parameter.ParameterField);

                    default:
                        return String.Format(" ([{0}] = {1}) ", parameter.ParameterField, pName);
                }
            }
        }
    }

    /// <summary>
    /// Represents a record db parameter.
    /// </summary>
    public class RecordDbParameter : DbParameter, IDbDataParameter, IDataParameter
    {
        private Boolean _CanBeAdded = true;
        private CompareKind _CompareKind = CompareKind.None;
        private DbType _DbType;
        private ParameterDirection _Direction;
        private bool _IsNullable;
        private Condition _OwnerCondition = null;
        private string _ParameterName = null;
        private String _ParamterField = null;
        private int _Size;
        private string _SourceColumn = null;
        private bool _SourceColumnNullMapping;
        private DataRowVersion _SourceVersion;
        private object _Value = null;

        protected RecordDbParameter()
            : base()
        {
        }

        protected RecordDbParameter(DbParameter pParameter)
            : base()
        {
            InitFromOtherParameterType(pParameter);
        }

        public Boolean CanBeAdded { get { return _CanBeAdded; } set { _CanBeAdded = value; } }

        public CompareKind CompareKindExpression { get { return _CompareKind; } set { _CompareKind = value; } }

        public override DbType DbType { get { return _DbType; } set { _DbType = value; } }

        public override ParameterDirection Direction { get { return _Direction; } set { _Direction = value; } }

        public override bool IsNullable { get { return _IsNullable; } set { _IsNullable = value; } }

        public String ParameterField { get { return _ParamterField; } set { _ParamterField = value; } }

        public override string ParameterName { get { return _ParameterName; } set { _ParameterName = value; } }

        public override int Size { get { return _Size; } set { _Size = value; } }

        public override string SourceColumn { get { return _SourceColumn; } set { _SourceColumn = value; } }

        public override bool SourceColumnNullMapping { get { return _SourceColumnNullMapping; } set { _SourceColumnNullMapping = value; } }

        public override DataRowVersion SourceVersion { get { return _SourceVersion; } set { _SourceVersion = value; } }

        public override object Value { get { return _Value; } set { _Value = value; } }

        internal Condition OwnerCondition { get { return _OwnerCondition; } set { _OwnerCondition = value; } }

        public override void ResetDbType()
        {
            _DbType = DbType.Object;
        }

        protected void InitFromOtherParameterType(DbParameter pParameter)
        {
            DbType = pParameter.DbType;
            Direction = pParameter.Direction;
            ParameterName = pParameter.ParameterName;
            Size = pParameter.Size;
            SourceColumn = pParameter.SourceColumn;
            SourceColumnNullMapping = pParameter.SourceColumnNullMapping;
            SourceVersion = pParameter.SourceVersion;
            Value = pParameter.Value;
        }
    }

    /// <summary>
    /// Represents a single parameter during instance of Record is created.
    /// </summary>
    public class RecordInstanceParameter
    {
        private String _ProprertyName = null;
        private Object _Value = null;

        /// <summary>
        /// Create a new parameter instance
        /// </summary>
        /// <param name="pPropertyName"></param>
        /// <param name="pValue"></param>
        public RecordInstanceParameter(String pPropertyName, Object pValue)
        {
            _ProprertyName = pPropertyName;
            _Value = pValue;
        }

        /// <summary>
        /// A property's name.
        /// </summary>
        public String PropertyName { get { return _ProprertyName; } }

        /// <summary>
        /// property's value.
        /// </summary>
        public Object Value { get { return _Value; } }
    }

    /// <summary>
    /// Represents a RecordParameter
    /// </summary>
    public class RecordParameter : RecordDbParameter
    {
        protected DbParameter _OwnerParameter = null;

        /// <summary>
        /// Create a new instance of RecordParameter
        /// </summary>
        /// <param name="pParameter">Owner parameter</param>
        public RecordParameter(DbParameter pParameter)
            : base(pParameter)
        {
            _OwnerParameter = pParameter;
            CompareKindExpression = CompareKind.Equal;
        }

        /// <summary>
        /// Create a new instance of record parameter
        /// </summary>
        /// <param name="pParameter">Owner parameter</param>
        /// <param name="pCompareKind">Type of compare for this parameter</param>
        public RecordParameter(DbParameter pParameter, CompareKind pCompareKind)
            : base(pParameter)
        {
            _OwnerParameter = pParameter;
            CompareKindExpression = pCompareKind;
        }

        /// <summary>
        /// Create a new instance of RecordParameter
        /// </summary>
        protected RecordParameter()
            : base()
        {
            _OwnerParameter = null;
            CompareKindExpression = CompareKind.None;
        }

        /// <summary>
        /// Owner parameter created by ProviderFactory instance.
        /// </summary>
        public DbParameter OwnerParameter
        {
            get { return _OwnerParameter; }
        }
    }

    /// <summary>
    /// Represents a RecordParameter
    /// </summary>
    /// <typeparam name="TRECORD">Record type on creating record parameter</typeparam>
    public sealed class RecordParameter<TRECORD> : RecordParameter
        where TRECORD : Record, new()
    {
        /// <summary>
        /// Create a new instance of record parameter
        /// </summary>
        /// <param name="pParameterName">Parameter's name</param>
        /// <param name="pValue">Parameter's value</param>
        public RecordParameter(String pParameterName, Object pValue)
            : base(Record.CreateParameter<TRECORD>(pParameterName, pValue))
        {
        }

        /// <summary>
        /// Create a new instance of record parameter
        /// </summary>
        /// <param name="pParameterName">Parameter's name</param>
        /// <param name="pCompareKind">Parameter's compare type</param>
        /// <param name="pValue">Parameter's value</param>
        public RecordParameter(String pParameterName, CompareKind pCompareKind, Object pValue)
            : base(Record.CreateParameter<TRECORD>(pParameterName, pValue), pCompareKind)
        {
        }
    }
}