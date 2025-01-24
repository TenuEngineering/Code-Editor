using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tester.Core
{
    public class variableSizeControl
    {
        const int totalVariableSize = 15; //byte cinsinden total size
        public async Task<bool> variableLimitControl(FATabStrip tsFiles, FastColoredTextBox CurrentTB)
        {
            string variableFile = "";

            List<string> variableControl = new List<string>();

            string directoryPath = tsFiles.SelectedItem.Tag as string;
            string fileName = tsFiles.SelectedItem.Title;


            string[] variable = new string[] {

                "uint8_t.txt",
                "uint16_t.txt",
                "uint32_t.txt",
                "float32_t.txt",
                "float64_t.txt"

            };

            foreach (var item in variable)
            {

                if (fileName == item)
                {
                    variableFile = fileName;
                    break;
                }
            }

            string modifiedPath2 = "";

            modifiedPath2 = Path.GetDirectoryName(directoryPath);
            string activeProjectPath = modifiedPath2;

            //toplam değişken boyutunu alıyoruz.
            int size = 0;
            if (!modifiedPath2.Contains("Variable"))
            {
                modifiedPath2 = Path.Combine(modifiedPath2, "Variable");
            }


            foreach (string item in variable)
            {

                string varibleType = item.Replace(".txt", "");
                int varSize = 0;
                switch (varibleType)
                {
                    case "uint8_t":
                        varSize = 1;
                        break;
                    case "uint16_t":
                        varSize = 2;
                        break;
                    case "uint32_t":
                        varSize = 4;
                        break;
                    case "float32_t":
                        varSize = 4;
                        break;
                    case "float64_t": //double
                        varSize = 8;
                        break;


                    case "uint8_array_t":
                        break;
                    case "uint16_array_t":
                        break;
                    case "float32_array_t":
                        break;
                    case "float64_array_t": //double
                        break;
                    default:
                        break;
                }

                switch (varibleType)
                {
                    case "uint8_t":
                        varibleType = "char";
                        break;
                    case "uint16_t":
                        varibleType = "int";
                        break;
                    case "uint32_t":
                        varibleType = "long";
                        break;
                    case "float32_t":
                        varibleType = "float";
                        break;
                    case "float64_t": //double
                        varibleType = "double";
                        break;

                    case "uint8_array_t":
                        break;
                    case "uint16_array_t":
                        break;
                    case "float32_array_t":
                        break;
                    case "float64_array_t": //double
                        break;
                    default:
                        break;

                }

                int countVar = 0;
                string modifiedPath3 = Path.Combine(modifiedPath2, item);
                if (item == variableFile)
                {
                    var tb = CurrentTB.Lines;

                    foreach (string tbText in tb)
                    {

                        if (string.IsNullOrEmpty(tbText))
                            continue;
                        
                        variableControl.Add(tbText.Split(' ')[1].Replace(";", ""));
                        countVar++;

                    }
                    size += countVar * varSize;
                    continue;
                }

                if (File.Exists(modifiedPath3))
                {
                    string[] lines1 = File.ReadAllLines(modifiedPath3);

                    foreach (var item2 in lines1)
                    {
                        if (string.IsNullOrEmpty(item2))
                            continue;
                        variableControl.Add(item2.Split(' ')[1].Replace(";", ""));
                        countVar++;

                    }

                }
                size += countVar * varSize;

            }


            for (int i = 0; i < variableControl.Count(); i++)
            {
                string val = variableControl[i];
                for (int j = 0; j < variableControl.Count(); j++)
                {
                    if (i == j)
                        continue;

                    if (variableControl[j] == val)
                    {
                        MessageBox.Show("Bu değişken zaten eklenmiş : " + val);
                        return false;
                    }

                }
            }



            MessageBox.Show(size + " total: " + totalVariableSize);

            if (size > totalVariableSize)
            {
                MessageBox.Show("Maksimum değişken sayısına ulaştınız\nSilinmesi gereken byte sayısı : " + (size - totalVariableSize));
                return false;
            }




            return true;
        }

    }


}
