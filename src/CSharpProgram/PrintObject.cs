using StringPrinterSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpProgram
{
    internal class PrintObject : IPrintObject
    {
        public string Data => "Default to print";
    }
}
