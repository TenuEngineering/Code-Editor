using FarsiLibrary.Win;
using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace ECUCodeEditor
{
    internal class AnalyzeCode
    {
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


        string[] deyimler = new string[] {
            "if",
            "else if", // TODO KULLANILMAYACAK
            "else",
            "for", // TODO KULLANILMAYACAK
            "foreach", 
            "while",
            "switch",
            "do"
        };

        string[] kontroller = new string[]
        {
            "break",
            "continue",
            "case",
            "return"
        };

        Dictionary<string, string> operatorBitEquals = new Dictionary<string, string>
        {
       
        };

        List<string> code = new List<string>();

        public AnalyzeCode() { 
        
        
        }

        public async void Analyze(List<string> codeLines, FATabStripItem tab2)
        {
            List<List<string>> conditionsLeftAndRight = new List<List<string>>();
            string lineCodeFormat = "";
            var CurrentTB = tab2.Controls[0] as FastColoredTextBox;

            string filePath = tab2.Tag as string;
            string parentDirectoryName = ""; // proje pathi bu değişkende 

            if (!string.IsNullOrEmpty(filePath))
            {
                DirectoryInfo directoryInfo = Directory.GetParent(filePath);
                if (directoryInfo != null)
                {
                    parentDirectoryName = directoryInfo.Parent?.FullName;
                }
            }
            else
            {
                var tab = CurrentTB.Parent as FATabStripItem;
                if (tab != null)
                {
                    filePath = tab.Tag as string;
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        DirectoryInfo directoryInfo = Directory.GetParent(filePath);
                        if (directoryInfo != null)
                        {
                            parentDirectoryName = directoryInfo.Parent?.FullName;
                        }
                    }
                }
            }

            string[] variable = new string[] {
                "uint8_t.txt",
                "uint16_t.txt",
                "uint32_t.txt",
                "float32_t.txt",
                "float64_t.txt"
            };

            for (int i = 0; i < codeLines.Count; i++)
            {
                string line = codeLines[i];
                string deyim = getExpression(line);
                conditionsLeftAndRight.Clear();
                string data = "58 102 204";
                int parametreSayisi = 0;
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                if (deyim != "none")
                {
                    string dataToGo = "";

                    switch (deyim)
                    {

                        case "if":
                            // Koşulun tamamını al
                            string condition = ExtractCondition(codeLines, ref i);
                            condition = condition.Replace("if", "");
                            string firstByte = ""; // and or veya tek koşul olup olmadığını belirtir
                                                   // 1 : tek kooşul imleç yok
                                                   // 2 : || kullanılmıştır 2 koşul
                                                   // 3 : && kullanılmıştır 2 koşul ifade eder
                            int conditionFormatFirst;


                            // parametrelerin ilk byte olan koşulu tanımlayan kısım bitsel
                            //string conParamFirstByte = CalculateBinaryCondition(condition);
                            //MessageBox.Show(conParamFirstByte);


                            //MessageBox.Show($"Koşul: {condition}");

                            // Her bir koşulu işlemek için düzenli ifadeler kullanıyoruz
                            string[] tokens = Regex.Split(condition, @"(\|\||&&)");

                            //MessageBox.Show("bulunan imleç : " + tokens);
                            foreach (var item in tokens)
                            {
                                if (tokens.Count() > 1)
                                {
                                    if (tokens[1] == "||")
                                    {
                                        firstByte = "101";
                                    }
                                    else if (tokens[1] == "&&")
                                    {
                                        firstByte = "102";
                                    }

                                }
                                else
                                {
                                    firstByte = "100"; // komut high kısmı
                                }

                                
                                if (!item.Contains("&") && !item.Contains("|"))
                                {

                                    conditionsLeftAndRight.Add(getOperator(item));

                                }

                            }
                            dataToGo += $" {firstByte}";
                            // ilk byte belli olduktan sonra parametre sayısını 1 yapıyoruz
                            // önce toplam gönderilecek parametre sayısını bulmamız gerekiyor

                            //data += $" {firstByte}"; // eklemeler parametre sayısı belli olduktan sonra yapılacak

                            foreach (var item in conditionsLeftAndRight)
                            {

                                //MessageBox.Show("Koşul eoperatörü: " + item[0] + "\nKoşul sol tarafı: " + item[1] + "\nKoşul sağ tarafı: " + item[2]);

                                //buraya koşul içerisindeki değerin kullanıcımı sistem mi değişkeni olduğunu aldırıcaz.
                                conditionFormatFirst = getUserOrSystemVar(parentDirectoryName, "USER_DATA", variable, item[1]);
                                if (conditionFormatFirst == 0)
                                {
                                    MessageBox.Show("Kullanılan değişken ( " + item[1] + " ) tanımlanmamış");
                                    // TODO:eğer kullanıcı verilerinde bulunamadıysa sistem verilerinde ara 
                                    return;
                                }


                                

                                // koşulun operatörü ve koşul sağ tarafı 

                                string input = item[2];
                                bool isNumeric = int.TryParse(input, out _);
                                long isNumericData = 0;
                                int operatorParametresi = 0;
                                // operatör yazılırken 4 bit high tanımı 4 bit te koşulun sağ tarafının türü (sayımı değişkenmi)
                                // komut high daki kalan 1 biti sayısı ise eğer artımı eksimi diye bakılacak eksi ise : 1
                                // artı ise 0 olarak kalıcak


                                // operatör boş ve null değilse girer aksi durumda bu bir bayraktır
                                if (!string.IsNullOrEmpty(item[0]))
                                {
                                    // TODO: bu bir byte sadece operatörü tanımlatyacak + - durumları komut low da ayarlanacak ve sayımı değilmi belirtilmesine gerek yok
                                    switch (item[0])
                                    {

                                        case "==":
                                            // sayı ve eşit eşit karşılığı = 0010 0001
                                            //                         komut high komut low
                                            //              operatörü temsil eder sayımı değişkenmi onu temsil eder
                                            operatorParametresi += 33; //  0010 0001
                                            break;

                                        case "!=":
                                            operatorParametresi += 65; //  0100 0001
                                            break;

                                        case ">":

                                            operatorParametresi += 97; //  0110 0001
                                            break;

                                        case "<":
                                            operatorParametresi += 129; //  1000 0001
                                            break;

                                        case ">=":
                                            operatorParametresi += 161; //  1010 0001
                                            break;

                                        case "<=":
                                            operatorParametresi += 193; //  1100 0001
                                            break;

                                        default:
                                            break;
                                    }
                                    parametreSayisi += 1; // operatör için kullanılacak byte


                                    if (isNumeric)
                                    {
                                        // sayı ise ilk 3 bit operatör için ayır
                                        // kalan bitler de sayıyı yaz

                                        UInt32 number = Convert.ToUInt32(item[2]);
                                        if (number < 0)
                                            operatorParametresi += 16; // 0001 0000 eksi artı olduğunu belirten bit 1 yapıldığı için 16 ekleniyoır
                                                                       // String'i tam sayıya çeviriyoruz
                                        int numberA = int.Parse(item[2]);


                                        // Sayının binary (ikili) gösterimini alıyoruz
                                        string binaryRepresentation = Convert.ToString(numberA, 2);

                                        // Binary gösterimdeki bit uzunluğunu alıyoruz
                                        int bitLength = binaryRepresentation.Length;

                                        // Bit uzunluğunu byte'a çeviriyoruz (8 bit = 1 byte)
                                        int byteLength = (bitLength + 7) / 8; // Tavan yuvarlama için 7 ekleyip bölüyoruz

                                        //MessageBox.Show($"Sayı: {number}");
                                        //MessageBox.Show($"Binary gösterimi: {binaryRepresentation}");
                                        //MessageBox.Show($"Bit uzunluğu: {bitLength} bit");
                                        //MessageBox.Show($"Byte uzunluğu: {byteLength} byte");

                                        //MessageBox.Show(binaryRepresentation);

                                        // Eğer byte sayısı 4'ten küçükse başına 0 ekleme kontrolü
                                        if (byteLength < 4)
                                        {
                                            MessageBox.Show("Sayı 4 byte'tan küçük, başına 0 eklenerek 4 byte tamamlanıyor.");
                                        }
                                        else if (byteLength > 4)
                                        {
                                            MessageBox.Show("Uyarı: Bu sayı 4 byte'tan daha fazla yer kaplıyor!");
                                        }

                                        dataToGo += $" {operatorParametresi}";
                                        MessageBox.Show("koşul operatör karşılığı: " + operatorParametresi.ToString() + "\nkoşu sağ tarafı: " + number);

                                        parametreSayisi += 4; // bir byte operatör için kullanıclacak;
                                        dataToGo += $" {number}";

                                    }
                                    else
                                    {

                                        // tam sayı değilse hangi değişken olduğunu kontrol et
                                        MessageBox.Show("Sayı değil");

                                        // koşulun sağ tarafını ara
                                        int conditionRight = getUserOrSystemVar(parentDirectoryName, "USER_DATA", variable, item[2]);

                                        if (conditionRight == 0)
                                        {
                                            MessageBox.Show("Kullanılan değişken ( " + item[2] + " ) tanımlanmamış");
                                            // TODO:eğer kullanıcı verilerinde bulunamadıysa sistem verilerinde ara 
                                            
                                            return;
                                        }

                                        dataToGo += $" {operatorParametresi} {conditionRight}";

                                        parametreSayisi += 2; // bir byte operatör için kullanıclacak;
                                        
                                        MessageBox.Show("koşul operatör karşılığı: " + operatorParametresi.ToString());

                                    }

                                }

                                parametreSayisi += 2;
                                dataToGo += $" {conditionFormatFirst}";

                                // koşul sol tarafıyla işlem yapacağız sol taraf kullanıcı mı sistem mi bakılacak ve değeri alınacak

                                // ilk olarak datayı kullanıcı değişkenlerinde ara

                                // kullanıcımı diye bak
                            }

                            data += $" {parametreSayisi}{dataToGo}";


                            MessageBox.Show("Gidecek veri: " + data);
                            //MessageBox.Show(CalculateBinaryCondition(condition));
                            //string conditionContent = ExtractConditionContent(codeLines, ref i);
                            //MessageBox.Show(conditionContent);



                            break;
                        case "else":
                            break;
                        case "for":
                            break;
                        case "foreach":
                            break;
                        case "while":

                            // Koşulun tamamını al
                            string conditionWh = ExtractCondition(codeLines, ref i);
                            condition = conditionWh.Replace("while", "");

                            MessageBox.Show($"Koşul: {condition}");
                            // Her bir koşulu işlemek için düzenli ifadeler kullanıyoruz
                            string[] tokensWh = Regex.Split(condition, @"(\|\||&&)");
                            foreach (var item in tokensWh)
                            {
                                List<string> ope = getOperator(item);

                            }

                            //MessageBox.Show(CalculateBinaryCondition(condition));



                            break;

                        case "switch":
                            break;

                        default:
                            break;
                    }
                }
                else if (line.Contains("."))
                {
                    string lineFormat  = await getAutoComplateFunction(line);
                    MessageBox.Show("bu bir fonksiyon ve formatı: " + lineFormat);
                }

                else 
                {
                    List<string> operations = getOperator(line);    
                    foreach (var item in operations)
                    {
                        MessageBox.Show("item: " + item);
                    }
                }


            }


        }

        private int getUserOrSystemVar(string path, string controlDict, string[] variable, string data) // proje pathini verir, kullanıcı sistemmi belirtir, kontrol edileccek değişken listesini verir
        {                                                                                   // data kontrol edileccek string veri

            string modifedPathUsers = Path.Combine(path, controlDict); // KULLANICI_VERİLŞERİMİ , SİSTEM_VERİLERİMİ BELİRTİYORUZ
            modifedPathUsers = Path.Combine(modifedPathUsers, "Variable"); // ALT KLASÖRÜ SONRADAN PARAMETRE OLARAK EKLENEBİLİR

            int valuLİneNumber = 1;
            string type = "";
            int valueBinarySayısı = 0;

            string modifedPath = "";
            bool founded = false;
            foreach (string item2 in variable)
            {
                modifedPath = Path.Combine(modifedPathUsers, item2);

                if (File.Exists(modifedPath))
                {
                    string[] lines1 = File.ReadAllLines(modifedPath);
                    foreach (var item3 in lines1)
                    {
                        if (string.IsNullOrEmpty(item3))
                        {
                            continue;
                        }

                        string valControl = item3.Split(' ')[1].Replace(";", "").Trim();
                        if (data == valControl)
                        {
                            MessageBox.Show("bu değişkenin türü: " + item2);
                            switch (item2.Replace(".txt", ""))
                            {

                                case "flag":
                                    // 0010 0000 0000 0000
                                    break;

                                case "uint8_t":
                                    valueBinarySayısı = 16384; // 0100 0000 0000 0000
                                    break;

                                case "uint16_t":
                                    valueBinarySayısı = 24576; // 0110 0000 0000 0000
                                    break;

                                case "uint32_t":
                                    valueBinarySayısı = 32768; // 1000 0000 0000 0000
                                    break;

                                case "float32_t":
                                    valueBinarySayısı = 40960; // 1010 0000 0000 0000
                                    break;

                                case "float64_t": // double
                                    valueBinarySayısı = 49152; // 1100 0000 0000 0000
                                    break;

                                default:
                                    break;

                            }
                            //parametreSayisi += 2; // parametre sayısı değişkenin sol tarafı belli olduktan sonra 2 byte kullanır

                            founded = true; break;
                        }


                        valuLİneNumber++;

                    }
                    if (founded)
                    {
                        valueBinarySayısı += valuLİneNumber;
                        break;
                    }
                        

                }

            }

            return valueBinarySayısı;

        }

        // ( görüldüğünde sol tarafındaki deyimi al
        private string getExpression(string code)
        {
            string[] expression = new string[] {};
            foreach (var deyim in deyimler)
            {
                if (code.Contains(deyim))
                {
                    return deyim; // Eğer bulursa, deyimi döndür
                }
            }
            return "none";
        }

        private string getCondition(string code)
        {
            string[] expression = new string[] { };
            expression = code.Split('(');
            int endIndex = expression[1].IndexOf(')');
            return expression[1].Substring(0,endIndex);
        }

        private List<string> getOperator(string code)
        {
            // Tanımlanan operatörler
            string[] operators = { "==", "!=", "<=", ">=", "<", ">", "=","+=","-="};

            // Sonuçları tutacak liste
            List<string> condition = new List<string>();

            // Bulunan operatörü ve pozisyonunu saklamak için değişkenler
            string foundOperator = "";
            int operatorPos = -1;

            // Kod satırında operatörleri arayın
            foreach (string op in operators)
            {
                int pos = code.IndexOf(op);
                if (pos != -1)
                {
                    foundOperator = op;
                    operatorPos = pos;
                    break; // İlk bulduğumuz operatörü alıyoruz
                }
            }

            // Eğer operatör bulunduysa
            if (!string.IsNullOrEmpty(foundOperator))
            {
                // Eşitliğin sol tarafını alın
                string leftSide = code.Substring(0, operatorPos).Trim();

                // Eşitliğin sağ tarafını alın
                string rightSide = code.Substring(operatorPos + foundOperator.Length).Trim();

                // Listeye ekle
                condition.Add(foundOperator); // Operatör
                condition.Add(leftSide);      // Eşitliğin solu
                condition.Add(rightSide);     // Eşitliğin sağı
            }

            return condition; // Listeyi döndür
        }

        private string ExtractCondition(List<string> codeLines, ref int currentIndex)
        {
            StringBuilder conditionBuilder = new StringBuilder();
            int parenthesesCount = 0;
            bool foundOpening = false;

            for (int i = currentIndex; i < codeLines.Count; i++)
            {
                string line = codeLines[i];
                
                // Eğer bir açılış parantezi bulursak
                if (line.Contains("("))
                {
                    foundOpening = true;
                    parenthesesCount += line.Count(c => c == '(');
                }

                // Eğer açılış parantezini bulduysak, koşulu birleştirmeye başla
                if (foundOpening)
                {
                   
                    conditionBuilder.Append(line);

                    // Kapanış parantezlerini say
                    parenthesesCount -= line.Count(c => c == ')');

                    // Eğer parantezler kapanmışsa, koşulu tam olarak aldık demektir
                    if (parenthesesCount == 0)
                    {
                        currentIndex = i;  // currentIndex'i güncelle, böylece döngü devam edebilir
                        break;
                    }
                }
            }


            string condition = conditionBuilder.ToString();
            if (conditionBuilder.ToString().Contains("{"))
            {
                int indeksOpen = condition.IndexOf("{");
                condition = conditionBuilder.ToString().Remove(indeksOpen);
            }


            condition = condition.Replace("(", "");
            condition = condition.Replace(")", "");

            return condition.Trim();
        }

        // tamlamalar burada yapılıyor
        private async Task<string> getAutoComplateFunction(string codeLine)
        {
            // Aktif sekmede bulunan FastColoredTextBox kontrolünü al
            int komutHigh = 0;
            int komutLow = 0;


            try
            {
                await Task.Delay(100);

                int parametreSayisi = 1;

                if (codeLine != "")
                {

                    // Satırdaki parantezi kaldır, sonra noktalarla böl
                    string codeToFormat = codeLine.Split('(')[0];
                    string[] parts = codeToFormat.Split('.');
                    komutHigh = format[parts[0]];
                    komutLow = pinNumber[parts[1]];

                    // En uzun eşleşmeyi bulmak için
                    int parametre1 = 0;
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        string keyToCheck = string.Join(".", parts, 2, i);
                        if (format.ContainsKey(keyToCheck))
                        {
                            parametre1 = this.format[keyToCheck];
                        }
                    }

                    // Parantez içindeki kısmı bulma
                    int startIndex = codeLine.IndexOf('(') + 1;
                    int endIndex = codeLine.IndexOf(')');
                    string insideParentheses = codeLine.Substring(startIndex, endIndex - startIndex);


                    // Alfanumerik değerleri arayan regex deseni
                    string pattern = @"[a-zA-Z0-9]+";
                    Regex regex = new Regex(pattern);
                    // Eşleşmeleri bul
                    Match match = regex.Match(insideParentheses);



                    var parameters = insideParentheses.Split(',');

                    // Virgül sayısını bulma
                    int commaCount = insideParentheses.Count(c => c == ',');
                    parametreSayisi += commaCount;

                    if (commaCount > 0)
                    {
                        if (commaCount >= 1) parametreSayisi++;
                        string param = "";
                        foreach (var item in parameters)
                        {
                            param += $" {item}";
                        }

                        MessageBox.Show("58 102 204 " + komutHigh + " " + komutLow + " " + parametreSayisi + " " + parametre1.ToString() + " " + param + " 59");
                        // Eğer komutHigh 0 kalmışsa, bu demektir ki, sözlükte eşleşen bir anahtar bulunamamış.
                        return "58 102 204 " + komutHigh + " " + komutLow + " " + parametreSayisi + " " + parametre1.ToString() + " " + param + " 59";
                    }

                    else
                    {
                        if (match.Success)
                        {
                            parametreSayisi += 1;
                            MessageBox.Show("58 102 204 " + komutHigh + " " + komutLow + " " + parametreSayisi + " " + parametre1.ToString() + " " + match.Value + " 59");
                            return "58 102 204 " + komutHigh + " " + komutLow + " " + parametreSayisi + " " + parametre1.ToString() + " " + match.Value + " 59";
                        }
                        else
                        {
                            MessageBox.Show("58 102 204 " + komutHigh + " " + komutLow + " " + parametreSayisi + " " + parametre1.ToString() + " 59");
                            return "58 102 204 " + komutHigh + " " + komutLow + " " + parametreSayisi + " " + parametre1.ToString() + " 59";
                        }
                    }


                }

            }
            
            catch
            {
                return "0";
            }
            return "0";
        }
    

        private string ExtractConditionContent(List<string> codeLines, ref int currentIndex)
        {
            StringBuilder conditionBuilder = new StringBuilder();
            int parenthesesCount = 0;
            bool foundOpening = false;

            for (int i = currentIndex; i < codeLines.Count; i++)
            {
                string line = codeLines[i];

                // Eğer bir açılış parantezi bulursak
                if (line.Contains("{"))
                {
                    foundOpening = true;
                    parenthesesCount += line.Count(c => c == '{');
                }

                // Eğer açılış parantezini bulduysak, koşulu birleştirmeye başla
                if (foundOpening)
                {
                    string codeContent = line;
                    if (line.Contains("{"))
                    {
                        int contentIndex = line.IndexOf("{");
                        int lineLen = line.Length;
                        MessageBox.Show("UZUNLUĞU: " + lineLen);
                        codeContent = line.Substring(contentIndex);
                      
                    }

                    conditionBuilder.Append(codeContent.Trim());

                    // Kapanış parantezlerini say
                    parenthesesCount -= line.Count(c => c == '}');

                    // Eğer parantezler kapanmışsa, koşulu tam olarak aldık demektir
                    if (parenthesesCount == 0)
                    {
                        currentIndex = i;  // currentIndex'i güncelle, böylece döngü devam edebilir
                        break;
                    }
                }
            }

            return conditionBuilder.ToString();
        }


        
    }
}
