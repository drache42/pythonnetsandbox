using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringPrinterSDK
{
    public abstract class PythonLogger : ILogger, IPyLogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            // In python.net, a generic method like Log<T> can't be concretely implemented because python has no understanding of generic methods.
            // To get around this, we pre-format the message, and pass it to the non-generic `Log` method
            this.Log(logLevel, eventId, formatter(state, null), exception, formatter(state, exception));
        }

        /// <summary>
        /// Non-Generic Log message which takes strings instead of the TState from ILogger.Log<TState> 
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written as a string</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="message">Combined message of <paramref name="state"/> and <paramref name="exception"/></param>
        public abstract void Log(LogLevel logLevel, EventId eventId, string state, Exception? exception, string message);

        /// <summary>
        /// Checks if the given <paramref name="logLevel"/> is enabled.
        /// </summary>
        /// <param name="logLevel">Level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        public abstract bool IsEnabled(LogLevel logLevel);
    }
}
