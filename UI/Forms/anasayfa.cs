using Microsoft.Build.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tester.Properties;
using System.Resources;
using System.Globalization;
using System.Threading;
using static Tester.createProject;
using System.Security.Cryptography;
using System.Reflection;
using Tester.Services;
using Tester.Core.Models;

namespace Tester
{
    public partial class anasayfa : Form
    {
        private readonly LanguageService _languageService;
        private readonly ProjectService _projectService;
        private string _lanCulture;


        ResourceManager resManager;    // Resource manager to access resx files
        CultureInfo cultureInfo;
        string lanCulture;
        public anasayfa()
        {
            InitializeComponent();

            // Servislerin başlatılması
            _languageService = new LanguageService("Tester.Languages.String", typeof(anasayfa).Assembly);
            _projectService = new ProjectService();

            // Varsayılan dil
            _languageService.ChangeLanguage("en");

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        // resx sözlüğünden veriler tüm uygulamada bu fonksiyondan alınacak


        private void button1_Click(object sender, EventArgs e)
        {
            createProject yn = new createProject(lanCulture,resManager,cultureInfo);
            yn.Show();
            this.Hide();
        }

        private void anasayfa_Load(object sender, EventArgs e)
        {
            LoadProjectsIntoTreeView();
        }

        private void LoadProjectsIntoTreeView()
        {
            var projects = _projectService.ReadProjectsFromFile(@"SONPROJELER.txt");

            var validProjects = projects.Where(p => Directory.Exists(p.Path)).ToList();

            var todayNode = new TreeNode(_languageService.GetString("HomePageTreeWievToday"));
            var yesterdayNode = new TreeNode(_languageService.GetString("HomePageTreeWievYesterday"));
            var thisWeekNode = new TreeNode(_languageService.GetString("HomePageTreeWievWeek"));

            foreach (var project in projects)
            {
                var projectNode = new TreeNode($"{project.Name}\n{project.Path}");
                string today = DateTime.Today.Day.ToString();
                string month = DateTime.Today.Month.ToString();
{} ;               string year = DateTime.Today.Year.ToString();


                string day = $"{today}.{month}.{year}";


                //MessageBox.Show("şuanki tarih: " + DateTime.Today.ToString().Replace("/"," ") + "\nproje tarihi: " + project.Date.Replace("."," "));
                if (project.Date == day)
                    InsertNodeAtTop(todayNode, projectNode);
                else if (project.Date == DateTime.Today.AddDays(-1).ToString().Replace("/", " "))
                    InsertNodeAtTop(yesterdayNode, projectNode);
                else
                {
                    //MessageBox.Show("haftada");
                    InsertNodeAtTop(thisWeekNode, projectNode);
                }

                    

            }
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(todayNode);
            treeView1.Nodes.Add(yesterdayNode);
            treeView1.Nodes.Add(thisWeekNode);
        }

        private void InsertNodeAtTop(TreeNode parentNode, TreeNode newNode)
        {
            parentNode.Nodes.Insert(0, newNode);
        }

        private async void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            await Task.Delay(50);
            await openProject(e);


        }

        private async Task openProject(TreeViewEventArgs e)
        {
            await Task.Delay(100);

            if (e.Node != null && !string.IsNullOrEmpty(e.Node.Text))
            {
                string selectedworkspaceFullPath = string.Empty;

                var parts = e.Node.Text.Split('\n');
                
                if (parts.Length > 1)
                {
                    selectedworkspaceFullPath = parts[1].Trim();
                }
                else
                {
                    return;
                }
                if (Directory.Exists(selectedworkspaceFullPath))
                {
                    PowerfulCSharpEditor powerfulCSharpEditorForm = new PowerfulCSharpEditor(selectedworkspaceFullPath);
                    powerfulCSharpEditorForm.FormClosed += (s, args) => this.Close();

                    powerfulCSharpEditorForm.Show();

                    this.Hide();
                }

                else
                {
                    MessageBox.Show(_languageService.GetString("ProjectNotFound"));
                }
            }

        }

        private void languages_SelectedValueChanged(object sender, EventArgs e)
        {
            if (languages.SelectedItem.ToString() == "English")
            {
                _languageService.ChangeLanguage("en");
                button1.Text = _languageService.GetString("HomePageButton1");
                button2.Text = _languageService.GetString("HomePageButton2");
                label1.Text = _languageService.GetString("HomePageProjectInfo");
            }
            else if (languages.SelectedItem.ToString() == "Turkish")
            {
                
                _languageService.ChangeLanguage("tr");
                button1.Text = _languageService.GetString("HomePageButton1");
                button2.Text = _languageService.GetString("HomePageButton2");
                label1.Text = _languageService.GetString("HomePageProjectInfo");
            }
            LoadProjectsIntoTreeView();
        }
    }


}