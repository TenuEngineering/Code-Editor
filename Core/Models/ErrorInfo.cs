using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECUCodeEditor.Core.Models
{
    public class ErrorInfo
    {
        public int LineNumber { get; set; }
        public int ErrorStartIndex { get; set; }
        public int ErrorEndIndex { get; set; }
        public string ErrDescription { get; set; }
    }
}
