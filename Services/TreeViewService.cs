using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECUCodeEditor.Core
{
    public class TreeViewService
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

            await PopulateSubDirectoriesWithIcons(directoryInfo, rootNode, imageList);
        }

        private async Task PopulateSubDirectoriesWithIcons(DirectoryInfo directoryInfo, TreeNode parentNode, ImageList imageList)
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
                await PopulateSubDirectoriesWithIcons(directory, directoryNode, imageList);
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                var fileNode = new TreeNode(file.Name)
                {
                    Tag = file.FullName,
                    ImageKey = file.Extension == ".txt" ? "textFile" : "file",
                    SelectedImageKey = file.Extension == ".txt" ? "textFile" : "file"
                };
                parentNode.Nodes.Add(fileNode);
            }
        }
    }

}
