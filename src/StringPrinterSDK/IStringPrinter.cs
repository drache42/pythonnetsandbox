using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringPrinterSDK
{
    public interface IStringPrinter
    {
        event EventHandler<PrePrintEventArgs>? PrePrint;
        event EventHandler<PostPrintEventArgs>? PostPrint;

        PrintResult Print();
        PrintResult Print(IPrintObject data);

        Task<PrintResult> AsyncPrint();

        Task<PrintResult> AsyncPrintWillThrow();
    }
}
