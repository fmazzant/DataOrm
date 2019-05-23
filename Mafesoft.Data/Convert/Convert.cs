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
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;

using System.Data.SqlClient;

namespace Mafesoft.Data.Convert
{
    /// <summary>
    /// Represents a class witch internal convert
    /// </summary>
    internal class InternalConvert
    {
        /// <summary>
        /// Converts a Provider factories supported in to String.
        /// </summary>
        /// <param name="pProviderFactory">Provider Factory Support</param>
        /// <returns></returns>
        public static String ConvertProviderToString(ProviderFactorySupport pProviderFactory)
        {
            switch (pProviderFactory)
            {
                case ProviderFactorySupport.Odbc:
                    return "System.Data.Odbc";

                case ProviderFactorySupport.OleDb:
                    return "System.Data.OleDb";

                case ProviderFactorySupport.OracleClient:
                    return "System.Data.OracleClient";

                case ProviderFactorySupport.SqlClient:
                    return "System.Data.SqlClient";

                case ProviderFactorySupport.SqlServerCe:
                    return "System.Data.SqlServerCe.3.5";

                default:
                    return "System.Data.SqlClient";
            }
        }

        /// <summary>
        /// Converts a parameter in to opportune format by connection type.
        /// </summary>
        /// <param name="pConnection">DbConnection</param>
        /// <param name="pFieldName">Field's name</param>
        /// <returns></returns>
        public static String ConvertQueryArgument(DbConnection pConnection, Object pFieldName)
        {
            if (pConnection != null)
            {
                if (pConnection.GetType() == typeof(SqlConnection))
                    return String.Format("@{0}", pFieldName);

                if (pConnection.GetType() == typeof(OleDbConnection))
                    return String.Format("?");

                //if (pConnection.GetType() == typeof(SqlCeConnection))
                //    return String.Format("@{0}", pFieldName);

                //if (pConnection.GetType() == typeof(OracleConnection))
                //    return String.Format("@{0}", pFieldName);

                if (pConnection.GetType() == typeof(OdbcConnection))
                    return String.Format("@{0}", pFieldName);
            }
            return String.Format("@{0}", pFieldName);
        }
    }
}