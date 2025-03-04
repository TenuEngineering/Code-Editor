using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ECUCodeEditor.Services.Autocomplate;
using ECUCodeEditor.Core;
using ECUCodeEditor.Core.Models;

namespace ECUCodeEditor.Core
{5
    public class FileService
    {
        
        public AutocompleteService autocompleteService;
        variableSizeControl variableSizeControl = new variableSizeControl();
        public string ReadFile(string path)
        {
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }

        public void WriteFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public string[] ReadAllLines(string path)
        {
            return File.Exists(path) ? File.ReadAllLines(path) : Array.Empty<string>();
        }

        public async Task<bool> Save(FATabStripItem tab, SaveFileDialog sfdMain,string activeProjectPath)
        {
            bool res = false;

            if (tab.Controls[0] is FastColoredTextBox tb)
            {
                if (tab.Tag == null)
                {
                    if (sfdMain.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        return false;
                    tab.Title = Path.GetFileName(sfdMain.FileName);
                    tab.Tag = sfdMain.FileName;
                }

                try
                {
                    try
                    {
                        res = await variableSizeControl.variableLimitControl(tab.Parent as FATabStrip,tb);

                    }
                    catch (Exception)
                    {
                        res = true;
                    }

                    if (res == false)
                    {
                        return false;
                    }
                    File.WriteAllText(tab.Tag as string, tb.Text);

                    await SaveVariableToFile(tab.Parent as FATabStrip,activeProjectPath);
                    tb.IsChanged = false;
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                        return await Save(tab, sfdMain, activeProjectPath);
                    else
                        return false;
                }

                tb.Invalidate();
            }

            return true;
        }

        private async Task SaveVariableToFile(FATabStrip tsFiles, string fileName)
        {
            string directoryPath = Path.GetDirectoryName(fileName);

            autocompleteService.getValueTagForAutoComplate(fileName, true);


            for (int iTab = 0; iTab < tsFiles.Items.Count; iTab++)
            {
                if (tsFiles.Items[iTab].Tag != null)
                {
                    string directoryPath2 = Path.GetDirectoryName(tsFiles.Items[iTab].Tag.ToString());

                    if (tsFiles.Items[iTab].Controls[0] is FastColoredTextBox tb)
                    {
                        if (directoryPath2 == directoryPath)
                        {
                            // Autocomplete menüye erişmek için
                            var tbInfo = tb.Tag as TbInfo;
                            if (tbInfo != null)
                            {
                                var popupMenu = tbInfo.popupMenu;
                                if (popupMenu != null)
                                {

                                    popupMenu.Items.Refresh();
                                    // Burada popupMenu üzerinde işlemler yapabilirsiniz
                                    // Örneğin, menüyü yeniden oluşturabilirsiniz:
                                    autocompleteService.BuildAutocompleteMenu(popupMenu);
                                }
                            }
                        }
                    }
                }

            }


            await Task.Delay(100);
        }
    }
}
