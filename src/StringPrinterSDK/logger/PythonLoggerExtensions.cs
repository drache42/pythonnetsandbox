using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace StringPrinterSDK
{
    public static class PythonLoggerExtensions
    {
        /// <summary>
        /// Adds a Python Logger
        /// </summary>
        /// <param name="pythonProviderFactory">Function that creates a new pythongLoggerProvider</param>
        /// <returns></returns>
        public static ILoggingBuilder AddPythonLogger(this ILoggingBuilder builder, Func<IServiceProvider, PythonLoggerProvider> pythonProviderFactory)
        {
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, PythonLoggerProvider>(pythonProviderFactory));

            return builder;
        }
    }
}
