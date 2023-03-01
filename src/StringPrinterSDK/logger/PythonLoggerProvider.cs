using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace StringPrinterSDK
{
    [UnsupportedOSPlatform("browser")]
    [ProviderAlias("Python")]
    public class PythonLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, PythonLogger> _loggers =
            new(StringComparer.OrdinalIgnoreCase);

        Func<string, PythonLogger> _newLogger;

        /// <summary>
        /// Pass in a concrete logger object
        /// </summary>
        /// <param name="logger"></param>
        public PythonLoggerProvider(Func<string, PythonLogger> newLogger)
        {
            _newLogger = newLogger;
        }

        /// <summary>
        /// Use the concrete object to create a version of itself
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public ILogger CreateLogger(string categoryName)
        {
            var concreteLogger = _newLogger(categoryName);
            if (concreteLogger != null) 
            {
                return _loggers.GetOrAdd(categoryName, concreteLogger);
            }

            throw new NullReferenceException("Unable to create PythonLogger");
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
