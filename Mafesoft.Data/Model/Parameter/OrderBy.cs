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

namespace Mafesoft.Data.Model.Parameter
{
    /// <summary>
    /// Order by kind
    /// </summary>
    public enum RecordOrderByKind
    {
        ASC,
        DESC
    }

    /// <summary>
    /// Represents a single order by definition
    /// </summary>
    public class RecordOrderBy
    {
        /// <summary>
        /// Create a new Order By definition
        /// </summary>
        /// <param name="pColumnName">Column's name</param>
        /// <param name="pKind">Order by kind</param>
        public RecordOrderBy(String pColumnName, RecordOrderByKind pKind)
            : base()
        {
            ColumnName = pColumnName;
            Kind = pKind;
        }

        /// <summary>
        /// Column's name
        /// </summary>
        public String ColumnName { get; internal set; }

        /// <summary>
        /// Order by kind
        /// </summary>
        public RecordOrderByKind Kind { get; internal set; }

        /// <summary>
        /// Converto object to string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}] {1}", ColumnName, Kind.ToString());
        }
    }
}