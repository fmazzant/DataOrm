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
using System.Diagnostics;

namespace Mafesoft.Data.Core
{
    /// <summary>
    /// Record connection exception
    /// </summary>
    [Serializable]
    public class RecordConnectionException : Exception
    {
        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        public RecordConnectionException()
            : base()
        {
            Debug.WriteLine(String.Format("RecordException: {0}", "Empty Message"));
        }

        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        /// <param name="pMessage">Message to visible</param>
        public RecordConnectionException(String pMessage)
            : base(pMessage)
        {
            Debug.WriteLine(String.Format("RecordException: {0}", pMessage));
        }
    }

    /// <summary>
    /// Record Connection Null Exception
    /// </summary>
    [Serializable]
    public class RecordConnectionNullException : NullReferenceException
    {
        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        public RecordConnectionNullException()
            : base()
        {
            Debug.WriteLine(String.Format("RecordException: {0}", "Empty Message"));
        }

        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        /// <param name="pMessage">Message to visible</param>
        public RecordConnectionNullException(String pMessage)
            : base(pMessage)
        {
            Debug.WriteLine(String.Format("RecordException: {0}", pMessage));
        }
    }

    /// <summary>
    /// Record Connection String Null Exception
    /// </summary>
    [Serializable]
    public class RecordConnectionStringNullException : NullReferenceException
    {
        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        public RecordConnectionStringNullException()
            : base()
        {
            Debug.WriteLine(String.Format("RecordException: {0}", "Empty Message"));
        }

        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        /// <param name="pMessage">Message to visible</param>
        public RecordConnectionStringNullException(String pMessage)
            : base(pMessage)
        {
            Debug.WriteLine(String.Format("RecordException: {0}", pMessage));
        }
    }

    /// <summary>
    /// Generic Exception instance
    /// </summary>
    [Serializable]
    public class RecordException : Exception
    {
        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        public RecordException()
            : base()
        {
            Debug.WriteLine(String.Format("RecordException: {0}", "Empty Message"));
        }

        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        /// <param name="pMessage">Message to visible</param>
        public RecordException(String pMessage)
            : base(pMessage)
        {
            Debug.WriteLine(String.Format("RecordException: {0}", pMessage));
        }
    }

    /// <summary>
    /// Record Handler Null Exception
    /// </summary>
    [Serializable]
    public class RecordHandlerNullException : NullReferenceException
    {
        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        public RecordHandlerNullException()
            : base()
        {
            Debug.WriteLine(String.Format("RecordException: {0}", "Empty Message"));
        }

        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        /// <param name="pMessage">Message to visible</param>
        public RecordHandlerNullException(String pMessage)
            : base(pMessage)
        {
            Debug.WriteLine(String.Format("RecordException: {0}", pMessage));
        }
    }

    /// <summary>
    /// Record ProviderFactory Null Exception
    /// </summary>
    [Serializable]
    public class RecordProviderFactoryNullException : NullReferenceException
    {
        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        public RecordProviderFactoryNullException()
            : base()
        {
            Debug.WriteLine(String.Format("RecordException: {0}", "Empty Message"));
        }

        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        /// <param name="pMessage">Message to visible</param>
        public RecordProviderFactoryNullException(String pMessage)
            : base(pMessage)
        {
            Debug.WriteLine(String.Format("RecordException: {0}", pMessage));
        }
    }

    /// <summary>
    /// Record Query table null exception
    /// </summary>
    [Serializable]
    public class RecordQueryNullException : NullReferenceException
    {
        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        public RecordQueryNullException()
            : base()
        {
            Debug.WriteLine(String.Format("RecordQueryNullException: {0}", "Empty Message"));
        }

        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        /// <param name="pMessage">Message to visible</param>
        public RecordQueryNullException(String pMessage)
            : base(pMessage)
        {
            Debug.WriteLine(String.Format("RecordQueryNullException: {0}", pMessage));
        }
    }

    /// <summary>
    /// Record table null exception
    /// </summary>
    [Serializable]
    public class RecordTableNullException : NullReferenceException
    {
        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        public RecordTableNullException()
            : base()
        {
            Debug.WriteLine(String.Format("RecordException: {0}", "Empty Message"));
        }

        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        /// <param name="pMessage">Message to visible</param>
        public RecordTableNullException(String pMessage)
            : base(pMessage)
        {
            Debug.WriteLine(String.Format("RecordException: {0}", pMessage));
        }
    }

    /// <summary>
    /// Record valid exception instance
    /// </summary>
    [Serializable]
    public class RecordValidException : Exception
    {
        /// <summary>
        /// Create a new instance of exception
        /// </summary>
        public RecordValidException()
            : base()
        {
            Debug.WriteLine(String.Format("RecordException: {0}", "Empty Message"));
        }

        /// <summary>
        /// Create a new instance of Exception
        /// </summary>
        /// <param name="pMessage">Message to visible</param>
        public RecordValidException(String pMessage)
            : base(pMessage)
        {
            Debug.WriteLine(String.Format("RecordException: {0}", pMessage));
        }
    }
}