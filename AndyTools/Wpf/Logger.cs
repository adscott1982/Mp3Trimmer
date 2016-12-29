// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Vision RT Ltd.">
//   Copyright (c) Vision RT Ltd. All rights reserved.
// </copyright>
// <summary>
//   The logger class. Holds an observable collection of string objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace AndyTools.Wpf
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The logger class. Holds an observable collection of string objects.
    /// </summary>
    public class Logger : ILogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class. 
        /// </summary>
        public Logger()
        {
            this.Log = new ObservableCollection<string>();
        }

        /// <summary>
        /// Gets the log.
        /// </summary>
        public ObservableCollection<string> Log { get; private set; }

        /// <summary>
        /// Add an entry to the log.
        /// </summary>
        /// <param name="logEntry">
        /// The log entry as a string.
        /// </param>
        public void Add(string logEntry)
        {
            if (string.IsNullOrWhiteSpace(logEntry))
            {
                return;
            }

            var time = DateTime.Now.ToString("HH:mm:ss");
            this.Log.Add($"{time} - {logEntry}");
        }

        public void Append(string logAppend)
        {
            if (string.IsNullOrWhiteSpace(logAppend))
            {
                return;
            }

            var currentString = this.Log.Last();
            var newString = currentString + logAppend;
            this.Log[this.Log.Count - 1] = newString;
        }
    }
}
