using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Tester.Core.Models;
using static Tester.PowerfulCSharpEditor;

namespace Tester.Core
{
    public class RuleEngine
    {
        ToolTipHandlers tooltip = new ToolTipHandlers();

        WavyLineStyle errorStyle = new WavyLineStyle(255, Color.Red);
        WavyLineStyle noErrorStyle = new WavyLineStyle(0, Color.Black);
        int errorCounter = 0;

        Dictionary<int, ErrorInfo> errorInfoDict = new Dictionary<int, ErrorInfo>();
        Dictionary<int, EventHandler<ToolTipNeededEventArgs>> toolTipHandlers = new Dictionary<int, EventHandler<ToolTipNeededEventArgs>>();

        public List<ErrorInfo> CheckRules(string code, string ruleFilePath)
        {
            var errors = new List<ErrorInfo>();
            var rules = File.ReadAllLines(ruleFilePath);

            foreach (var rule in rules)
            {
                var parts = rule.Split(':');
                if (parts.Length < 2) continue;

                var pattern = parts[1].Trim();
                var description = parts.Length > 2 ? parts[2].Trim() : "Unknown error";

                var regex = new Regex(pattern);
                var matches = regex.Matches(code);

                foreach (Match match in matches)
                {
                    errors.Add(new ErrorInfo
                    {
                        LineNumber = GetLineNumber(code, match.Index),
                        ErrorStartIndex = match.Index,
                        ErrorEndIndex = match.Index + match.Length,
                        ErrDescription = description
                    });
                }
            }

            return errors;
        }

        //kısıtlama ve kontroller
        public async Task CheckCustomRules(Range range, FastColoredTextBox fctb)
        {
            try
            {
                // Range içindeki metni al
                string text = range.Text;
                string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.None);
                await Task.Delay(100);

                foreach (var line in lines)
                {

                    if (line.Contains("if"))
                    {
                        // 'if' ifadesinden sonraki kısmı al
                        string ifCondition = line.Split(new[] { "if" }, StringSplitOptions.None)[1];

                        // '&&' ve '||' operatörlerini kullanarak koşulları say
                        int conditionCount = ifCondition.Split(new[] { "&&", "||" }, StringSplitOptions.None).Length - 1;

                        if (conditionCount > 1)
                        {

                            // Hata mesajı göster
                            MessageBox.Show("An 'if' statement can contain a maximum of 2 conditions.", "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // İmlecin bulunduğu satırı al
                            // İmlecin bulunduğu satırı al
                            int currentLine = fctb.Selection.Start.iLine;

                            // Satırdaki metni al
                            string lineText = fctb[currentLine].Text;

                            // Eğer satır boş değilse
                            if (!string.IsNullOrEmpty(lineText))
                            {

                                int sayac = 0;

                                // Satırın sonundaki karakterin pozisyonunu bul
                                for (int i = 0; i < lineText.Length; i++)
                                {
                                    int lastCharIndex = lineText.Length - i;

                                    // Silinecek karakterin başlangıç ve bitiş pozisyonlarını ayarla
                                    var startPlace = new Place(lastCharIndex - 1, currentLine);
                                    var endPlace = new Place(lastCharIndex, currentLine);

                                    // Seçim yap ve silme işlemini gerçekleştir
                                    fctb.Selection = new Range(fctb, startPlace, endPlace);

                                    if (fctb.SelectedText.Contains('|') || fctb.SelectedText.Contains('&'))
                                    {
                                        sayac++;
                                        fctb.SelectedText = string.Empty;
                                    }

                                    if (sayac > 1)
                                    {
                                        break;
                                    }

                                }

                            }
                        }
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task controlRules(object sender)
        {
            await Task.Delay(100);
            FastColoredTextBox tb = sender as FastColoredTextBox;

            // Önce tüm stilleri temizle
            tb.Range.ClearStyle(errorStyle);
            tb.Range.ClearFoldingMarkers();
            string cardName = Path.Combine("DOSYALAR", fullCardName);
            // Yeni path'i oluştur
            string kartFilePath = Path.Combine(cardName, "Rules");


            try
            {
                DirectoryInfo d = new DirectoryInfo(kartFilePath);

                if (!d.Exists)
                {
                    MessageBox.Show("Belirtilen dizin mevcut değil.");
                    return;
                }

                var fileNames = d.GetFiles("*.txt").Select(f => f.Name).ToList();
                string pattern = "";
                string pattern2 = "";
                string stopIndex = "";
                string errDescriptions = "";
                string hata_nedeni = "";

                errorList.Items.Clear();
                errorInfoDict.Clear();
                toolTipHandlers.Clear();

                int lineNumber = 0;
                errorCounter = 0;

                string filePath = tb.Tag as string;
                string parentDirectoryName = "";

                if (!string.IsNullOrEmpty(filePath))
                {
                    DirectoryInfo directoryInfo = Directory.GetParent(filePath);
                    if (directoryInfo != null)
                    {
                        parentDirectoryName = directoryInfo.Parent?.Name;
                    }
                }
                else
                {
                    var tab = tb.Parent as FATabStripItem;
                    if (tab != null)
                    {
                        filePath = tab.Tag as string;
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            DirectoryInfo directoryInfo = Directory.GetParent(filePath);
                            if (directoryInfo != null)
                            {
                                parentDirectoryName = directoryInfo.Parent?.Name;
                            }
                        }
                    }
                }


                foreach (string item2 in tb.Lines)
                {
                    lineNumber++;
                    if (string.IsNullOrEmpty(item2))
                    {
                        continue;
                    }


                    foreach (var item in fileNames)
                    {
                        string file = Path.Combine(kartFilePath, item);
                        if (File.Exists(file))
                        {

                            string[] lines = File.ReadAllLines(file);
                            try
                            {
                                foreach (string line in lines)
                                {
                                    var parts = line.Split(':');


                                    string getPatternInfo = parts[0].Trim();
                                    string getPattern = parts[1].Trim();

                                    if (getPatternInfo == "startRegex_index")
                                    {
                                        pattern = getPattern;
                                    }
                                    else if (getPatternInfo == "rulesRegex")
                                    {
                                        pattern2 = getPattern;
                                    }
                                    else if (getPatternInfo == "description")
                                    {
                                        errDescriptions = getPattern;
                                    }
                                    else if (getPatternInfo == "hata_nedeni")
                                    {
                                        hata_nedeni = getPattern;
                                    }
                                    else
                                    {
                                        stopIndex = getPattern;
                                    }


                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }

                            Regex regex = new Regex(pattern);
                            Match match = regex.Match(item2);




                            if (match.Success)
                            {

                                Regex regex2 = new Regex(pattern2);
                                Match match2 = regex2.Match(item2);


                                if (!match2.Success)
                                {
                                    int errorStartIndex = match.Index;
                                    int errorEndIndex = item2.IndexOf(stopIndex, errorStartIndex);

                                    if (errorEndIndex == -1)
                                    {
                                        errorEndIndex = item2.Length;
                                    }
                                    else
                                    {
                                        errorEndIndex += stopIndex.Length;
                                    }

                                    // Doğru range alındığından emin olmak için kontrol edelim
                                    if (errorStartIndex < errorEndIndex)
                                    {
                                        var range = tb.GetRange(new Place(errorStartIndex, lineNumber - 1), new Place(errorEndIndex, lineNumber - 1));
                                        // Hata bulunduğunda
                                        if (errorInfoDict == null)
                                        {
                                            errorInfoDict = new Dictionary<int, ErrorInfo>();

                                        }

                                        ErrorInfo errorInfo = new ErrorInfo
                                        {
                                            LineNumber = lineNumber,
                                            ErrorStartIndex = errorStartIndex,
                                            ErrorEndIndex = errorEndIndex,
                                            ErrDescription = errDescriptions
                                        };

                                        if (!errorInfoDict.ContainsKey(lineNumber))
                                        {
                                            errorInfoDict.Add(lineNumber, errorInfo);
                                            // Satır numarasına göre sözlüğe ekleyin
                                            errorInfoDict[lineNumber] = errorInfo;
                                        }

                                        range.SetStyle(errorStyle);

                                        if (!toolTipHandlers.ContainsKey(lineNumber))
                                        {
                                            // ToolTip işleyicisini oluştur ve satır numarasıyla birlikte Dictionary'e ekle
                                            EventHandler<ToolTipNeededEventArgs> toolTipHandler = (s, args) =>
                                            {
                                                tooltip.ToolTipHandler(s, args, errorInfoDict);
                                            };

                                            if (toolTipHandlers == null)
                                            {

                                                toolTipHandlers = new Dictionary<int, EventHandler<ToolTipNeededEventArgs>>();
                                            }

                                            toolTipHandlers.Add(lineNumber, toolTipHandler);  // Yeni handler'ı ekle
                                            tb.ToolTipNeeded += toolTipHandler;
                                            toolTipHandlers[lineNumber] = toolTipHandler; // Satır numarasına göre sakla

                                            errorCounter++;
                                            errorList.Items.Add(new ListViewItem(new string[] { "", "C205", $"{errDescriptions}", parentDirectoryName, $"{lineNumber}" }));
                                        }


                                        break;
                                    }

                                }
                                else
                                {
                                    if (errorCounter > 0)
                                    {
                                        errorCounter--;
                                    }

                                    // Hata yoksa, eğer bu satır için bir ToolTip varsa kaldır
                                    if (toolTipHandlers.ContainsKey(lineNumber))
                                    {
                                        tb.ToolTipNeeded -= toolTipHandlers[lineNumber]; // ToolTip'i kaldır
                                        toolTipHandlers.Remove(lineNumber);  // Dictionary'den işleyiciyi çıkar
                                    }
                                    break;

                                }

                            }


                        }
                        else
                        {
                            MessageBox.Show("Dosya bulunamadı.");
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu3: " + ex.Message);
            }
        }

        private int GetLineNumber(string code, int index)
        {
            return code.Take(index).Count(c => c == '\n') + 1;
        }
    }

}
