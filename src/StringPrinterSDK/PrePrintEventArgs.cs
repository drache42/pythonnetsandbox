using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringPrinterSDK
{
    public class PrePrintEventArgs : EventArgs
    {
        public string PrePrintString { get; set; }

        public PrePrintEventArgs(string _prePrintString)
        {
            PrePrintString = _prePrintString;
        }
    }
}
