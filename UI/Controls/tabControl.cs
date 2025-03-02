using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tester.Core;
using Tester.Core.Models;
using Tester.Services;
using FastColoredTextBoxNS;
using Tester.Core.Triggers.fctb_triggers;

namespace Tester.UI.Forms
{
    public partial class tabControl : UserControl
    {
        // ToolTip işleyicilerini saklamak için bir Dictionary tanımlayın
        Dictionary<int, EventHandler<ToolTipNeededEventArgs>> toolTipHandlers = new Dictionary<int, EventHandler<ToolTipNeededEventArgs>>();
        List<ExplorerItem> explorerList = new List<ExplorerItem>();
        public FCTB_Triggers triggers = new FCTB_Triggers();

        AutocompleteService autocompleteService = new AutocompleteService();
        FileService fileService = new FileService();
        Color changedLineColor = Color.FromArgb(255, 230, 230, 255);

        public ToolStripStatusLabel lbWordUnderMouse;
        public ListView errorList;

        private Style sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray)));

        public tabControl()
        {
            this.triggers.lbWordUnderMouse = lbWordUnderMouse;
            InitializeComponent();
            this.triggers.tsFiles = tsFiles;
            this.triggers.errorList = errorList;
        }





        FastColoredTextBox CurrentTB
        {
            get
            {
                if (tsFiles.SelectedItem == null)
                    return null;
                return (tsFiles.SelectedItem.Controls[0] as FastColoredTextBox);
            }

            set
            {
                tsFiles.SelectedItem = (value.Parent as FATabStripItem);
                value.Focus();
            }
        }

        public FastColoredTextBox getCurrentTB()
        {
            return CurrentTB;
        }

        public FATabStrip GetFATabStrip()
        {
            return tsFiles;
        }

        private async void tsFiles_TabStripItemClosing(FarsiLibrary.Win.TabStripItemClosingEventArgs e)
        {
            try
            {
                if (e.Item.Controls[0] is FastColoredTextBox tb && tb.IsChanged)
                {
                    var result = MessageBox.Show("Do you want to save " + e.Item.Title + "?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        bool saveVal = await fileService.Save(e.Item,sfdMain,CurrentTB.Tag.ToString());

                        if (!saveVal)
                        {
                            toolTipHandlers.Clear();

                            e.Cancel = true;
                            return;
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        toolTipHandlers.Clear();

                        e.Cancel = true;
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tsFiles_TabStripItemSelectionChanged(FarsiLibrary.Win.TabStripItemChangedEventArgs e)
        {
            if (CurrentTB != null)
            {
                //errorList.Items.Clear();

                CurrentTB.Focus();
                string text = CurrentTB.Text;
                ThreadPool.QueueUserWorkItem(
                    (o) => ReBuildObjectExplorer(text)
                );
            }
        }

        private void ReBuildObjectExplorer(string text)
        {
            try
            {
                List<ExplorerItem> list = new List<ExplorerItem>();
                int lastClassIndex = -1;
                //find classes, methods and properties
                Regex regex = new Regex(@"^(?<range>[\w\s]+\b(class|struct|enum|interface)\s+[\w<>,\s]+)|^\s*(public|private|internal|protected)[^\n]+(\n?\s*{|;)?", RegexOptions.Multiline);
                foreach (Match r in regex.Matches(text))
                    try
                    {
                        string s = r.Value;
                        int i = s.IndexOfAny(new char[] { '=', '{', ';' });
                        if (i >= 0)
                            s = s.Substring(0, i);
                        s = s.Trim();

                        var item = new ExplorerItem() { title = s, position = r.Index };
                        if (Regex.IsMatch(item.title, @"\b(class|struct|enum|interface)\b"))
                        {
                            item.title = item.title.Substring(item.title.LastIndexOf(' ')).Trim();
                            item.type = ExplorerItemType.Class;
                            list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), new ExplorerItemComparer());
                            lastClassIndex = list.Count;
                        }
                        else
                            if (item.title.Contains(" event "))
                        {
                            int ii = item.title.LastIndexOf(' ');
                            item.title = item.title.Substring(ii).Trim();
                            item.type = ExplorerItemType.Event;
                        }
                        else
                                if (item.title.Contains("("))
                        {
                            var parts = item.title.Split('(');
                            item.title = parts[0].Substring(parts[0].LastIndexOf(' ')).Trim() + "(" + parts[1];
                            item.type = ExplorerItemType.Method;
                        }
                        else
                                    if (item.title.EndsWith("]"))
                        {
                            var parts = item.title.Split('[');
                            if (parts.Length < 2) continue;
                            item.title = parts[0].Substring(parts[0].LastIndexOf(' ')).Trim() + "[" + parts[1];
                            item.type = ExplorerItemType.Method;
                        }
                        else
                        {
                            int ii = item.title.LastIndexOf(' ');
                            item.title = item.title.Substring(ii).Trim();
                            item.type = ExplorerItemType.Property;
                        }
                        list.Add(item);
                    }
                    catch {; }

                list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), new ExplorerItemComparer());

                BeginInvoke(
                    new Action(() =>
                    {
                        explorerList = list;
                        //dgvObjectExplorer.RowCount = explorerList.Count;
                        //dgvObjectExplorer.Invalidate();
                    })
                );
            }
            catch {; }
        }

        public async Task CreateTab(string fileName)
        {
            try
            {
                var tb = new FastColoredTextBox();
                tb.Font = new Font("Consolas", 9.75f);
                tb.ContextMenuStrip = cmMain;
                tb.Dock = DockStyle.Fill;
                tb.BorderStyle = BorderStyle.Fixed3D;
                //tb.VirtualSpace = true;
                tb.LeftPadding = 17;
                tb.Language = Language.CSharp;
                tb.AddStyle(sameWordsStyle);//same words style
                var tab = new FATabStripItem(fileName != null ? Path.GetFileName(fileName) : "[new]", tb);
                tab.Tag = fileName;
                if (fileName != null)
                    tb.OpenFile(fileName);
                tb.Tag = new TbInfo();

                await Task.Delay(100);

                tsFiles.AddTab(tab);
                tsFiles.SelectedItem = tab;
                tb.Focus();
                tb.DelayedTextChangedInterval = 1000;
                tb.DelayedEventsInterval = 500;

                
                triggers.errorList = errorList;

                tb.TextChangedDelayed += new EventHandler<TextChangedEventArgs>(triggers.tb_TextChangedDelayed); // AKTİF EDİLECEK
                tb.SelectionChangedDelayed += new EventHandler(triggers.tb_SelectionChangedDelayed);
                tb.TextChanged += triggers.FastColoredTextBox_TextChanged;
                tb.KeyDown += new KeyEventHandler(triggers.tb_KeyDown);


                this.triggers.lbWordUnderMouse = lbWordUnderMouse;

                tb.MouseMove += new MouseEventHandler(triggers.tb_MouseMove);
                tb.ChangedLineColor = changedLineColor;


                //if (btHighlightCurrentLine.Checked)
                //    tb.CurrentLineColor = currentLineColor;
                //tb.ShowFoldingLines = btShowFoldingLines.Checked;
                tb.HighlightingRangeType = HighlightingRangeType.VisibleRange;
                //tool error show

                //create autocomplete popup menu
                AutocompleteMenu popupMenu = new AutocompleteMenu(tb);
                popupMenu.Items.ImageList = ilAutocomplete;
                popupMenu.MinFragmentLength = 1;
                popupMenu.Opening += new EventHandler<CancelEventArgs>(popupMenu_Opening);
                autocompleteService.BuildAutocompleteMenu(popupMenu);
                (tb.Tag as TbInfo).popupMenu = popupMenu;
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Retry)
                    await CreateTab(fileName);
            }
        }

        void popupMenu_Opening(object sender, CancelEventArgs e)
        {
            //---block autocomplete menu for comments
            //get index of green style (used for comments)
            var iGreenStyle = CurrentTB.GetStyleIndex(CurrentTB.SyntaxHighlighter.GreenStyle);
            if (iGreenStyle >= 0)
                if (CurrentTB.Selection.Start.iChar > 0)
                {
                    //current char (before caret)
                    var c = CurrentTB[CurrentTB.Selection.Start.iLine][CurrentTB.Selection.Start.iChar - 1];
                    //green Style
                    var greenStyleIndex = Range.ToStyleIndex(iGreenStyle);
                    //if char contains green style then block popup menu
                    if ((c.style & greenStyleIndex) != 0)
                        e.Cancel = true;
                }
        }

    }
}
