﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DotNext.Net.Cluster.Replication
{
    internal class LogEntryList<LogEntry> : ReadOnlyCollection<LogEntry>, IAuditTrailSegment<LogEntry>
        where LogEntry : class, ILogEntry
    {
        internal LogEntryList(IList<LogEntry> entries)
            : base(entries)
        {
        }

        internal LogEntryList()
            : this(Array.Empty<LogEntry>())
        {
        }

        long? IAuditTrailSegment<LogEntry>.SnapshotIndex => null;

        void IDisposable.Dispose() { }
    }

    //TODO: Should be removed in .NET Standard 2.1

    /// <summary>
    /// Represents internal API that is not intended for public usage.
    /// </summary>
    public static class LogEntryEnumerator
    {
        /// <summary>
        /// Represents a method that is compatible with the signature of asynchronous enumeration method.
        /// </summary>
        /// <typeparam name="LogEntry">The type of the log entry.</typeparam>
        /// <param name="entries">The enumerator over the collection of log entries.</param>
        /// <returns>The log entry obtained from the enumerator.</returns>
        public static ValueTask<LogEntry> Advance<LogEntry>(this IEnumerator<LogEntry> entries) where LogEntry : class, ILogEntry => new ValueTask<LogEntry>(entries.MoveNext() ? entries.Current : null);
    }
}
