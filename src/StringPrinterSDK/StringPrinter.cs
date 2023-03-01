using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringPrinterSDK
{
    public class StringPrinter : IStringPrinter
    {
        public event EventHandler<PrePrintEventArgs>? PrePrint;
        public event EventHandler<PostPrintEventArgs>? PostPrint;

        private ILogger? logger;
        
        protected virtual void OnPrePrint(PrePrintEventArgs e)
        {
            PrePrint?.Invoke(this, e);
        }
        
        protected virtual void OnPostPrint(PostPrintEventArgs e)
        {
            PostPrint?.Invoke(this, e);
        }

        public StringPrinter(ILogger<StringPrinter>? _logger) 
        {
            logger = _logger;
        }
        
        public PrintResult Print()
        {
            logger?.LogInformation("String to print");
            OnPrePrint(new PrePrintEventArgs("Pre Print string"));
            
            Console.WriteLine("Hello World!");
            
            OnPostPrint(new PostPrintEventArgs("Post Print string"));
            logger?.LogInformation("Finished printing");

            return new PrintResult { Succeeded = true };
        }

        public PrintResult Print(IPrintObject data)
        {
            logger?.LogInformation("String to print");
            OnPrePrint(new PrePrintEventArgs("Pre Print string"));

            Console.WriteLine(data.Data);

            OnPostPrint(new PostPrintEventArgs("Post Print string"));
            logger?.LogInformation("Finished printing");

            return new PrintResult { Succeeded = true };
        }

        public async Task<PrintResult> AsyncPrint()
        {
            await Task.Delay(500);
            logger?.LogInformation("Async pre log message");
            OnPrePrint(new PrePrintEventArgs("async Pre Print string"));

            Console.WriteLine("Async Hello World!");

            OnPostPrint(new PostPrintEventArgs("async Post Print string"));
            logger?.LogInformation("Async post log message");

            return new PrintResult { Succeeded = true };

        }

        public async Task<PrintResult> AsyncPrintWillThrow()
        {
            await Task.Delay(500);
            
            throw new System.Exception("Throwing an exception in a Task");
        }
    }
}
