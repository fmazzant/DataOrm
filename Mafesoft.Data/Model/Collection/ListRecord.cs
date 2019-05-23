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
using Mafesoft.Data.Core.Attribute;
using Mafesoft.Data.Core.Collection.Common;
using Mafesoft.Data.Core.Column;
using Mafesoft.Data.Core.Common;
using Mafesoft.Data.Core.Parameter;
using Mafesoft.Data.Core.Parameter.Util;
using Mafesoft.Data.Model.Parameter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;

namespace Mafesoft.Data.Core.Collection
{
    namespace Common
    {
        /// <summary>
        /// Represents a list of record
        /// </summary>
        public abstract class ListRecord : RecordMember
        {
            internal List<String> InternalQueryColumnsList = new List<String>();

            /// <summary>
            /// Providers to load a specifics records from database
            /// </summary>
            /// <param name="pTransaction"></param>
            protected ListRecord(DbTransaction pTransaction)
                : base(pTransaction)
            {
            }
        }

        /// <summary>
        /// Represents a list record base
        /// </summary>
        /// <typeparam name="TRECORD">Record type</typeparam>
        public abstract class ListRecordBase<TRECORD> : ListRecord
           where TRECORD : Record, new()
        {
            /// <summary>
            /// Providers to load a specifics records from database.
            /// </summary>
            /// <param name="pTransaction">A specific transaction</param>
            protected ListRecordBase(DbTransaction pTransaction)
                : base(pTransaction)
            {
            }

            /// <summary>
            /// Property Get, list of record of TRECORD type
            /// </summary>
            //public abstract IList<RecordView<TRECORD>> Items { get; }
            public abstract IEnumerable<TRECORD> Items { get; }

            //public abstract IEnumerable<TRECORD> GetItems();
        }
    }

    /// <summary>
    /// Represents a list record
    /// </summary>
    /// <typeparam name="TRECORD">Record type</typeparam>
    [FieldTable(null)]
    public class ListRecord<TRECORD> : ListRecordBase<TRECORD>, IListSource
        where TRECORD : Record, new()
    {
        protected List<RecordColumn> _Columns = new List<RecordColumn>();
        protected List<RecordView<TRECORD>> _Items = new List<RecordView<TRECORD>>();
        protected String _SelectCommandText = String.Empty;

        public override string ConnectionString
        {
            get { return new TRECORD().ConnectionString; }
        }

        public override DbProviderFactory ProviderFactory
        {
            get { return new TRECORD().ProviderFactory; }
        }

        /// <summary>
        /// Creates a new List Record instance. Providers to NOT load a specifics records from database.
        /// </summary>
        public ListRecord()
            : base(null)
        {
        }

        /// <summary>
        /// Creates a new List Record instance. Providers to NOT load a specifics records from database.
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pParameters">List of parameters</param>
        protected ListRecord(DbTransaction pTransaction, params RecordParameter[] pParameters)
            : base(pTransaction)
        {
        }

        /// <summary>
        /// reates a new List Record instance. Providers to NOT load a specifics records from database.
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pCondition">Conditions</param>
        /// <param name="pOrderBy">OrderBy fields</param>
        protected ListRecord(DbTransaction pTransaction, Condition pCondition, params RecordOrderBy[] pOrderBy)
            : base(pTransaction)
        {
        }

        /// <summary>
        /// Creates a new List Record instance. Providers to NOT load a specifics records from database.
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pSelectQuery">A specific select query</param>
        /// <param name="pParameters">List of parameters</param>
        protected ListRecord(DbTransaction pTransaction, String pSelectQuery, params RecordParameter[] pParameters)
            : base(pTransaction)
        {
        }

        /// <summary>
        /// Property Get, return number of Items.
        /// </summary>
        private int _count;

        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Property Get return i-esimo element
        /// </summary>
        /// <param name="index">Index of</param>
        /// <returns>i-esimo element</returns>
        public new TRECORD this[int index]
        {
            get
            {
                return (TRECORD)_Items[index];
            }
        }

        /// <summary>
        /// Property Get, list of record of TRECORD type
        /// </summary>
        public override IEnumerable<TRECORD> Items
        {
            get
            {
                foreach (RecordView<TRECORD> item in _Items)
                    yield return (TRECORD)item;
                yield break;
            }
        }

        //public override IEnumerable<TRECORD> GetItems()
        //{
        //    foreach (RecordView<TRECORD> item in _Items)
        //        yield return (TRECORD)item;
        //    yield break;
        //}

        /// <summary>
        /// Select Query.
        /// </summary>
        protected String SelectCommandText
        {
            get { return _SelectCommandText; }
        }

        //public List<TRECORD> Items { get { return new List<TRECORD>(); } set { } }

        /// <summary>
        /// Create a new instance of ListRecord with TRECORD's type. This is a new empty object and not providers load
        /// a list of TRECORD object.
        /// </summary>
        /// <returns>A ListRecord empty objects</returns>
        public static ListRecord<TRECORD> CreateNewInstance()
        {
            return new ListRecord<TRECORD>();
        }

        /// <summary>
        /// Create a new instance of ListRecord with TRECORD's type generic and it providers to load a list of object from
        /// database.
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pParameters">List of parameters</param>
        /// <returns>A ListRecord fill objects</returns>
        public static ListRecord<TRECORD> CreateNewInstance(DbTransaction pTransaction, params RecordParameter[] pParameters)
        {
            return new ListRecord<TRECORD>(pTransaction, pParameters);
        }

        /// <summary>
        /// Create a new instance of ListRecord with TRECORD's type generic and it providers to load a list of object from
        /// database.
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pCondition">Conditions</param>
        /// <returns>A ListRecord fill objects</returns>
        public static ListRecord<TRECORD> CreateNewInstance(DbTransaction pTransaction, Condition pCondition)
        {
            return new ListRecord<TRECORD>(pTransaction, pCondition);
        }

        /// <summary>
        /// Create a new instance of ListRecord with TRECORD's type generic and it providers to load a list of object from
        /// database.
        /// </summary>
        /// <param name="pTransaction">A specific transaction<</param>
        /// <param name="pQuerySelect">A specific definition of query select</param>
        /// <param name="pParameters">List of parameters</param>
        /// <returns>A ListRecord fill objects</returns>
        public static ListRecord<TRECORD> CreateNewInstance(DbTransaction pTransaction, String pQuerySelect, params RecordParameter[] pParameters)
        {
            return new ListRecord<TRECORD>(pTransaction, pQuerySelect, pParameters);
        }

        /// <summary>
        /// Load a list of record from database
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pParams">List of prameters</param>
        public void Load(DbTransaction pTransaction, params RecordParameter[] pParameters)
        {
            LoadBefore();
            BuildSelectCommand(pParameters);
            InternalSelectCommand(pTransaction, pParameters);
            LoadAfter();
        }

        /// <summary>
        /// Load a list of record from database
        /// </summary>
        /// <param name="pTransation">A specific transaction</param>
        /// <param name="pCondition">Condition</param>
        /// <param name="pOrderBy">OrderBy field</param>
        public void Load(DbTransaction pTransaction, Condition pCondition, params RecordOrderBy[] pOrderBy)
        {
            LoadBefore();
            List<RecordParameter> pParameters = new List<RecordParameter>();
            BuildSelectCommand(pTransaction, pCondition, pParameters, pOrderBy);
            InternalSelectCommand(pTransaction, pParameters.ToArray());
            LoadAfter();
        }

        /// <summary>
        /// Load a list of record from database
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pSelectQuery">A specific query</param>
        /// <param name="pParameters">List of prameters</param>
        public void Load(DbTransaction pTransaction, String pSelectQuery, params RecordParameter[] pParameters)
        {
            LoadBefore();
            InternalSpecificSelectCommand(pTransaction, pSelectQuery, pParameters);
            LoadAfter();
        }

        /// <summary>
        /// Saves a list of record to fisics database.
        /// </summary>
        /// <param name="pTransaction"></param>
        public void SaveToDb(DbTransaction pTransaction)
        {
            SaveBefore();
            //foreach (TRECORD t in Items)
            //    t.SaveToDb(pTransaction);
            foreach (TRECORD t in Items)
                t.SaveToDb(pTransaction);
            SaveAfter();
        }

        /// <summary>
        /// Build a instance of this class.
        /// </summary>
        internal override void Initialize(DbTransaction pTransaction)
        {
            RecordType = typeof(TRECORD);
            this.RecordConnectionString = CreateConnectionString<TRECORD>(typeof(TRECORD));
            InitializeContext();
            InitializeMembers(typeof(TRECORD));
            _Columns = new List<RecordColumn>(Current[typeof(TRECORD)].m_Columns);
        }

        /// <summary>
        /// Create new connection string
        /// </summary>
        /// <returns>Current ConnectionString</returns>
        public new static String CreateConnectionString<T>(Type pTypeClass)
            where T : Record, new()
        {
            return new T().ConnectionString;

            //object[] members = pTypeClass.GetCustomAttributes(typeof(FieldConnectionString), true);
            //foreach (object obj in members)
            //{
            //    if (obj is FieldConnectionString)
            //    {
            //        FieldConnectionString ft = obj as FieldConnectionString;
            //        return ft.ConnectionString;
            //    }
            //}
            //throw new RecordConnectionStringNullException();
        }

        /// <summary>
        /// Build a Select query command
        /// </summary>
        /// <param name="pParams">List of record parameters</param>
        protected void BuildSelectCommand(params RecordParameter[] pParams)
        {
            if (RecordConnection == null)
            {
                RecordConnection = ProviderFactory.CreateConnection();
                RecordConnection.ConnectionString = ConnectionString;
                //RecordConnection = CreateConnection(null, this.ConnectionString);
            }

            String Attrs = String.Empty;
            String Where = String.Empty;

            foreach (String filed in Current[typeof(TRECORD)].m_Members) //foreach (MemberInfo member in Current[this.GetType()].Attributes)
            {
                MemberInfo member = typeof(TRECORD).GetMember(filed)[0];
                object field = member.GetCustomAttributes(typeof(Field), true)[0];

                if (!String.IsNullOrEmpty(Attrs))
                    Attrs += ", ";
                Attrs += String.Format("[{0}]", (field as Field).Name);
            }

            if (pParams != null && pParams.Length > 0)
                Where = String.Format("WHERE ({0})", BuildWhere(pParams));

            _SelectCommandText = String.Format("SELECT {0} FROM {1} {2}", Attrs, TableName, Where);
        }

        /// <summary>
        /// Build a Select query command
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="listOfParameters">List of record parameters</param>
        protected void BuildSelectCommand(DbTransaction transaction, Condition condition, List<RecordParameter> listOfParameters, params RecordOrderBy[] pOrderBy)
        {
            if (RecordConnection == null)
            {
                RecordConnection = ProviderFactory.CreateConnection();
                RecordConnection.ConnectionString = ConnectionString;
                //RecordConnection = CreateConnection(null, this.ConnectionString);
            }

            String SelectFormat = "SELECT {0} FROM {1} {2}";

            String Attrs = String.Empty;
            String Where = String.Empty;

            foreach (String filed in Current[typeof(TRECORD)].m_Members) //foreach (MemberInfo member in Current[this.GetType()].Attributes)
            {
                MemberInfo member = typeof(TRECORD).GetMember(filed)[0];

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

            if (pOrderBy != null && pOrderBy.Length > 0)
            {
                SelectFormat += " ORDER BY ";
                foreach (RecordOrderBy orderBy in pOrderBy)
                    SelectFormat += orderBy.ToString() + ",";
                SelectFormat = SelectFormat.Substring(0, SelectFormat.Length - 1);
            }

            _SelectCommandText = String.Format(SelectFormat, Attrs, TableName, Where);
        }

        /// <summary>
        /// Build a Where conditional for select query
        /// </summary>
        /// <param name="pParams">List of parameters</param>
        /// <returns>A string's format ([par1]=valu) AND ([par1] = value) </returns>
        protected String BuildWhere(params RecordParameter[] pParams)
        {
            String Where = String.Empty;
            if (pParams != null)
            {
                int pIndex = 0;
                foreach (RecordParameter param in pParams)
                {
                    if (param.OwnerParameter.ParameterName != String.Format("p{0}", pIndex))
                    {
                        param.ParameterField = param.ParameterName.Clone() as String;
                        param.ParameterName = String.Format("p{0}", pIndex);
                        param.OwnerParameter.ParameterName = String.Format("p{0}", pIndex);
                    }

                    if (!String.IsNullOrEmpty(Where))
                        Where += " AND ";
                    Where += CompareKindToString.SignWithFormat(RecordConnection, param, param.CompareKindExpression);
                    pIndex++;
                }
            }
            return Where;
        }

        /// <summary>
        /// After Load. The procedure load is when its call a Load(..)
        /// </summary>
        protected virtual void LoadAfter()
        {
        }

        /// <summary>
        /// Before Load. The procedure load is when its call a Load(..)
        /// </summary>
        protected virtual void LoadBefore()
        {
        }

        /// <summary>
        /// After save on fisics database
        /// </summary>
        protected virtual void SaveAfter()
        {
        }

        /// <summary>
        /// Before Save on fisics database
        /// </summary>
        protected virtual void SaveBefore()
        {
        }

        /// <summary>
        /// Execute a Select command
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pParameters">List of record parameters</param>
        private void InternalSelectCommand(DbTransaction pTransaction, params RecordParameter[] pParameters)
        {
            DbCommand SelectCommand = CreateCommand(pTransaction);
            SelectCommand.CommandText = _SelectCommandText;

            Debug.WriteLine(_SelectCommandText);

            if (pTransaction == null)
            {
                SelectCommand.Connection = RecordConnection;
                SelectCommand.Connection.ConnectionString = ChooseCurrentConnectionString();
            }
            else
            {
                SelectCommand.Connection = pTransaction.Connection;
            }
            SelectCommand.Parameters.Clear();

            if (pParameters != null && pParameters.Length > 0)
                foreach (RecordParameter par in pParameters)
                    SelectCommand.Parameters.Add(par.OwnerParameter);

            _Items.Clear();
            OnSelecting(new RecordSourceCommandEventArgs(SelectCommand));

            try
            {
                if (pTransaction == null)
                    SelectCommand.Connection.Open();

                DbDataReader reader = SelectCommand.ExecuteReader();

                InternalQueryColumnsList.Clear();
                foreach (DataRow col in reader.GetSchemaTable().Rows)
                    InternalQueryColumnsList.Add(col["ColumnName"] as String);

                _count = 0;
                foreach (RecordView<TRECORD> item in RecordView<TRECORD>.GetRecordViewItems<TRECORD>(reader, this, InternalQueryColumnsList))
                {
                    _Items.Add(item);
                    //OnDataItemBound(new RecordSourceItemBoundEventArgs(item));
                    _count++;
                }

                HasValue = _Items.Count > 0;
                OnNumberRows(new RecordSourceNumberRowsEventArgs(_Items.Count));

                OnSelected(new RecordSourceStatusEventArgs(SelectCommand, _Items.Count, null));
                OnDataBound(new RecordSourceBoundEventArgs(new List<RecordView>(_Items.ToArray())));

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

        public static List<T> DataReaderMapToList<T>(IDataReader dr)
            where T : Record
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    object[] fs = prop.GetCustomAttributes(typeof(Field), false);
                    Field f = null;
                    if (fs.Length > 0)
                        f = prop.GetCustomAttributes(typeof(Field), false)[0] as Field;

                    if (f != null && !object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        public static T DataReaderMapToObject<T>(IDataReader dr)
            where T : Record
        {
            T obj = default(T);

            obj = Activator.CreateInstance<T>();
            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                object[] fs = prop.GetCustomAttributes(typeof(Field), false);
                Field f = null;
                if (fs.Length > 0)
                    f = prop.GetCustomAttributes(typeof(Field), false)[0] as Field;

                if (f != null && !object.Equals(dr[f.Name], DBNull.Value))
                {
                    prop.SetValue(obj, dr[prop.Name], null);
                }
            }

            return obj;
        }

        /// <summary>
        /// Execute a Select command
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <param name="pSelectQuery">A specific query select</param>
        /// <param name="pParameters">List of record parameters</param>
        private void InternalSpecificSelectCommand(DbTransaction pTransaction, String pSelectQuery, params RecordParameter[] pParameters)
        {
            _SelectCommandText = pSelectQuery;
            DbCommand SelectCommand = CreateCommand(pTransaction);
            SelectCommand.CommandText = pSelectQuery;

            if (pTransaction == null)
            {
                SelectCommand.Connection = RecordConnection;
                if (SelectCommand.Connection == null)
                    SelectCommand.Connection = CreateConnection(pTransaction, ChooseCurrentConnectionString());

                SelectCommand.Connection.ConnectionString = ChooseCurrentConnectionString();
                //if (String.IsNullOrEmpty(SelectCommand.Connection.ConnectionString))
                //    SelectCommand.Connection.ConnectionString = this.ConnectionString;
            }
            else
            {
                SelectCommand.Connection = pTransaction.Connection;
            }

            SelectCommand.Parameters.Clear();

            if (pParameters != null && pParameters.Length > 0)
                foreach (RecordParameter par in pParameters)
                    SelectCommand.Parameters.Add(par.OwnerParameter);

            _Items.Clear();
            OnSelecting(new RecordSourceCommandEventArgs(SelectCommand));

            try
            {
                if (pTransaction == null)
                    SelectCommand.Connection.Open();

                DbDataReader reader = SelectCommand.ExecuteReader();

                List<String> columns = new List<string>();
                foreach (DataRow col in reader.GetSchemaTable().Rows)
                    columns.Add(col["ColumnName"] as String);
                _count = 0;
                foreach (RecordView<TRECORD> item in RecordView<TRECORD>.GetRecordViewItems<TRECORD>(reader, this, columns))
                {
                    //Mapper<TRECORD>.Map(item, new TRECORD());
                    _Items.Add(item);
                    //OnDataItemBound(new RecordSourceItemBoundEventArgs(item));
                    _count++;
                }

                HasValue = _Items.Count > 0;
                OnNumberRows(new RecordSourceNumberRowsEventArgs(_Items.Count));

                OnSelected(new RecordSourceStatusEventArgs(SelectCommand, _Items.Count, null));
                OnDataBound(new RecordSourceBoundEventArgs(new List<RecordView>(_Items.ToArray())));

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
        /// IListSource implementations
        /// </summary>
        public bool ContainsListCollection
        {
            get { return true; }
        }

        /// <summary>
        /// return a list of TRECORD items
        /// </summary>
        /// <returns></returns>
        public IList GetList()
        {
            List<TRECORD> list = new List<TRECORD>();
            //foreach (TRECORD record in Items)
            //    list.Add(record);
            foreach (TRECORD record in Items)
                list.Add(record);
            return list;
        }
    }
}