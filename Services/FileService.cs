using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester.Core
{
    public class FileService
    {
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
    }
}
