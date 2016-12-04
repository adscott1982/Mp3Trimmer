// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Vision RT Ltd.">
//   Copyright (c) Vision RT Ltd. All rights reserved.
// </copyright>
// <summary>
//   The Logger interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AndyTools.Wpf
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// The Logger interface.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the log as an observable collection.
        /// </summary>
        ObservableCollection<string> Log { get; }

        /// <summary>
        /// Add entries to the log
        /// </summary>
        /// <param name="logEntry">
        /// The log entry.
        /// </param>
        void Add(string logEntry);
    }
}
