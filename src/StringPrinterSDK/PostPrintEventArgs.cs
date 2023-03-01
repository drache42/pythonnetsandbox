using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringPrinterSDK
{
    public class PostPrintEventArgs : EventArgs
    {
        public string PostPrintString { get; set; }

        public PostPrintEventArgs(string _postPrintString)
        {
            PostPrintString = _postPrintString;
        }
    }
}
