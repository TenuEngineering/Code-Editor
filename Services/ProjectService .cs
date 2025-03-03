using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ECUCodeEditor.Core.Models;

namespace ECUCodeEditor.Services
{
    public class ProjectService
    {
        public List<Project> ReadProjectsFromFile(string filePath)
        {
            var projects = new List<Project>();

            if (File.Exists(filePath))
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    var parts = line.Split(',');

                    if (parts.Length == 3)
                    {
                        projects.Add(new Project
                        {
                            Name = parts[0],
                            Date = parts[1],
                            Path = parts[2]
                        });
                    }
                }
            }

            return projects;
        }

        public void PopulateTreeView(TreeView treeView, List<Project> projects, Func<string, string> getString)
        {
            var todayNode = new TreeNode(getString("HomePageTreeWievToday"));
            var yesterdayNode = new TreeNode(getString("HomePageTreeWievYesterday"));
            var thisWeekNode = new TreeNode(getString("HomePageTreeWievWeek"));

            foreach (var project in projects)
            {
                var projectNode = new TreeNode($"{project.Name}\n{project.Path}");

                if (project.Date == DateTime.Today.ToString("dd.MM.yyyy"))
                    todayNode.Nodes.Insert(0, projectNode);
                else if (project.Date == DateTime.Today.AddDays(-1).ToString("dd.MM.yyyy"))
                    yesterdayNode.Nodes.Insert(0, projectNode);
                else
                    thisWeekNode.Nodes.Insert(0, projectNode);
            }

            treeView.Nodes.Clear();
            treeView.Nodes.Add(todayNode);
            treeView.Nodes.Add(yesterdayNode);
            treeView.Nodes.Add(thisWeekNode);
        }
    }
}
