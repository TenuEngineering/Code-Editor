using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tester.Services
{
    public class IconLoaderService
    {
        private readonly string _baseIconPath;

        public IconLoaderService(string baseIconPath)
        {
            _baseIconPath = baseIconPath;
        }

        public ImageList LoadIcons()
        {
            ImageList imageList = new ImageList();

            try
            {
                AddIconToList(imageList, "folder", "folder.png");
                AddIconToList(imageList, "codeW", "codeW.png");
                AddIconToList(imageList, "functions", "fonksiyonlar.png");
                AddIconToList(imageList, "variable", "variable.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading icons: " + ex.Message);
            }

            return imageList;
        }

        private void AddIconToList(ImageList imageList, string key, string fileName)
        {
            string fullPath = Path.Combine(_baseIconPath, fileName);

            if (File.Exists(fullPath))
            {
                imageList.Images.Add(key, Image.FromFile(fullPath));
            }
            else
            {
                MessageBox.Show($"Icon file not found: {fullPath}");
            }
        }
    }
}
