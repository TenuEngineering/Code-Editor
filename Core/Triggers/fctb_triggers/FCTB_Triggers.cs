using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using Tester.UI.Forms;
using System.Drawing;
using Tester.Core.Models;
using System.Text.RegularExpressions;
using System.Drawing;
using FastColoredTextBoxNS;

namespace Tester.Core.Triggers.fctb_triggers
{
    public class FCTB_Triggers
    {
        public FATabStrip tsFiles;
        RuleEngine ruleEngine = new RuleEngine();
        public Navigate navigate;
        public ListView errorList;

        // Eşleşmeleri ve mevcut konumu global değişkenlerle tutalım tbFind
        private List<Range> matchRanges = new List<Range>();
        bool tbFindChanged = false;
        private int currentMatchIndex = -1;
        int errorCounter = 0;

        public ToolStripStatusLabel lbWordUnderMouse;
        // Stil nesnesini bir kere oluşturun
        WavyLineStyle errorStyle = new WavyLineStyle(255, Color.Red);
        WavyLineStyle noErrorStyle = new WavyLineStyle(0, Color.Black);



        // !! ÖENMLİ DÜZENLE
        public async void tb_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(100);
            FastColoredTextBox tb = sender as FastColoredTextBox;
            var code = tb.Text;
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


            // code txt üzerine variable klasörünün altındaki değişknelerin txt lerinden değişkenleri code stringine ekle

            var syntaxChecker = new SyntaxChecker();


            //var result = syntaxChecker.CheckSyntax(code);
            //var syntaxTree = syntaxChecker.GetSyntaxTree(code);
            //syntaxChecker.CheckSyntaxIssues(syntaxTree);

            tb.Range.ClearStyle(errorStyle);
            tb.Range.ClearFoldingMarkers();
            errorList.Items.Clear();


            var errorPositions = syntaxChecker.GetSyntaxErrorPositions(code);

            if (errorPositions.Count > 0)
            {
                errorCounter = 1;
            }

            else
            {
                errorCounter = 0;

            }
            string[] a = new string[]
            {
                "Tür veya ad uzayı tanımı yada dosya sonu bekleniyor",
                "'else' bir deyim başlatamaz.",
                "Sözdizimi hatası, '(' bekleniyor",
                ") bekleniyor",
                "Geçersiz ifade terimi else",
                "; bekleniyor",
                "Geçersiz ifade terimi '{'"
            };
            //await ruleEngine.controlRules(sender);

            foreach (var error in errorPositions)
            {
                if (a.Contains(error.errorMessage))
                {

                }
                // Hatalı bölgeyi vurgulamak için range (aralık) oluştur
                var range = tb.GetRange(new Place(error.startColumn, error.startLine), new Place(error.endColumn, error.startLine));
                errorList.Items.Add(new ListViewItem(new string[] { "", $"{error.errorCode}", $"{error.errorMessage}", parentDirectoryName, $"{error.startLine + 1}" }));

                // Hatalı bölgeye stil ekle
                range.SetStyle(errorStyle);

                // Hataları MessageBox ile gösterebilir veya başka bir işlem yapabilirsiniz.
                //MessageBox.Show($"Hata Kodu: {error.errorCode}, Mesaj: {error.errorMessage}, Satır: {error.startLine + 1}");
            }

        }


        public async void FastColoredTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                
                // Önceki vurgulamayı temizle
                var tb = sender as FastColoredTextBox;

                // Tüm metni kontrol et
                string text = tb.Text;

                // Hatalı kısımları işaretlemek için stil
                var warningStyle = new FastColoredTextBoxNS.TextStyle(null, Brushes.Red, FontStyle.Underline);

                // Stil eklenmeden önce tüm stilleri temizle
                e.ChangedRange.ClearStyle(warningStyle);

                // Özel kuralların kontrolü
                await ruleEngine.CheckCustomRules(e.ChangedRange, tb);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async void tb_SelectionChangedDelayed(object sender, EventArgs e)
        {
            await Task.Delay(100);

            var tb = sender as FastColoredTextBox;
            //remember last visit time
            if (tb.Selection.IsEmpty && tb.Selection.Start.iLine < tb.LinesCount)
            {
                if (navigate.lastNavigatedDateTime != tb[tb.Selection.Start.iLine].LastVisit)
                {
                    tb[tb.Selection.Start.iLine].LastVisit = DateTime.Now;
                    navigate.lastNavigatedDateTime = tb[tb.Selection.Start.iLine].LastVisit;
                }
            }
            Style sameWordsStyle = new FastColoredTextBoxNS.MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray)));

            //highlight same words
            tb.VisibleRange.ClearStyle(sameWordsStyle);
            if (!tb.Selection.IsEmpty)
                return;//user selected diapason
            //get fragment around caret
            var fragment = tb.Selection.GetFragment(@"\w");
            string text = fragment.Text;
            if (text.Length == 0)
                return;
            //highlight same words
            Range[] ranges = tb.VisibleRange.GetRanges("\\b" + text + "\\b").ToArray();

            if (ranges.Length > 1)
                foreach (var r in ranges)
                    r.SetStyle(sameWordsStyle);
        }


        public async void tb_KeyDown(object sender, KeyEventArgs e)
        {
            await Task.Delay(100);


            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.OemMinus)
            {
                navigate.NavigateBackward();
                e.Handled = true;
            }


            if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.OemMinus)
            {
                navigate.NavigateForward();
                e.Handled = true;
            }


        }
        // !! Önemli düzenle
        public void tbFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            FastColoredTextBox tbFind = sender as FastColoredTextBox;

            if (e.KeyChar == '\r' && tbFind != null)
            {
                if (matchRanges.Count == 0) // Eğer eşleşmeler yoksa veya arama ilk defa yapılıyorsa
                {
                    Range r = tbFindChanged ? tbFind.Range.Clone() : tbFind.Selection.Clone();
                    tbFindChanged = false;
                    r.End = new Place(tbFind[tbFind.LinesCount - 1].Count, tbFind.LinesCount - 1);

                    // Arama desenini oluştur ve tüm metinde eşleşmeleri bul
                    Regex pattern = new Regex(Regex.Escape(tbFind.Text)); // Metni güvenli hale getir
                    var matches = pattern.Matches(tbFind.Text); // Eşleşmeleri al

                    // Eşleşmeleri range olarak listeye ekleyelim
                    matchRanges.Clear(); // Önceki eşleşmeleri temizle
                    foreach (Match match in matches)
                    {
                        int startIndex = match.Index;
                        int length = match.Length;
                        Place startPlace = tbFind.PositionToPlace(startIndex);
                        Place endPlace = tbFind.PositionToPlace(startIndex + length);
                        matchRanges.Add(new Range(tbFind, startPlace, endPlace)); // Eşleşmeyi ekle
                    }

                    if (matchRanges.Count == 0) // Eşleşme bulunamadıysa
                    {
                        MessageBox.Show("Not found.");
                        return;
                    }
                }

                // Sonraki eşleşmeye geç
                currentMatchIndex++;
                if (currentMatchIndex >= matchRanges.Count) // Eğer sona ulaştıysak
                {
                    MessageBox.Show("No more matches.");
                    currentMatchIndex = -1; // İlk başa dön
                    matchRanges.Clear(); // Eşleşmeleri temizle
                    return;
                }

                // Mevcut eşleşmeyi seçili hale getirme ve görünür yapma
                var foundRange = matchRanges[currentMatchIndex];
                foundRange.Inverse(); // Seçimi ters çevir
                tbFind.Selection = foundRange;
                tbFind.DoSelectionVisible();
            }
        }
        public async void tb_MouseMove(object sender, MouseEventArgs e)
        {

            var tb = sender as FastColoredTextBox;
            await Task.Delay(100);
            var place = tb.PointToPlace(e.Location);
            var r = new Range(tb, place, place);

            string text = r.GetFragment("[a-zA-Z]").Text;
            lbWordUnderMouse.Text = text;

        }


    }
}
