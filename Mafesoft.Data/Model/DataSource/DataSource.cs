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
using System.Collections.Generic;
using System.Data.Common;

namespace Mafesoft.Data.Core.Common
{
    /// <summary>
    /// Represents the method that will handle when know number of record selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="rows"></param>
    public delegate void NumberRowsHandler(object sender, RecordSourceNumberRowsEventArgs e);

    /// <summary>
    /// Represents the method that will handle when a list of item is bounded
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">Contains the event data</param>
    public delegate void RecordSourceBoundHandler(object sender, RecordSourceBoundEventArgs e);

    /// <summary>
    /// Represents the method that will handle Selecting, Updating, Inserting and Deleting events
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">Contains the event data</param>
    public delegate void RecordSourceCommandHandler(object sender, RecordSourceCommandEventArgs e);

    /// <summary>
    /// Represents the method that will handle when an item is bounded
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">Contains the event data</param>
    public delegate void RecordSourceItemBoundHandler(object sender, RecordSourceItemBoundEventArgs e);

    /// <summary>
    /// Represents the method that will handle Selected, Updated, Inserted and Deleted events
    /// </summary>
    /// <param name="sender">The source of the event</param>
    /// <param name="e">Contains the event data</param>
    public delegate void RecordSourceStatusHandler(object sender, RecordSourceStatusEventArgs e);

    /// <summary>
    /// RecordSource represents a source event base class.
    /// </summary>
    public class RecordSource
    {
        /// <summary>
        /// Create a new protected instance of RecordSource
        /// </summary>
        protected RecordSource()
        {
        }

        /// <summary>
        /// Occurs when a Data Bound operation has completed
        /// </summary>
        public event RecordSourceBoundHandler DataBound;

        /// <summary>
        /// Occurs when a Data Item Bound operation has completed
        /// </summary>
        public event RecordSourceItemBoundHandler DataItemBound;

        /// <summary>
        /// Occurs when a delete operation has completed
        /// </summary>
        public event RecordSourceStatusHandler Deleted;

        /// <summary>
        /// Occurs before a delete operation.
        /// </summary>
        public event RecordSourceCommandHandler Deleting;

        /// <summary>
        /// Occurs when a insert operation has completed
        /// </summary>
        public event RecordSourceStatusHandler Inserted;

        /// <summary>
        /// Occurs before a insert operation.
        /// </summary>
        public event RecordSourceCommandHandler Inserting;

        /// <summary>
        /// Occurs when a query is completed
        /// </summary>
        public event NumberRowsHandler NumberRows;

        /// <summary>
        /// Occurs when a select operation has completed
        /// </summary>
        public event RecordSourceStatusHandler Selected;

        /// <summary>
        /// Occurs before a select operation.
        /// </summary>
        public event RecordSourceCommandHandler Selecting;

        /// <summary>
        /// Occurs when a update operation has completed
        /// </summary>
        public event RecordSourceStatusHandler Updated;

        /// <summary>
        /// Occurs before a update operation.
        /// </summary>
        public event RecordSourceCommandHandler Updating;

        #region OnEvent

        internal virtual void OnDataBound(RecordSourceBoundEventArgs e)
        {
            if (DataBound != null)
                DataBound(this, e);
        }

        internal virtual void OnDataItemBound(RecordSourceItemBoundEventArgs e)
        {
            if (DataItemBound != null)
                DataItemBound(this, e);
        }

        internal virtual void OnDeleted(RecordSourceStatusEventArgs e)
        {
            if (Deleted != null)
                Deleted(this, e);
        }

        internal virtual void OnDeleting(RecordSourceCommandEventArgs e)
        {
            if (Deleting != null)
                Deleting(this, e);
        }

        internal virtual void OnInserted(RecordSourceStatusEventArgs e)
        {
            if (Inserted != null)
                Inserted(this, e);
        }

        internal virtual void OnInserting(RecordSourceCommandEventArgs e)
        {
            if (Inserting != null)
                Inserting(this, e);
        }

        internal virtual void OnNumberRows(RecordSourceNumberRowsEventArgs e)
        {
            if (NumberRows != null)
                NumberRows(this, e);
        }

        internal virtual void OnSelected(RecordSourceStatusEventArgs e)
        {
            if (Selected != null)
                Selected(this, e);
        }

        internal virtual void OnSelecting(RecordSourceCommandEventArgs e)
        {
            if (Selecting != null)
                Selecting(this, e);
        }

        internal virtual void OnUpdated(RecordSourceStatusEventArgs e)
        {
            if (Updated != null)
                Updated(this, e);
        }

        internal virtual void OnUpdating(RecordSourceCommandEventArgs e)
        {
            if (Updating != null)
                Updating(this, e);
        }

        #endregion OnEvent
    }

    /// <summary>
    /// Contains the list of item that it bounded
    /// </summary>
    public class RecordSourceBoundEventArgs : EventArgs
    {
        private IList<RecordView> _Items = null;

        /// <summary>
        /// Create a new instance of items that it bounded
        /// </summary>
        /// <param name="pItem">list of item</param>
        public RecordSourceBoundEventArgs(IList<RecordView> pItem)
        {
            _Items = pItem;
        }

        /// <summary>
        /// List of item that it bounded
        /// </summary>
        public IList<RecordView> Items
        {
            get { return _Items; }
        }
    }

    /// <summary>
    /// Contains the event command data
    /// </summary>
    public class RecordSourceCommandEventArgs : EventArgs
    {
        private DbCommand _Command = null;

        /// <summary>
        /// Creates a new instance of Container command event data.
        /// </summary>
        /// <param name="pCommand"></param>
        public RecordSourceCommandEventArgs(DbCommand pCommand)
        {
            _Command = pCommand;
        }

        /// <summary>
        /// Command executing
        /// </summary>
        public DbCommand Command
        {
            get { return _Command; }
        }
    }

    /// <summary>
    /// Contains the item that it bounded
    /// </summary>
    public class RecordSourceItemBoundEventArgs : EventArgs
    {
        private Record _Item = null;

        /// <summary>
        /// Create a new instance of Container the item
        /// </summary>
        /// <param name="pItem">item</param>
        public RecordSourceItemBoundEventArgs(Record pItem)
        {
            _Item = pItem;
        }

        /// <summary>
        /// Item that it bounded
        /// </summary>
        public Record Item
        {
            get { return _Item; }
        }
    }

    /// <summary>
    /// Contains the number of Rows that it bounded
    /// </summary>
    public class RecordSourceNumberRowsEventArgs : EventArgs
    {
        private int _NumberRows = 0;

        /// <summary>
        /// Create a new instance of number of Rows bounded
        /// </summary>
        /// <param name="numberRows"></param>
        public RecordSourceNumberRowsEventArgs(int numberRows)
        {
            _NumberRows = numberRows;
        }

        /// <summary>
        /// Number of Rows
        /// </summary>
        public int NumberRows
        {
            get { return _NumberRows; }
        }
    }

    /// <summary>
    /// Contains the result of command execution
    /// </summary>
    public class RecordSourceStatusEventArgs : EventArgs
    {
        private int _AffectedRows = 0;
        private DbCommand _Command = null;
        private Exception _Exception = null;

        /// <summary>
        /// Create a new instance of result command executed
        /// </summary>
        /// <param name="pCommand">Command executed</param>
        /// <param name="pAffectedRows">Rows modified</param>
        /// <param name="pException">Exception during command execution</param>
        public RecordSourceStatusEventArgs(DbCommand pCommand, int pAffectedRows, Exception pException)
        {
            _Command = pCommand;
            _AffectedRows = pAffectedRows;
            _Exception = pException;
        }

        /// <summary>
        /// Rows modified
        /// </summary>
        public int AffectedRows
        {
            get { return _AffectedRows; }
        }

        /// <summary>
        /// Command executed
        /// </summary>
        public DbCommand Command
        {
            get { return _Command; }
        }

        /// <summary>
        /// Exception during command execution. It is null when not exists an exception
        /// </summary>
        public Exception Exception
        {
            get { return _Exception; }
        }
    }
}