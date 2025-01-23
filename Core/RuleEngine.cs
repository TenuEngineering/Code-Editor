using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Tester.PowerfulCSharpEditor;

namespace Tester.Core
{
    public class RuleEngine
    {
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

        private int GetLineNumber(string code, int index)
        {
            return code.Take(index).Count(c => c == '\n') + 1;
        }
    }

}
