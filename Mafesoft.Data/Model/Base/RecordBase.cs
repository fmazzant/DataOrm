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
using Mafesoft.Data.Core.Parameter;
using Mafesoft.Data.Model.Parameter;
using System;
using System.Data.Common;
using System.Reflection;
using System.Threading;

namespace Mafesoft.Data.Core.Common
{
    /// <summary>
    /// Handler for transaction execution
    /// </summary>
    /// <param name="transaction"></param>
    public delegate void StatmentHandler(DbTransaction transaction);

    /// <summary>
    /// Handler for transaction execution with error
    /// </summary>
    /// <param name="e">The e.</param>
    public delegate void StatmentErrorHandler(Exception e);

    /// <summary>
    /// Handler for transaction execution is completed.
    /// </summary>
    public delegate void StatmentCompletedHandler();

    /// <summary>
    /// Represents a record base.
    /// </summary>
    public abstract class RecordBase : RecordSource
    {
        /// <summary>
        /// A static ConnectionString for application
        /// </summary>
        //public static String SharedConnectionString = String.Empty;

        //public String ConnectionString = String.Empty;

        /// <summary>
        /// A static SharedProviderFactory for application
        /// </summary>
        //public static DbProviderFactory SharedProviderFactory
        //{
        //    get { return _SharedProviderFactory; }
        //    set
        //    {
        //        _SharedProviderFactory = value;
        //    }
        //}

        //private static DbProviderFactory _SharedProviderFactory = null;
        //internal static DbProviderFactory ProviderFactory = null;

        /// <summary>
        /// Limit edition when Limit is true.
        /// </summary>
        internal static Boolean Limit = false;

        /// <summary>
        /// BindingFlags operator
        /// </summary>
        internal BindingFlags _BindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        private DbConnection _RecordConnection = null;
        private String _RecordConnectionString = null;
        private DbProviderFactory _RecordProviderFactory = null;

        /// <summary>
        /// Create new Record base instance
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        protected RecordBase(DbTransaction pTransaction)
            : base()
        {
            Initialize(pTransaction);
        }

        /// <summary>
        /// Handler to execution generic statment
        /// </summary>
        /// <typeparam name="TRESULT"></typeparam>
        /// <returns></returns>
        public delegate TRESULT StatmentThreadHandler<TRESULT>();

        /// <summary>
        /// Handler to result generic statment execution
        /// </summary>
        /// <typeparam name="TRESULT">Type of</typeparam>
        /// <param name="obj"></param>
        public delegate void StatmentThreadHandlerAsync<TRESULT>(TRESULT obj);

        /// <summary>
        /// BindingFlags Operator
        /// </summary>
        internal BindingFlags RecordBindingFlags
        {
            get { return _BindingFlags; }
        }

        /// <summary>
        /// RecordConnection
        /// </summary>
        protected DbConnection RecordConnection
        {
            get { return _RecordConnection; }
            set { _RecordConnection = value; }
        }

        /// <summary>
        /// RecordConnectionString
        /// </summary>
        protected String RecordConnectionString
        {
            get { return _RecordConnectionString; }
            set { _RecordConnectionString = value; }
        }

        /// <summary>
        /// RecordProviderFactory
        /// </summary>
        protected DbProviderFactory RecordProviderFactory
        {
            get { return _RecordProviderFactory; }
            set { _RecordProviderFactory = value; }
        }

        /// <summary>
        /// Build a instance of Record
        /// </summary>
        internal abstract void Initialize(DbTransaction pTransaction);

        /// <summary>
        /// Create a Adapter instance
        /// </summary>
        /// <returns></returns>
        public static DbDataAdapter CreateAdapter<TRECORD>()
            where TRECORD : Record, new()
        {
            return new TRECORD().ProviderFactory.CreateDataAdapter();
            //if (SharedProviderFactory == null)
            //    throw new RecordProviderFactoryNullException("ProviderFactory is null!");
            //DbDataAdapter adapter = SharedProviderFactory.CreateDataAdapter();
            //return adapter;
        }

        /// <summary>
        /// Create a new command instance
        /// </summary>
        /// <param name="pQuery">A specific query</param>
        /// <param name="pTransaction">A specific transaction</param>
        /// <returns></returns>
        public static DbCommand CreateCommand<TRECORD>(String pQuery, DbTransaction pTransaction)
               where TRECORD : Record, new()
        {
            DbCommand cmd = new TRECORD().ProviderFactory.CreateCommand();

            if (!String.IsNullOrEmpty(pQuery))
                cmd.CommandText = pQuery;

            if (pTransaction != null)
            {
                cmd.Transaction = pTransaction;
            }

            return cmd;
        }

        /// <summary>
        /// Create a new connection instance
        /// </summary>
        /// <param name="pTransaction">A specific transaction</param>
        /// <returns></returns>
        public static DbConnection CreateConnection<TRECORD>(DbTransaction pTransaction, String pConnectionString)
            where TRECORD : Record, new()
        {
            if (pConnectionString == null)
                throw new RecordConnectionStringNullException("ConnectionString is null!"); ;
            if (pTransaction != null)
                return pTransaction.Connection;
            DbConnection conn = new TRECORD().ProviderFactory.CreateConnection();
            conn.ConnectionString = pConnectionString;
            return conn;
        }

        /// <summary>
        /// Create new connection string
        /// </summary>
        /// <param name="pTypeClass">The p type class.</param>
        /// <returns>
        /// Current ConnectionString
        /// </returns>
        /// <exception cref="RecordConnectionStringNullException"></exception>
        public static String CreateConnectionString<TRECORD>(Type pTypeClass)
            where TRECORD : Record, new()
        {
            //if (!String.IsNullOrEmpty(SharedConnectionString))
            //    return SharedConnectionString;

            //object[] members = pTypeClass.GetCustomAttributes(typeof(FieldConnectionString), true);
            //foreach (object obj in members)
            //{
            //    if (obj is FieldConnectionString)
            //    {
            //        FieldConnectionString ft = obj as FieldConnectionString;
            //        return ft.ConnectionString;
            //    }
            //}
            return new TRECORD().ConnectionString;
        }

        /// <summary>
        /// Create a new order by definition
        /// </summary>
        /// <param name="columnName">Column's name</param>
        /// <param name="orderByKind">Colun's kind ordering</param>
        /// <returns></returns>
        public static RecordOrderBy CreateOrderBy(String columnName, RecordOrderByKind orderByKind)
        {
            return new RecordOrderBy(columnName, orderByKind);
        }

        /// <summary>
        /// Create a new parameter instance
        /// </summary>
        /// <typeparam name="TRECORD">A generic Record type</typeparam>
        /// <param name="pParameterName">Parameter's name</param>
        /// <param name="pValue">Parameter's value</param>
        /// <returns>Parameter</returns>
        public static RecordParameter CreateParameter<TRECORD>(String pParameterName, object pValue)
            where TRECORD : Record, new()
        {
            return CreateParameter<TRECORD>(pParameterName, CompareKind.Equal, pValue);
        }

        /// <summary>
        /// Create a new parameter instance
        /// </summary>
        /// <typeparam name="TRECORD">A generic Record type</typeparam>
        /// <param name="pParameterName">Parameter's name</param>
        /// <param name="pCompareKind">Compare operation</param>
        /// <param name="pValue">Parameter's value</param>
        /// <returns>Parameter</returns>
        public static RecordParameter CreateParameter<TRECORD>(String pParameterName, CompareKind pCompareKind, object pValue)
            where TRECORD : Record, new()
        {
            object[] customs = typeof(TRECORD).GetCustomAttributes(true);

            //foreach (object obj in customs)
            //    if (obj is FieldProviderName)
            //        SharedProviderFactory = CreateProviderFactoty((obj as FieldProviderName).ProviderSupport);

            //if (SharedProviderFactory == null)
            //    throw new RecordProviderFactoryNullException("Provider Factory is null");

            //DbParameter parameter = (DbParameter)SharedProviderFactory.CreateParameter();
            DbParameter parameter = (DbParameter)new TRECORD().ProviderFactory.CreateParameter();
            parameter.ParameterName = pParameterName;
            parameter.Value = pValue != null ? pValue : String.Empty;
            RecordParameter rparameter = new RecordParameter(parameter, pCompareKind);
            return rparameter;
        }

        /// <summary>
        /// Create a provider factory instace
        /// </summary>
        /// <param name="pProviderFactory">Provider support name</param>
        /// <returns>Provider factory</returns>
        public static DbProviderFactory CreateProviderFactoty(ProviderFactorySupport pProviderFactory)
        {
            //if (SharedProviderFactory != null)
            //    return SharedProviderFactory;

            if (pProviderFactory == ProviderFactorySupport.None)
                throw new RecordProviderFactoryNullException("ProviderFactory is null!");

            String ProviderName = Convert.InternalConvert.ConvertProviderToString(pProviderFactory);

            DbProviderFactory provider = DbProviderFactories.GetFactory(ProviderName);
            return provider;
            //SharedProviderFactory = provider;
            //return SharedProviderFactory;
        }

        /// <summary>
        /// Execution Handler as Thread and response with async handler type
        /// </summary>
        /// <typeparam name="TRESULT"></typeparam>
        /// <param name="hander"></param>
        /// <param name="handlerAsync"></param>
        public static void ExecuteStatmentAsThreadAsync<TRESULT>(StatmentThreadHandler<TRESULT> hander, StatmentThreadHandlerAsync<TRESULT> handlerAsync)
        {
            Thread t = new Thread(new ThreadStart(delegate() { handlerAsync(hander()); }));
            t.Start();
        }

        /// <summary>
        /// Gets the name of the catalog.
        /// </summary>
        /// <param name="pType">Type of the p.</param>
        /// <returns></returns>
        /// <exception cref="Mafesoft.Data.Core.RecordTableNullException">Catalog's name is null</exception>
        public static String GetCatalogName(Type pType)
        {
            String CatalogName = String.Empty;
            object[] members = pType.GetCustomAttributes(typeof(FieldCatalog), true);
            foreach (object obj in members)
            {
                if (obj is FieldCatalog)
                {
                    FieldCatalog ft = obj as FieldCatalog;
                    CatalogName = "[" + ft.CatalogName + "]";
                    return CatalogName;
                }
            }
            throw new RecordTableNullException("Catalog's name is null");
        }

        /// <summary>
        /// Gets the name of the scheme.
        /// </summary>
        /// <param name="pType">Type of the p.</param>
        /// <returns></returns>
        /// <exception cref="Mafesoft.Data.Core.RecordTableNullException">Scheme's name is null</exception>
        public static String GetSchemeName(Type pType)
        {
            String SchemeName = String.Empty;
            object[] members = pType.GetCustomAttributes(typeof(FieldScheme), true);
            foreach (object obj in members)
            {
                if (obj is FieldScheme)
                {
                    FieldScheme ft = obj as FieldScheme;
                    SchemeName = "[" + ft.SchemeName + "]";
                    return SchemeName;
                }
            }
            throw new RecordTableNullException("Scheme's name is null");
        }

        /// <summary>
        /// Returns a table's name
        /// </summary>
        /// <param name="pType">Record type</param>
        /// <returns></returns>
        public static String GetTableName(Type pType)
        {
            String _TableName = String.Empty;
            object[] members = pType.GetCustomAttributes(typeof(FieldTable), true);
            foreach (object obj in members)
            {
                if (obj is FieldTable)
                {
                    FieldTable ft = obj as FieldTable;
                    _TableName = "[" + ft.TableName + "]";
                    return _TableName;
                }
            }
            throw new RecordTableNullException("Table's name is null");
        }
    }
}