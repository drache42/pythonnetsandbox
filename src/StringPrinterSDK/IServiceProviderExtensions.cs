using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringPrinterSDK
{
    public static class IServiceProviderExtensions
    {
        public static IServiceCollection AddStringPrinterSDK(this IServiceCollection services)
        {
            services.AddSingleton<IStringPrinter, StringPrinter>();
            return services;
        }
    }
}
