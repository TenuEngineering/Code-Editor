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

namespace Tester
{
    public partial class anasayfa : Form
    {
        ResourceManager resManager;    // Resource manager to access resx files
        CultureInfo cultureInfo;
        string lanCulture;
        public anasayfa()
        {
            InitializeComponent();
            LoadLanguages();

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void LoadLanguages()
        {
            // ResX dosyalarını kullanmak için ResourceManager
            resManager = new ResourceManager("Tester.Languages.String", Assembly.GetExecutingAssembly());
            ChangeLanguage("en");
            lanCulture = "en";
        }
        private void ChangeLanguage(string langCode)
        {
            cultureInfo = new CultureInfo(langCode);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            // Seçilen dildeki label text'ini güncelle
            //labelSelectedLanguage.Text = resManager.GetString("SelectedLanguageLabel", cultureInfo);
            button1.Text = GetString("HomePageButton1");
            button2.Text = GetString("HomePageButton2");
            label1.Text = GetString("HomePageProjectInfo");
            lanCulture = langCode;
            LoadProjectsIntoTreeView();

        }

        // resx sözlüğünden veriler tüm uygulamada bu fonksiyondan alınacak
        private string GetString(string name)
        {
            
            return resManager.GetString(name);
        }


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
            var projects = ReadProjectsFromFile(@"SONPROJELER.txt");

            var validProjects = projects.Where(p => Directory.Exists(p.Path)).ToList();

            var todayNode = new TreeNode(GetString("HomePageTreeWievToday"));
            var yesterdayNode = new TreeNode(GetString("HomePageTreeWievYesterday"));
            var thisWeekNode = new TreeNode(GetString("HomePageTreeWievWeek"));

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

        private List<Project> ReadProjectsFromFile(string filePath)
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
                            Path = parts[2],
                            Date = parts[1]
                        });
                    }
                }
            
            }

            return projects;
        }

        public class Project
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Date { get; set; }
        }

        private async void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            await Task.Delay(50);
            await openProject(e);


        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

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
                    MessageBox.Show(GetString("ProjectNotFound"));
                }
            }

        }

        private void languages_SelectedValueChanged(object sender, EventArgs e)
        {
            if (languages.SelectedItem.ToString() == "English")
            {
                ChangeLanguage("en");
            }
            else if (languages.SelectedItem.ToString() == "Turkish")
            {
                
                ChangeLanguage("tr");
            }
        }
    }


}