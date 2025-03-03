using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECUCodeEditor.Core.Models;

namespace ECUCodeEditor.Core
{
    internal class ToolTipHandlers
    {
        public void ToolTipHandler(object sender, ToolTipNeededEventArgs args, Dictionary<int, ErrorInfo> errorInfoDict)
        {
            //FastColoredTextBox tb = sender as FastColoredTextBox;

            //var range = tb.GetRange(new Place(errorStartIndex, lineNumber - 1), new Place(errorEndIndex, lineNumber - 1));
            //MessageBox.Show("satır numarası : " + lineNumber + "\nhata açıklaması : " + errDescriptions);

            //args.ToolTipTitle = "Hata";
            //args.ToolTipText = $"{errDescriptions}";


            FastColoredTextBox tb = sender as FastColoredTextBox;

            // Satır numarasını belirleyin
            int lineNumber2 = args.Place.iLine + 1;  // args.Place.iLine sıfırdan başlar, bu yüzden 1 ekliyoruz

            // Eğer o satır için bir hata bilgisi varsa
            if (errorInfoDict.ContainsKey(lineNumber2))
            {
                var errorInfo = errorInfoDict[lineNumber2];

                var range = tb.GetRange(new Place(errorInfo.ErrorStartIndex, errorInfo.LineNumber - 1), new Place(errorInfo.ErrorEndIndex, errorInfo.LineNumber - 1));

                // Eğer imleç bu range içinde ise, tooltip'i göster
                if (range.Contains(args.Place))
                {
                    args.ToolTipTitle = "Hata";
                    args.ToolTipText = errorInfo.ErrDescription;
                }
            }



        }
    }

}
