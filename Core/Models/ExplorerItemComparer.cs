using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECUCodeEditor.Core.Models
{
    class ExplorerItemComparer : IComparer<ExplorerItem>
    {
        public int Compare(ExplorerItem x, ExplorerItem y)
        {
            return x.title.CompareTo(y.title);
        }
    }

}
