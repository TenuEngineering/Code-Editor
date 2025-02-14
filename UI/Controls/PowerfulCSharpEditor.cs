using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using System.Security.Cryptography;
using System.DirectoryServices.ActiveDirectory;
using Microsoft.Build.Tasks;
using System.Drawing.Drawing2D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO.Ports;
using System.Reflection.Metadata.Ecma335;
using System.Collections;
using Tester.Core;
using Tester.UI.Forms;
using Tester.Core.Models;
using Tester.Services;
using AutocompleteMenuNS;

namespace Tester
{
    // deneme
    // deneme
    public partial class PowerfulCSharpEditor : Form
    {
        private FileService _fileService;
        private SyntaxChecker _syntaxChecker;
        private AutocompleteService _autocompleteService;
        private TreeViewService _treeViewService;
        private RuleEngine _ruleEngine;

        Navigate navigate = new Navigate();

        string[] keywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield" };
        string[] methods = { "Equals()", "GetHashCode()", "GetType()", "ToString()" };
        string[] snippets = { "if(^)\n{\n}", "if(^)\n{\n}\nelse\n{\n}", "for(^;;)\n{\n}", "while(^)\n{\n}", "do\n{\n^;\n}while();", "switch(^)\n{\ncase : break;\n}" };
        string[] declarationSnippets = {
               "public class ^\n{\n}", "private class ^\n{\n}", "internal class ^\n{\n}",
               "public struct ^\n{\n;\n}", "private struct ^\n{\n;\n}", "internal struct ^\n{\n;\n}",
               "public void ^()\n{\n;\n}", "private void ^()\n{\n;\n}", "internal void ^()\n{\n;\n}", "protected void ^()\n{\n;\n}",
               "public ^{ get; set; }", "private ^{ get; set; }", "internal ^{ get; set; }", "protected ^{ get; set; }"
               };


        Dictionary<string, int> format = new Dictionary<string, int>()
        {
            {"enable_pin",100},
            {"input.digital.active_high",224 },
            {"input.digital.active_low",192 },
            {"input.analog",128 },
            {"output.high.continues",96 },
            {"output.high.timed",64 },
            {"output.low.continues",32 },
            {"output.low.timed",0 },
        };
        Dictionary<string, int> pinNumber = new Dictionary<string, int>()
        {
            {"a1",0},
            {"a2",1 },
            {"a3",2 },
            {"a4",3 },
            {"a5",4 },
            {"a6",5 },
            {"a7",6 },
        };
        Dictionary<string, int> typeTotalByte = new Dictionary<string, int>()
        {
            {"char",1},
            {"int",2},
            {"long",4 },
            {"float",4 },
        };

        // ToolTip işleyicilerini saklamak için bir Dictionary tanımlayın
        Dictionary<int, EventHandler<ToolTipNeededEventArgs>> toolTipHandlers = new Dictionary<int, EventHandler<ToolTipNeededEventArgs>>();

        List<string> charVariable = new List<string>();
        List<string> intVariable = new List<string>();
        List<string> longVariable = new List<string>();
        List<string> floatVariable = new List<string>();
        List<string> doubleVariable = new List<string>();
        List<string> arrayVariable = new List<string>();
        List<string> userFlagVariable = new List<string>();

        List<string> systemVariable = new List<string>();
        List<string> systemFlagVariable = new List<string>();

        int errorCounter = 0;
        const int totalVariableSize = 15; //byte cinsinden total size

        //combox taki değişken türü değişmeden önceki değerini almak için
        private string previousComboBoxValue;
        private string previousVarName;
        private int oldTypeSize;
        private bool isDataGridViewChanged = false;
        private bool isUpdating = false; // Global bir değişken, güncellemenin gerçekleşip gerçekleşmediğini izler

        // Eşleşmeleri ve mevcut konumu global değişkenlerle tutalım tbFind
        private List<Range> matchRanges = new List<Range>();
        private int currentMatchIndex = -1;

        //{"input",1},
        //{"output",0},
        //{ "digital",1},
        //{ "analog",0},
        //{ "high",1},
        //{ "low",0},
        //{ "continues",1},
        //{ "timed",0},
        //{ "active_high",1},
        //{ "active_low",0},
 


        string workspaceFullPath;
        string usersAllVariablePath;
        string systemAllVariablePath;

        static string[] sources = new string[]{

        };

        static string[] variableAutoComplate = new string[]{

        };

        Style invisibleCharsStyle = new InvisibleCharsRenderer(Pens.Gray);
        Color currentLineColor = Color.FromArgb(100, 210, 210, 255);
        Color changedLineColor = Color.FromArgb(255, 230, 230, 255);

        // Stil nesnesini bir kere oluşturun
        WavyLineStyle errorStyle = new WavyLineStyle(255, Color.Red);
        WavyLineStyle noErrorStyle = new WavyLineStyle(0, Color.Black);

        string fullCardName;
        List<string> fileNames;

        string activeProjectPath;
        int controlCounter = 0;
        string rulesPath;
        tabControl tsFiles = new tabControl();
        
        public PowerfulCSharpEditor()
        {
            InitializeComponent();
            InitializeContextMenu();
            //init menu images
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PowerfulCSharpEditor));
            copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripButton.Image")));
            cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripButton.Image")));
            pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripButton.Image")));
            _fileService = new FileService();
            _syntaxChecker = new SyntaxChecker();
            _autocompleteService = new AutocompleteService();
            _treeViewService = new TreeViewService();
            _ruleEngine = new RuleEngine();

        }


        public PowerfulCSharpEditor(string workspaceFullPath)
        {
            InitializeComponent();
            InitializeContextMenu();
            //init menu images
            this.KeyPreview = true;
            this.workspaceFullPath = workspaceFullPath;

            
            //MessageBox.Show(workspaceFullPath + "\n\n\n" + usersAllVariablePath);

            // Diziyi List<T> yapısına dönüştür


            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PowerfulCSharpEditor));
            copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripButton.Image")));
            cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripButton.Image")));
            pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripButton.Image")));


        }

        private async void PowerfulCSharpEditor_Load(object sender, EventArgs e)
        {
            await InitializeTreeView();

            tsFiles.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(tsFiles);
            //  LoadProjectContents();
            //await LoadDirectory(this.workspaceFullPath);

        }


        private async Task LoadProjectContents()
        {
            if (Directory.Exists(workspaceFullPath))
            {
                await PopulateTreeView(workspaceFullPath, treeView1.Nodes);
            }
        }
        private async Task PopulateTreeView(string dirPath, TreeNodeCollection nodes)
        {
            var directoryInfo = new DirectoryInfo(dirPath);
            var rootNode = new TreeNode(directoryInfo.Name)
            {
                Tag = directoryInfo.FullName
            };
            nodes.Add(rootNode);
            await PopulateSubTree(directoryInfo, rootNode);
        }

        private async Task PopulateSubTree(DirectoryInfo directoryInfo, TreeNode nodeToAddTo)
        {
            foreach (var directory in directoryInfo.GetDirectories())
            {
                var directoryNode = new TreeNode(directory.Name)
                {
                    Tag = directory.FullName
                };
                nodeToAddTo.Nodes.Add(directoryNode);
                await PopulateSubTree(directory, directoryNode);
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                var fileNode = new TreeNode(file.Name)
                {
                    Tag = file.FullName
                };

                if (file.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    fileNode.ForeColor = Color.Blue;
                }

                nodeToAddTo.Nodes.Add(fileNode);
            }
        }

        private async void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await tsFiles.CreateTab(null);
        }


        private async Task LoadSubDirectories(string dir, TreeNode node)
        {
            try
            {
                string[] subdirectoryEntries = Directory.GetDirectories(dir);
                foreach (string subdirectory in subdirectoryEntries)
                {
                    DirectoryInfo di = new DirectoryInfo(subdirectory);
                    TreeNode tds = node.Nodes.Add(di.Name);
                    tds.Tag = di.FullName;
                    tds.ImageIndex = 0; // Klasör için 03.png indeksi (0)
                    tds.SelectedImageIndex = 0; // Seçili klasör için 03.png indeksi (0)
                    await LoadSubDirectories(subdirectory, tds);
                }

                string[] files = Directory.GetFiles(dir);
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    TreeNode tds = node.Nodes.Add(fi.Name);
                    tds.Tag = fi.FullName;
                    if (fi.Extension.ToLower() == ".txt")
                    {
                        tds.ImageIndex = 1; // .txt dosyaları için 04.png indeksi (1)
                        tds.SelectedImageIndex = 1; // Seçili .txt dosyaları için 04.png indeksi (1)
                    }
                    else
                    {
                        tds.ImageIndex = 1; // Diğer dosyalar için de 04.png indeksi (1) kullanabilirsiniz
                        tds.SelectedImageIndex = 1;
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Proje artık mevcut değil");
            }
            
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createProject yn = new createProject();
            yn.Show();
        }


        private async Task InitializeTreeView()
        {
            // ImageList kontrolünü formunuza ekleyin
            ImageList imageList = new ImageList();
            await Task.Delay(100);

            // RESIMLER klasöründeki ikon dosyalarını yükleyin
            string baseIconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RESIMLER");

            // Folder and file icons for general usage
            string folderIconPath = Path.Combine(baseIconPath, "folder.png");
            string fileIconPath = Path.Combine(baseIconPath, "codeW.png");

            // Custom icons for "Functions" and "Variable" directories
            string fonksiyonlarIconPath = Path.Combine(baseIconPath, "fonksiyonlar.png");
            string variableIconPath = Path.Combine(baseIconPath, "variable.png");

            // Add icons to ImageList if the files exist
            if (File.Exists(folderIconPath))
            {
                imageList.Images.Add("folder", Image.FromFile(folderIconPath));
            }
            else
            {
                MessageBox.Show("Folder icon not found.");
            }

            if (File.Exists(fileIconPath))
            {
                imageList.Images.Add("codeW", Image.FromFile(fileIconPath));
            }
            else
            {
                MessageBox.Show("File icon not found.");
            }

            if (File.Exists(fonksiyonlarIconPath))
            {
                imageList.Images.Add("fonksiyonlar", Image.FromFile(fonksiyonlarIconPath));
            }

            if (File.Exists(variableIconPath))
            {
                imageList.Images.Add("variable", Image.FromFile(variableIconPath));
            }

            // TreeView kontrolüne ImageList'i atayın
            treeView1.ImageList = imageList;

            // Load the tree structure with custom icons
            await LoadDirectoryWithIcons(workspaceFullPath, treeView1.Nodes);
        }

        // Method to load directory and apply folder-specific icons
        private async Task LoadDirectoryWithIcons(string directoryPath, TreeNodeCollection nodes)
        {
            //MessageBox.Show(directoryPath);

            var directoryInfo = new DirectoryInfo(directoryPath);
            var rootNode = new TreeNode(directoryInfo.Name)
            {
                Tag = directoryInfo.FullName,
                ImageKey = "folder", // Default folder icon
                SelectedImageKey = "folder"
            };
            nodes.Add(rootNode);
            await LoadSubDirectoriesWithIcons(directoryInfo, rootNode);
        }

        // Method to load subdirectories and assign custom icons
        private async Task LoadSubDirectoriesWithIcons(DirectoryInfo directoryInfo, TreeNode parentNode)
        {
            foreach (var directory in directoryInfo.GetDirectories())
            {
                var directoryNode = new TreeNode(directory.Name)
                {
                    Tag = directory.FullName,
                    ImageKey = "folder",  // Default folder icon
                    SelectedImageKey = "folder"
                };
                parentNode.Nodes.Add(directoryNode);
                await LoadSubDirectoriesWithIcons(directory, directoryNode);
            }

            foreach (var file in directoryInfo.GetFiles("*.txt"))
            {
                var fileNode = new TreeNode(file.Name)
                {
                    Tag = file.FullName,
                    ImageKey = GetCustomIconKey(file.DirectoryName),  // Assign folder-specific icons
                    SelectedImageKey = GetCustomIconKey(file.DirectoryName)
                };
                parentNode.Nodes.Add(fileNode);
            }
        }

        // Method to return the appropriate icon based on folder name
        private string GetCustomIconKey(string directory)
        {
            if (directory.Contains("Functions"))
            {
                return "functions";  // Custom icon for Functions folder
            }
            else if (directory.Contains("Variable"))
            {
                return "variable";  // Custom icon for Variable folder
            }
            else
            {
                return "codeW";  // Default file icon
            }
        }

        private async Task HighlightInvisibleChars(Range range)
        {
            await Task.Delay(100);
            range.ClearStyle(invisibleCharsStyle);
            if (btInvisibleChars.Checked)
                range.SetStyle(invisibleCharsStyle, @".$|.\r\n|\s");
        }

        private async void errorList_DoubleClick(object sender, MouseEventArgs e)
        {
            if (errorList.SelectedItems.Count > 0)
            {
                var selectedItem = errorList.SelectedItems[0];

                // ListView'deki satır numarasını al
                if (int.TryParse(selectedItem.SubItems[4].Text, out int lineNumber))
                {
                    // Aktif FastColoredTextBox'ı bul
                    FastColoredTextBox tb = tsFiles.getCurrentTB();
                    if (tb != null)
                    {
                        // Satırı vurgula
                        await HighlightLine(lineNumber - 1, tb); // Satır numarası 0 tabanlı olduğu için -1
                    }
                }
            }
        }
        
        private async Task HighlightLine(int lineNumber, FastColoredTextBox tb)
        {
            await Task.Delay(100);
            if (lineNumber >= 0 && lineNumber < tb.LinesCount)
            {
                // Satırın başlangıç ve bitiş yerlerini belirleyin
                var startPlace = new Place(0, lineNumber);
                var endPlace = new Place(tb.Lines[lineNumber].Length, lineNumber);

                // Satırı seçin
                tb.Selection.Start = startPlace;
                tb.Selection.End = endPlace;

                // Seçimi görünür yap
                tb.DoSelectionVisible();

                // Seçimi vurgulamak için stil ayarı
                tb.SelectionColor = Color.Blue;

                // Ekranı yeniden çiz
                tb.Refresh();
            }
            else
            {
                MessageBox.Show("Geçersiz satır numarası.");
            }
        }



        List<ExplorerItem> explorerList = new List<ExplorerItem>();

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
                            dgvObjectExplorer.RowCount = explorerList.Count;
                            dgvObjectExplorer.Invalidate();
                        })
                );
            }
            catch {; }
        }

        // kuralları ayrı bir iş parçacığında kontrol et


        private async Task InitializeDataGridViewEvents(DataGridView dgv)
        {
            await Task.Delay(100);
            dgv.CellValueChanged += (s, e) => isDataGridViewChanged = true;
            dgv.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgv.IsCurrentCellDirty)
                {
                    dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
        }

        private async void tsFiles_TabStripItemClosing(TabStripItemClosingEventArgs e)
        {

            try
            {
                if (e.Item.Controls[0] is FastColoredTextBox tb && tb.IsChanged)
                {
                    var result = MessageBox.Show("Do you want to save " + e.Item.Title + "?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        bool saveVal = await _fileService.Save(e.Item,sfdMain,activeProjectPath);

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


        private async void saveToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            if (tsFiles != null)
            {
                await _fileService.Save(tsFiles.getCurrentTB().Parent as FATabStripItem, sfdMain,activeProjectPath);
        
            }
        }
        private async void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsFiles != null)
            {
                bool saveVal = await _fileService.Save(tsFiles.getCurrentTB().Parent as FATabStripItem, sfdMain, activeProjectPath);

                string oldFile = tsFiles.getCurrentTB().Tag as string;
                tsFiles.getCurrentTB().Tag = null;
                if (!saveVal)
                    if (oldFile != null)
                    {
                        tsFiles.getCurrentTB().Tag = oldFile;
                        //tsFiles.GetFATabStrip().Title = Path.GetFileName(oldFile);
                    }
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdMain.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                await tsFiles.CreateTab(ofdMain.FileName);
        }


        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsFiles.getCurrentTB().Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsFiles.getCurrentTB().Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsFiles.getCurrentTB().Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsFiles.getCurrentTB().Selection.SelectAll();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsFiles.getCurrentTB().UndoEnabled)
                tsFiles.getCurrentTB().Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsFiles.getCurrentTB().RedoEnabled)
                tsFiles.getCurrentTB().Redo();
        }

        private void tmUpdateInterface_Tick(object sender, EventArgs e)
        {
            try
            {
                if (tsFiles.getCurrentTB() != null && tsFiles.GetFATabStrip().Items.Count > 0)
                {
                    var tb = tsFiles.getCurrentTB();
                    undoStripButton.Enabled = undoToolStripMenuItem.Enabled = tb.UndoEnabled;
                    redoStripButton.Enabled = redoToolStripMenuItem.Enabled = tb.RedoEnabled;
                    saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = tb.IsChanged;
                    saveAsToolStripMenuItem.Enabled = true;
                    pasteToolStripButton.Enabled = pasteToolStripMenuItem.Enabled = true;
                    cutToolStripButton.Enabled = cutToolStripMenuItem.Enabled =
                    copyToolStripButton.Enabled = copyToolStripMenuItem.Enabled = !tb.Selection.IsEmpty;
                    printToolStripButton.Enabled = true;
                }
                else
                {
                    saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = false;
                    saveAsToolStripMenuItem.Enabled = false;
                    cutToolStripButton.Enabled = cutToolStripMenuItem.Enabled =
                    copyToolStripButton.Enabled = copyToolStripMenuItem.Enabled = false;
                    pasteToolStripButton.Enabled = pasteToolStripMenuItem.Enabled = false;
                    printToolStripButton.Enabled = false;
                    undoStripButton.Enabled = undoToolStripMenuItem.Enabled = false;
                    redoStripButton.Enabled = redoToolStripMenuItem.Enabled = false;
                    dgvObjectExplorer.RowCount = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            if (tsFiles.getCurrentTB() != null)
            {
                var settings = new PrintDialogSettings();
                settings.Title = tsFiles.GetFATabStrip().SelectedItem.Title;
                settings.Header = "&b&w&b";
                settings.Footer = "&b&p";
                tsFiles.getCurrentTB().Print(settings);
            }
        }

        bool tbFindChanged = false;



        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsFiles.getCurrentTB().ShowFindDialog();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsFiles.getCurrentTB().ShowReplaceDialog();
        }

        private void PowerfulCSharpEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            int counter2 = 0;
            List<FATabStripItem> list = new List<FATabStripItem>();
            foreach (FATabStripItem tab in tsFiles.GetFATabStrip().Items)
                list.Add(tab);
            foreach (var tab in list)
            {
                counter2++;
                TabStripItemClosingEventArgs args = new TabStripItemClosingEventArgs(tab);
                tsFiles_TabStripItemClosing(args);
                if (args.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                tsFiles.GetFATabStrip().RemoveTab(tab);
            }
            if (counter2 == 0)
            {
                Application.Exit();
            }
        }


        private void dgvObjectExplorer_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (tsFiles.getCurrentTB() != null)
            {
                var item = explorerList[e.RowIndex];
                tsFiles.getCurrentTB().GoEnd();
                tsFiles.getCurrentTB().SelectionStart = item.position;
                tsFiles.getCurrentTB().DoSelectionVisible();
                tsFiles.getCurrentTB().Focus();
            }
        }
        
        private void dgvObjectExplorer_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                ExplorerItem item = explorerList[e.RowIndex];
                if (e.ColumnIndex == 1)
                    e.Value = item.title;
                else
                    switch (item.type)
                    {
                        case ExplorerItemType.Class:
                            e.Value = global::Tester.Properties.Resources.class_libraries;
                            return;
                        case ExplorerItemType.Method:
                            e.Value = global::Tester.Properties.Resources.box;
                            return;
                        case ExplorerItemType.Event:
                            e.Value = global::Tester.Properties.Resources.lightning;
                            return;
                        case ExplorerItemType.Property:
                            e.Value = global::Tester.Properties.Resources.property;
                            return;
                    }
            }
            catch {; }
        }

        private void tsFiles_TabStripItemSelectionChanged(TabStripItemChangedEventArgs e)
        {
            if (tsFiles.getCurrentTB() != null)
            {
                errorList.Items.Clear();

                tsFiles.getCurrentTB().Focus();
                string text = tsFiles.getCurrentTB().Text;
                ThreadPool.QueueUserWorkItem(
                    (o) => ReBuildObjectExplorer(text)
                );
            }
        }

        private void backStripButton_Click(object sender, EventArgs e)
        {
            navigate.NavigateBackward();
        }

        private void forwardStripButton_Click(object sender, EventArgs e)
        {
            navigate.NavigateForward();
        }

        DateTime lastNavigatedDateTime = DateTime.Now;





        private void autoIndentSelectedTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsFiles.getCurrentTB().DoAutoIndent();
        }

        private async void btInvisibleChars_Click(object sender, EventArgs e)
        {
            foreach (FATabStripItem tab in tsFiles.GetFATabStrip().Items)
                await HighlightInvisibleChars((tab.Controls[0] as FastColoredTextBox).Range);
            if (tsFiles.getCurrentTB() != null)
                tsFiles.getCurrentTB().Invalidate();
        }

        private void btHighlightCurrentLine_Click(object sender, EventArgs e)
        {
            foreach (FATabStripItem tab in tsFiles.GetFATabStrip().Items)
            {
                if (btHighlightCurrentLine.Checked)
                    (tab.Controls[0] as FastColoredTextBox).CurrentLineColor = currentLineColor;
                else
                    (tab.Controls[0] as FastColoredTextBox).CurrentLineColor = Color.Transparent;
            }
            if (tsFiles.getCurrentTB() != null)
                tsFiles.getCurrentTB().Invalidate();
        }

        private void commentSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsFiles.getCurrentTB().InsertLinePrefix("//");
        }

        private void uncommentSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsFiles.getCurrentTB().RemoveLinePrefix("//");
        }

        private void cloneLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //expand selection
            tsFiles.getCurrentTB().Selection.Expand();
            //get text of selected lines
            string text = Environment.NewLine + tsFiles.getCurrentTB().Selection.Text;
            //move caret to end of selected lines
            tsFiles.getCurrentTB().Selection.Start = tsFiles.getCurrentTB().Selection.End;
            //insert text
            tsFiles.getCurrentTB().InsertText(text);
        }

        private void cloneLinesAndCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //start autoUndo block
            tsFiles.getCurrentTB().BeginAutoUndo();
            //expand selection
            tsFiles.getCurrentTB().Selection.Expand();
            //get text of selected lines
            string text = Environment.NewLine + tsFiles.getCurrentTB().Selection.Text;
            //comment lines
            tsFiles.getCurrentTB().InsertLinePrefix("//");
            //move caret to end of selected lines
            tsFiles.getCurrentTB().Selection.Start = tsFiles.getCurrentTB().Selection.End;
            //insert text
            tsFiles.getCurrentTB().InsertText(text);
            //end of autoUndo block
            tsFiles.getCurrentTB().EndAutoUndo();
        }

        private void bookmarkPlusButton_Click(object sender, EventArgs e)
        {
            if (tsFiles.getCurrentTB() == null)
                return;
            tsFiles.getCurrentTB().BookmarkLine(tsFiles.getCurrentTB().Selection.Start.iLine);
        }

        private void bookmarkMinusButton_Click(object sender, EventArgs e)
        {
            if (tsFiles.getCurrentTB() == null)
                return;
            tsFiles.getCurrentTB().UnbookmarkLine(tsFiles.getCurrentTB().Selection.Start.iLine);
        }

        private void gotoButton_DropDownOpening(object sender, EventArgs e)
        {
            gotoButton.DropDownItems.Clear();
            foreach (Control tab in tsFiles.GetFATabStrip().Items)
            {
                FastColoredTextBox tb = tab.Controls[0] as FastColoredTextBox;
                foreach (var bookmark in tb.Bookmarks)
                {
                    var item = gotoButton.DropDownItems.Add(bookmark.Name + " [" + Path.GetFileNameWithoutExtension(tab.Tag as String) + "]");
                    item.Tag = bookmark;
                    item.Click += (o, a) =>
                    {
                        var b = (Bookmark)(o as ToolStripItem).Tag;
                        try
                        {
                            //tsFiles.getCurrentTB() = b.TB;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                        b.DoVisible();
                    };
                }
            }
        }

        private void btShowFoldingLines_Click(object sender, EventArgs e)
        {
            foreach (FATabStripItem tab in tsFiles.GetFATabStrip().Items)
                (tab.Controls[0] as FastColoredTextBox).ShowFoldingLines = btShowFoldingLines.Checked;
            if (tsFiles.getCurrentTB() != null)
                tsFiles.getCurrentTB().Invalidate();
        }

        private void Zoom_click(object sender, EventArgs e)
        {
            if (tsFiles.getCurrentTB() != null)
                tsFiles.getCurrentTB().Zoom = int.Parse((sender as ToolStripItem).Tag.ToString());
        }


        // This event handler manually raises the CellValueChanged event 
        // by calling the CommitEdit method. 


        private TreeNode rightClickedNode;
        private async void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                // Seçilen düğümü al
                TreeNode selectedNode = treeView1.GetNodeAt(e.X, e.Y);

                string originalPath = selectedNode.Tag.ToString();

                // Orijinal path'in dizin yolunu al
                string directoryPath = Path.GetDirectoryName(originalPath);

                // Yeni dizin adı ve dosya adı
                string newDirectoryName = "SYSTEM_DATA";
                string newFileName = "INFO.txt";


                // USER_DATA dizinini yeni dizin adı ile değiştir
                string newDirectoryPath = Path.Combine(Path.GetDirectoryName(directoryPath), newDirectoryName);

                // etiketi olan kullanıcı değişkenlerini alır
                this.usersAllVariablePath = Path.Combine(Path.GetDirectoryName(directoryPath), @"USER_DATA\Variable");
                getValueTagForAutoComplate(this.usersAllVariablePath, true);

                // Yeni path'i oluştur
                string modifiedPath = Path.Combine(newDirectoryPath, newFileName);


                string cardName = "";
                string version = "";

                try
                {
                    string[] lines = new string[] { };

                    try
                    {
                        lines = File.ReadAllLines(modifiedPath);

                    }
                    catch (Exception)
                    {
                        directoryPath = Path.GetDirectoryName(directoryPath);

                        // USER_DATA dizinini yeni dizin adı ile değiştir
                        newDirectoryPath = Path.Combine(Path.GetDirectoryName(directoryPath), newDirectoryName);

                        // Yeni path'i oluştur
                        modifiedPath = Path.Combine(newDirectoryPath, newFileName);
                        lines = File.ReadAllLines(modifiedPath);


                    }


                    foreach (string line in lines)
                    {
                        if (line.StartsWith("KART ADI:"))
                        {
                            cardName = line.Split(':')[1];
                        }
                        else if (line.StartsWith("VERSIYON:"))
                        {
                            version = line.Split(':')[1];
                        }
                    }

                    // Değerleri birleştir
                    fullCardName = $"{cardName}_{version}";

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu : " + ex.Message);
                    fullCardName = string.Empty;
                }


                //string kartFilePath = $"DOSYALAR/{fullCardName}/Functions";
                //MessageBox.Show(kartFilePath);

                // USER_DATA dizinini yeni dizin adı ile değiştir
                string dosyalarPath = Path.Combine("DOSYALAR", fullCardName);
                // Yeni path'i oluştur
                string fonksiyonlarPath = Path.Combine(dosyalarPath, "Functions");

                try
                {
                    DirectoryInfo d = new DirectoryInfo(fonksiyonlarPath);
                    // Klasörün mevcut olup olmadığını kontrol edin
                    if (!d.Exists)
                    {
                        MessageBox.Show("Belirtilen dizin mevcut değil1.");
                        return;
                    }

                    // Tüm .txt dosyalarını al

                    // Tüm .txt dosyalarını al ve bir listeye kaydet
                    fileNames = d.GetFiles("*.txt").Select(f => f.Name).ToList();
                    // Dizi içeriğini temizle
                    sources = new string[] { };

                    string file;
                    //string fonksPath;
                    List<string> sourceList;
                    foreach (var item in fileNames)
                    {
                        sourceList = new List<string>(sources);
                        //fonksPath = $"{kartFilePath}/Functions";
                        file = Path.Combine(fonksiyonlarPath, item);

                        if (File.Exists(file))
                        {
                            string[] lines = File.ReadAllLines(file);
                            foreach (string line in lines)
                            {
                                sourceList.Add(line);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Dosya bulunamadı.");
                        }

                        // Listeyi tekrar diziye dönüştür
                        sources = sourceList.ToArray();
                    }



                    //// Düğüm var mı ve .txt dosyası mı kontrol et
                    if (selectedNode != null && selectedNode.Text.EndsWith(".txt"))
                    {


                        if (selectedNode.Tag != null && selectedNode.Tag is string filePath)
                        {
                            if (File.Exists(filePath))
                            {
                                await tsFiles.CreateTab(filePath);
                                string[] lines = File.ReadAllLines(filePath);
                                StringBuilder numberedText = new StringBuilder();
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    // Hata varsa göster
                    MessageBox.Show("Bir hata oluştu : " + ex.Message);
                }

                    // her dosya değiştiğinde yani her karta özel tamlalamaları vereceğiz 
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            


        }
        private void getValueTagForAutoComplate(string directoryPath,bool PathReady)
        {


            charVariable.Clear();
            intVariable.Clear();
            longVariable.Clear();
            floatVariable.Clear();
            doubleVariable.Clear();

            string[] variable = new string[] {
                "uint8_t.txt",
                "uint16_t.txt",
                "uint32_t.txt",
                "float32_t.txt",
                "float64_t.txt"
            };

            string modifiedPath2 = "";
            if (!PathReady)
            {
                string newDirectoryName2 = "USER_DATA/Variable";
                directoryPath = Path.GetDirectoryName(directoryPath);
                if (Path.GetDirectoryName(directoryPath).Contains("USER_DATA"))
                {
                    newDirectoryName2 = "/Variable";
                }

                // USER_DATA dizinini yeni dizin adı ile değiştir
                string newDirectoryPath2 = Path.Combine(Path.GetDirectoryName(directoryPath), newDirectoryName2);

                // Yeni path'i oluştur
                modifiedPath2 = newDirectoryPath2;

            }
            else
            {
                modifiedPath2 = directoryPath;
            }



            activeProjectPath = modifiedPath2;
            variableAutoComplate = new string[] { };

            //string fonksPath;
            List<string> variableList;

            variableList = new List<string>(variableAutoComplate);


            foreach (var item in variable)
            {

                string modifiedPath3 = Path.Combine(modifiedPath2, item);

                if (File.Exists(modifiedPath3))
                {
                    string[] lines1 = File.ReadAllLines(modifiedPath3);
                    string getValueTag;

                    for (int i = 0; i < lines1.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(lines1[i]))
                        {
                            getValueTag = lines1[i].Split(' ')[1].Replace(";", "").Trim();

                            if (!string.IsNullOrEmpty(getValueTag))
                            {
                                switch (item)
                                {
                                    case "uint8_t.txt":
                                        charVariable.Add(getValueTag);
                                        break;
                                    case "uint16_t.txt":
                                        intVariable.Add(getValueTag);
                                        break;
                                    case "uint32_t.txt":
                                        longVariable.Add(getValueTag);
                                        break;
                                    case "float32_t.txt":
                                        floatVariable.Add(getValueTag);
                                        break;
                                    case "float64_t.txt":
                                        doubleVariable.Add(getValueTag);
                                        break;
                                    default:
                                        break;
                                }
                                variableList.Add(getValueTag);
                            }

                        }


                    }

                    variableAutoComplate = variableList.ToArray();

                }
            }

        }
        private List<string> GetAllTabPageTitles(TabControl tabControl)
        {

            List<string> tabPageTitles = new List<string>();
            foreach (TabPage tabPage in tabControl.TabPages)
            {

                tabPageTitles.Add(tabPage.Text);

            }
            return tabPageTitles;

        }

        private string copiedPath;
        ContextMenuStrip contextMenu = new ContextMenuStrip();

        private void InitializeContextMenu()
        {
            contextMenu.Items.Add("Add new file", null, add_File);
            contextMenu.Items.Add("Cut", null, Cut_Click);
            contextMenu.Items.Add("Copy", null, Copy_Click);
            contextMenu.Items.Add("Paste", null, Paste_Click);

            contextMenu.Items.Add("Delete", null, Delete_Click);
            contextMenu.Items.Add("Rename", null, Rename_Click);
            contextMenu.Items.Add("FilPath", null, FilePath_Click);
            contextMenu.Items.Add("New", null, New_Click);


            treeView1.ContextMenuStrip = contextMenu;

        }

        private void add_File(object sender, EventArgs e)
        {
            string click_path = rightClickedNode.Tag.ToString();
            if (click_path.Contains("Functions"))
            {
                //MessageBox.Show("fonksiyonlar klasörüne dosya eklenebilir");
                string newName = Microsoft.VisualBasic.Interaction.InputBox("dosya adını girin: ", "dosya ekleme", rightClickedNode.Text);
                if (!string.IsNullOrEmpty(newName))
                {
                    StreamWriter userWriter2 = new StreamWriter(click_path + "/" + newName);
                    toolStripButton5_Click(sender,e);
                }
                    
            }
            else
            {
                MessageBox.Show("Sadece fonksiyon ekleyebilirsiniz");
            }
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            if (rightClickedNode != null)
            {
                Clipboard.SetText(rightClickedNode.Tag.ToString());
                // "Paste" menü öğesini adını kullanarak erişin
                var pasteMenuItem = contextMenu.Items["Paste"];
                // Menüyü etkinleştir / devre dışı bırak
                pasteMenuItem.Enabled = Clipboard.ContainsText();  // Panoda metin varsa etkin, yoksa devre dışı
                rightClickedNode.Remove();
            }
        }
        private void Copy_Click(object sender, EventArgs e)
        {

            if (rightClickedNode != null)
            {
                MessageBox.Show("burda");
                Clipboard.SetText(rightClickedNode.Tag.ToString());
                copiedPath = rightClickedNode.Tag.ToString();
            }
        }

        private async void Paste_Click(object sender, EventArgs e)
        {
            if (copiedPath != null && rightClickedNode != null)
            {
                if (rightClickedNode.Tag == null)
                {
                    MessageBox.Show("null");
                    return;
                }

                string parentPath = rightClickedNode.Tag.ToString();

                string newPath = Path.Combine(parentPath, Path.GetFileName(copiedPath));
                MessageBox.Show("path: " + newPath);

                if (File.Exists(newPath) || Directory.Exists(newPath))
                {
                    MessageBox.Show("Aynı isimde bir dosya zaten mevcut.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (File.Exists(copiedPath))
                {
                    File.Copy(copiedPath, newPath, true);
                    TreeNode newNode = new TreeNode(Path.GetFileName(newPath))
                    {
                        Tag = newPath
                    };
                    rightClickedNode.Nodes.Add(newNode);
                }
                else if (Directory.Exists(copiedPath))
                {
                    CopyDirectory(copiedPath, newPath);
                    TreeNode newNode = new TreeNode(Path.GetFileName(newPath))
                    {
                        Tag = newPath
                    };
                    rightClickedNode.Nodes.Add(newNode);
                }


                await UpdateTreeView();
                copiedPath = null;
            }
        }

        private async Task UpdateTreeView()
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                await UpdateNodeIcon(node);
            }
        }

        private async Task UpdateNodeIcon(TreeNode node)
        {
            if (node.Tag != null)
            {
                string path = node.Tag.ToString();

                if (File.Exists(path))
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                }
                else if (Directory.Exists(path))
                {
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 0;
                }
            }

            foreach (TreeNode childNode in node.Nodes)
            {
                await UpdateNodeIcon(childNode);
            }
        }

        private void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string subDirName = Path.GetFileName(subDir);
                CopyDirectory(subDir, Path.Combine(destDir, subDirName));
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), true);
            }
        }


        private void Delete_Click(object sender, EventArgs e)
        {
            if (rightClickedNode != null && !rightClickedNode.Tag.ToString().Contains("Variable")) 
            {

                if (MessageBox.Show("Bu dosyayı/kasörü silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string path = rightClickedNode.Tag.ToString();
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    else if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    rightClickedNode.Remove();
                }
            }
            else
            {
                MessageBox.Show("Variable klasöründen dosya silinemez.");
            }
        }
        private void Rename_Click(object sender, EventArgs e)
        {
            if (rightClickedNode != null)
            {
                string oldPath = rightClickedNode.Tag.ToString();
                string newName = Microsoft.VisualBasic.Interaction.InputBox("Yeni adı girin:", "Yeniden Adlandır", rightClickedNode.Text);
                if (!string.IsNullOrEmpty(newName))
                {
                    string newPath = Path.Combine(Path.GetDirectoryName(oldPath), newName);

                    if (Directory.Exists(oldPath))
                    {
                        Directory.Move(oldPath, newPath);
                    }
                    else if (File.Exists(oldPath))
                    {
                        File.Move(oldPath, newPath);
                    }
                    rightClickedNode.Text = newName;
                    rightClickedNode.Tag = newPath;
                }
            }
        }
        private void FilePath_Click(object sender, EventArgs e)
        {
            if (rightClickedNode != null)
            {
                string filePath = rightClickedNode.Tag as string;

                if (!string.IsNullOrEmpty(filePath))
                {
                    Clipboard.SetText(filePath);
                    MessageBox.Show($"Seçili dosya yolu kopyalandı: {filePath}");
                }
            }
        }
        private void New_Click(object sender, EventArgs e)
        {
            if (rightClickedNode != null)
            {
                string projectName = rightClickedNode.Text;
                createProject createProject = new createProject(projectName, true);
                createProject.Show();
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                rightClickedNode = treeView1.GetNodeAt(e.X, e.Y);
                if (rightClickedNode != null)
                {
                    treeView1.SelectedNode = rightClickedNode;
                }
            }
        }


        private void treeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = "SONPROJELER.txt";

            if (File.Exists(filePath))
            {
                string[] projectLines = File.ReadAllLines(filePath);

                foreach (string line in projectLines)
                {
                    string[] parts = line.Split(',');

                    if (parts.Length >= 3)
                    {
                        string projectName = parts[0];
                        string projectLocation = parts[2];

                        if (Directory.Exists(projectLocation))
                        {
                            TreeNode projectNode = new TreeNode(projectName);
                            treeView1.Nodes.Add(projectNode);

                            AddDirectoryNodes(projectNode, projectLocation);
                        }
                        else
                        {
                            MessageBox.Show($"Proje dizini bulunamadı: {projectLocation}");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Geçersiz satır formatı: {line}");
                    }
                }
            }
            else
            {
                MessageBox.Show("SONPROJELER.txt dosyası bulunamadı.");
            }
        }

        private void AddDirectoryNodes(TreeNode parentNode, string directoryPath)
        {

            string[] subDirectories = Directory.GetDirectories(directoryPath);
            foreach (string subDirectory in subDirectories)
            {
                TreeNode subDirNode = new TreeNode(Path.GetFileName(subDirectory))
                {
                    Tag = subDirectory
                };
                parentNode.Nodes.Add(subDirNode);

                AddDirectoryNodes(subDirNode, subDirectory);
            }


            string[] files = Directory.GetFiles(directoryPath);
            foreach (string file in files)
            {
                if (Path.GetExtension(file).ToLower() == ".txt")
                {
                    TreeNode fileNode = new TreeNode(Path.GetFileName(file))
                    {
                        Tag = file,
                        ImageIndex = 1,
                        SelectedImageIndex = 1
                    };
                    parentNode.Nodes.Add(fileNode);
                }
            }
        }
             
        private async void convertCode_Click(object sender, EventArgs e){

            FATabStripItem activeTab = tsFiles.GetFATabStrip().SelectedItem;

            if (activeTab != null)
            {
                FastColoredTextBox tb = activeTab.Controls[0] as FastColoredTextBox;

                if (tb != null)
                {

                    if (errorCounter == 0)
                    {
                        getValueTagForAutoComplate(activeTab.Tag.ToString(),false);
                        MessageBox.Show("char adedi: " + charVariable.Count() + "\nint adedi: " + intVariable.Count() + "\nfloat adedi: " + floatVariable.Count() + "\nlong adedi : " + longVariable.Count() + "\ndouble adedi: " + doubleVariable.Count());
                        var converter = new ConvertCodeFormat(); // ConvertCodeFormat sınıfını başlat
                        try
                        {
                            await Task.Delay(100);
                            int lineNumber = 1;
                            AnalyzeCode analyzeCode = new AnalyzeCode();
                            analyzeCode.Analyze(tb.Lines.ToList(), activeTab);
                            

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Hata: {ex.Message}");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Hataları düzelttikten sonra tekrar deneyiniz.");
                    }
                }
                else
                {
                    MessageBox.Show("FastColoredTextBox bulunamadı.");
                }
            }
            else
            {
                MessageBox.Show("Aktif bir sekme yok.");
            }
        }

        private void closeErrorList_Click(object sender, EventArgs e)
        {
            // splitContainer2.Panel2
            //splitContainer2.Panel2Collapsed = true;
            splitContainer2.SplitterDistance = 1000;
        }

        private void tabControl2_Click(object sender, EventArgs e)
        {
            //splitContainer2.Panel2Collapsed = true;
            splitContainer2.SplitterDistance = 1000;
        }

        private async void PowerfulCSharpEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if  (e.Control && e.KeyCode == Keys.S)// Ctrl-S Save
            {
                //if (tsFiles.SelectedItem != null)
                await _fileService.Save(tsFiles.getCurrentTB().Parent as FATabStripItem, sfdMain,activeProjectPath);
                isDataGridViewChanged = false;
                // Do what you want here
                e.Handled = true;
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            string filePath = "SONPROJELER.txt";

            if (File.Exists(filePath))
            {
                string[] projectLines = File.ReadAllLines(filePath);

                foreach (string line in projectLines)
                {
                    string[] parts = line.Split(',');

                    if (parts.Length >= 3)
                    {
                        string projectName = parts[0];
                        string projectLocation = parts[2];

                        if (Directory.Exists(projectLocation))
                        {
                            TreeNode projectNode = new TreeNode(projectName);
                            treeView1.Nodes.Add(projectNode);

                            AddDirectoryNodes(projectNode, projectLocation);
                        }
                        else
                        {
                            MessageBox.Show($"Proje dizini bulunamadı: {projectLocation}");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Geçersiz satır formatı: {line}");
                    }
                }
            }
            else
            {
                MessageBox.Show("SONPROJELER.txt dosyası bulunamadı.");
            }
        }
    }


}
