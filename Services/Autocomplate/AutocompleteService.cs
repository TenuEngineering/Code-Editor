using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Properties;
using Tester.Services.Autocomplate;
using Tester.Core.Method;
using System.IO;
using System.Windows.Forms;

namespace Tester.Core
{
    public class AutocompleteService
    {
        List<string> charVariable = new List<string>();
        List<string> intVariable = new List<string>();
        List<string> longVariable = new List<string>();
        List<string> floatVariable = new List<string>();
        List<string> doubleVariable = new List<string>();
        List<string> arrayVariable = new List<string>();
        List<string> userFlagVariable = new List<string>();

        List<string> systemVariable = new List<string>();
        List<string> systemFlagVariable = new List<string>();

        string[] keywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield" };
        string[] methods = { "Equals()", "GetHashCode()", "GetType()", "ToString()" };
        string[] snippets = { "if(^)\n{\n}", "if(^)\n{\n}\nelse\n{\n}", "for(^;;)\n{\n}", "while(^)\n{\n}", "do\n{\n^;\n}while();", "switch(^)\n{\ncase : break;\n}" };
        string[] declarationSnippets = {
               "public class ^\n{\n}", "private class ^\n{\n}", "internal class ^\n{\n}",
               "public struct ^\n{\n;\n}", "private struct ^\n{\n;\n}", "internal struct ^\n{\n;\n}",
               "public void ^()\n{\n;\n}", "private void ^()\n{\n;\n}", "internal void ^()\n{\n;\n}", "protected void ^()\n{\n;\n}",
               "public ^{ get; set; }", "private ^{ get; set; }", "internal ^{ get; set; }", "protected ^{ get; set; }"
               };
        public string[] sources = new string[]{

        };
        static string[] variableAutoComplate = new string[]{

        };
        public AutocompleteService()
        {
        }



        public void BuildAutocompleteMenu(AutocompleteMenu popupMenu)
        {
            List<AutocompleteItem> items = new List<AutocompleteItem>();

            foreach (var item in snippets)
                items.Add(new SnippetAutocompleteItem(item) { ImageIndex = 1 });
            foreach (var item in declarationSnippets)
                items.Add(new DeclarationSnippet(item) { ImageIndex = 0 });
            foreach (var item in methods)
                items.Add(new MethodAutocompleteItem(item) { ImageIndex = 2 });
            foreach (var item in sources)
                items.Add(new MethodAutocompleteItem2(item));
            foreach (var item in variableAutoComplate)
                items.Add(new MethodAutocompleteItem2(item));
            foreach (var item in keywords)
                items.Add(new AutocompleteItem(item));

            items.Add(new InsertSpaceSnippet());
            items.Add(new InsertSpaceSnippet(@"^(\w+)([=<>!:]+)(\w+)$"));
            items.Add(new InsertEnterSnippet());

            //set as autocomplete source
            popupMenu.Items.SetAutocompleteItems(items);

            popupMenu.SearchPattern = @"[\w\.:=!<>]";
        }

        public void getValueTagForAutoComplate(string directoryPath, bool PathReady)
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
                //directoryPath = Path.GetDirectoryName(directoryPath);
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



            string activeProjectPath = modifiedPath2;
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

        public List<AutocompleteItem> GetAutocompleteItems()
        {
            var items = new List<AutocompleteItem>();
            foreach (var snippet in snippets)
                items.Add(new SnippetAutocompleteItem(snippet));

            foreach (var keyword in keywords)
                items.Add(new AutocompleteItem(keyword));

            return items;
        }
    }

}
