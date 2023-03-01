using StringPrinterSDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace CSharpProgram
{
    class PythonLoggerImpl : PythonLogger
    {
        private readonly string _name;

        public PythonLoggerImpl(string name)
        {
            _name = name.ToUpper();
        } 

        public override bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public override void Log(LogLevel logLevel, EventId eventId, string state, Exception? exception, string message)
        {
            Console.WriteLine($"{_name}: "+ message);
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddStringPrinterSDK();
                    services.AddLogging(builder =>
                    {
                        builder.ClearProviders();

                        builder.AddPythonLogger(services =>
                        {
                             return new PythonLoggerProvider(name => new PythonLoggerImpl(name));
                        });
                    });
                })
                .Build();

            var stringPrinter = host.Services.GetRequiredService<IStringPrinter>();

            stringPrinter.PrePrint += StringPrinter_PrePrint;
            stringPrinter.PostPrint += StringPrinter_PostPrint;

            var t = stringPrinter.AsyncPrintWillThrow();
            var a = t.GetAwaiter();
            var r = a.GetResult();

            // Normal print
            var result = stringPrinter.Print();
            Console.WriteLine($"Succeeded: {result.Succeeded}");

            // Object print
            var printObj = new PrintObject();
            result = stringPrinter.Print(printObj);
            Console.WriteLine($"Succeeded: {result.Succeeded}");

            // Async print
            result = await stringPrinter.AsyncPrint();
            Console.WriteLine($"Succeeded: {result.Succeeded}");

            // Throw an exception
            try
            {
                #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is expected for this POC
                result = stringPrinter.Print(null);
                #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                Console.WriteLine($"Succeeded: {result.Succeeded}");
            }
            catch (Exception)
            {
                Console.WriteLine("Yay an exception!");
            }
        }

        private static void StringPrinter_PrePrint(object? sender, PrePrintEventArgs e)
        {
            Console.WriteLine($"My pre print function. {e.PrePrintString}");
        }

        private static void StringPrinter_PostPrint(object? sender, PostPrintEventArgs e)
        {
            Console.WriteLine($"My post print function. {e.PostPrintString}");
        }
    }
}