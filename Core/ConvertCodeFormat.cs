using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ECUCodeEditor
{
    internal class ConvertCodeFormat
    {
        public string ConvertTo(string codeLine, Dictionary<string, int> format, Dictionary<string, int> pinNumber, int lineNumber)
        {
            var analysisResult = AnalyzeCodeLine(codeLine, format, pinNumber); // Kod satırını analiz et
            return ApplyFormat(analysisResult, lineNumber); // Formata uygula
        }

        private string ExtractConditionUsingStack(string codeLine)
        {
            Stack<char> stack = new Stack<char>();
            StringBuilder condition = new StringBuilder();

            // Kod satırını karakter karakter işleyelim
            for (int i = 0; i < codeLine.Length; i++)
            {
                char currentChar = codeLine[i];

                // Parantez açıldığında yığına ekleyelim
                if (currentChar == '(')
                {
                    stack.Push(currentChar);
                }
                // Parantez kapandığında yığına son eklenen parantezi çıkartalım
                else if (currentChar == ')')
                {
                    if (stack.Count > 0 && stack.Peek() == '(')
                    {
                        stack.Pop(); // Eşleşen parantezi çıkar
                        break; // Parantez kapandı, artık koşul bitti
                    }
                }
                // Yığın içinde olduğumuz sürece koşul karakterlerini toplayalım
                else if (stack.Count > 0)
                {
                    condition.Append(currentChar);
                }
            }

            return condition.ToString().Trim(); // Koşulu döndürelim
        }




        private AnalysisResult AnalyzeCodeLine(string codeLine, Dictionary<string, int> format, Dictionary<string, int> pinNumber)
        {

            var result = new AnalysisResult();

            if (codeLine.Trim().StartsWith("if"))
            {
                result.Type = "if";
                result.Condition = ExtractConditionUsingStack(codeLine); // Yığın kullanarak koşulu çıkar
                result.Parameters = ExtractParameters(codeLine, result.Condition);
                result.ParamCount = result.Parameters.Count;
                result.BinaryCondition = CalculateBinaryCondition(result.Condition);
            }
            else if (codeLine.Contains("="))
            {
                result.Type = "assignment";
                result.LeftHandSide = ExtractLeftHandSide(codeLine);
                result.RightHandSide = ExtractRightHandSide(codeLine);
                result.ParamCount = 1;
            }

            return result;
        }



        private string ApplyFormat(AnalysisResult analysis, int lineNumber)
        {
            string format = "";

            if (analysis.Type == "if")
            {
                format = $"58 102 204 100 {lineNumber} {analysis.ParamCount} {analysis.BinaryCondition} {analysis.ParametersToString()} crc 59";
            }
            else if (analysis.Type == "assignment")
            {
                format = $"58 102 204 30 01 {analysis.LeftHandSide} {analysis.RightHandSide} crc 59";
            }

            return format;
        }

        private string ExtractCondition(string codeLine)
        {
            int startIndex = codeLine.IndexOf('(') + 1;
            int endIndex = codeLine.IndexOf(')');
            return codeLine.Substring(startIndex, endIndex - startIndex);
        }

        private List<string> ExtractParameters(string codeLine, string condition)
        {
            var parameters = new List<string>();

            var matches = Regex.Matches(condition, @"\b\w+\b");
            foreach (Match match in matches)
            {
                parameters.Add(match.Value);
            }

            return parameters;
        }

        private string CalculateBinaryCondition(string condition)
        {

            // Koşul operatörlerini tanımla (1 bit)
            Dictionary<string, string> conditionMap = new Dictionary<string, string>
            {
                { "==", "1" },  // Eşittir
                { "!=", "1" },  // Eşit değil
                { ">", "1" },   // Büyüktür
                { "<", "1" },   // Küçüktür
                { ">=", "1" },  // Büyük eşit
                { "<=", "1" }   // Küçük eşit
            };

            // Mantıksal operatörleri tanımla (1 bit)
            Dictionary<string, string> operatorMap = new Dictionary<string, string>
            {
                { "&&", "1" }, // AND
                { "||", "0" }  // OR
            };

            // Her bir koşulu işlemek için düzenli ifadeler kullanıyoruz
            string[] tokens = Regex.Split(condition, @"(\|\||&&)");

            List<string> countCondition = new List<string>();
            MessageBox.Show(tokens[0]);
            foreach (string token in tokens)
            {
                string trimmedToken = token.Trim();


                // Koşul operatörlerini bulup, ikili değerini ekle
                foreach (var conditionKey in conditionMap.Keys)
                {
                    if (trimmedToken.Contains(conditionKey))
                    {
                        countCondition.Insert(0, conditionMap[conditionKey]);
                        break;
                    }
                }


                // Mantıksal operatörleri ekle
                foreach (var operatorKey in operatorMap.Keys)
                {
                    if (trimmedToken.Contains(operatorKey))
                    {
                        countCondition.Insert(0, operatorMap[operatorKey]);
                        trimmedToken = trimmedToken.Replace(operatorKey, "").Trim();
                    }
                }
            }
           
           

            // Kalan bitleri "00" olarak doldur
            while (countCondition.Count < 8)
            {
                countCondition.Insert(0, "0");  // En başa "00" ekleyerek bitleri dolduruyoruz
            }
            string combinedString = string.Join("", countCondition);
            int decimalValue = Convert.ToInt32(combinedString, 2);
            return decimalValue.ToString();
        }
        private string ExtractLeftHandSide(string codeLine)
        {
            return codeLine.Split('=')[0].Trim();
        }

        private string ExtractRightHandSide(string codeLine)
        {
            return codeLine.Split('=')[1].Trim(';').Trim();
        }

        class AnalysisResult
        {
            public string Type { get; set; }
            public string Condition { get; set; }
            public List<string> Parameters { get; set; } = new List<string>();
            public string BinaryCondition { get; set; }
            public int ParamCount { get; set; }
            public string LeftHandSide { get; set; }
            public string RightHandSide { get; set; }

            public string ParametersToString()
            {
                return string.Join(" ", Parameters);
            }
        }
    }
}
