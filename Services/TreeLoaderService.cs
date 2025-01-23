using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tester.Services
{
    public class TreeLoaderService
    {
        public async Task LoadDirectoryWithIcons(string directoryPath, TreeNodeCollection nodes, ImageList imageList)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            var rootNode = new TreeNode(directoryInfo.Name)
            {
                Tag = directoryInfo.FullName,
                ImageKey = "folder",
                SelectedImageKey = "folder"
            };

            nodes.Add(rootNode);
            await LoadSubDirectoriesWithIcons(directoryInfo, rootNode, imageList);
        }

        private async Task LoadSubDirectoriesWithIcons(DirectoryInfo directoryInfo, TreeNode parentNode, ImageList imageList)
        {
            foreach (var directory in directoryInfo.GetDirectories())
            {
                var directoryNode = new TreeNode(directory.Name)
                {
                    Tag = directory.FullName,
                    ImageKey = "folder",
                    SelectedImageKey = "folder"
                };

                parentNode.Nodes.Add(directoryNode);
                await LoadSubDirectoriesWithIcons(directory, directoryNode, imageList);
            }

            foreach (var file in directoryInfo.GetFiles("*.txt"))
            {
                var fileNode = new TreeNode(file.Name)
                {
                    Tag = file.FullName,
                    ImageKey = GetCustomIconKey(file.DirectoryName, imageList),
                    SelectedImageKey = GetCustomIconKey(file.DirectoryName, imageList)
                };

                parentNode.Nodes.Add(fileNode);
            }
        }

        private string GetCustomIconKey(string directory, ImageList imageList)
        {
            if (directory.Contains("Functions"))
            {
                return imageList.Images.ContainsKey("functions") ? "functions" : "codeW";
            }
            else if (directory.Contains("Variable"))
            {
                return imageList.Images.ContainsKey("variable") ? "variable" : "codeW";
            }
            else
            {
                return "codeW";
            }
        }
    }
}
