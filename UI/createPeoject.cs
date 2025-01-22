using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography;

namespace Tester
{
    public partial class createProject : Form
    {

        private bool isSubFolderCreation;
        private string projectName;
        private Thread trd;

        ResourceManager resManager;
        CultureInfo cultureInfo;

        public createProject(string projectName, bool isSubFolderCreation)
        {
            InitializeComponent();
            this.projectName = projectName;
            this.isSubFolderCreation = isSubFolderCreation; // Alt klasör oluşturma 
        }

        public createProject(string langCode, ResourceManager resManager2, CultureInfo cultureInfo)
        {
            InitializeComponent();
            resManager = resManager2;

            cultureInfo = new CultureInfo(langCode);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            if (langCode != "en")
            {

                label2.Text = GetString("CreateProjectWorkspace");
                chooiseBrowse.Text = GetString("CreateProjectBrowseBtn");
                label3.Text = GetString("CreateProjectLabelAdd");
                groupBox6.Text = GetString("CreateProjectProjectName");
                groupBox3.Text = GetString("CreateProjectChooiceECU");
                groupBox2.Text = GetString("CreateProjectECUName");
                addCard.Text = GetString("CreateProjectAddECU");
                groupBox4.Text = GetString("CreateProjectECUInfo");
                groupBox5.Text = GetString("CreateProjectAddedECU");
                deleteButton.Text = GetString("CreateProjectDelete");
                button3.Text = GetString("HomePageButton1");

            }

        }


        public createProject()
        {
            InitializeComponent();
        }

        ArrayList kartisim = new ArrayList();
        ArrayList kartno = new ArrayList();
        bool kontrol = false, olusturkontrol;

        private string GetString(string name)
        {

            return resManager.GetString(name);
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(workSpacePath.Text.ToString()))
            {

                string workspace = workSpacePath.SelectedItem.ToString();
                await createProjectTh(workspace);


            }
            else
            {
                MessageBox.Show(GetString("ChooseWorkspace"));
            }


        }

        private async Task createProjectTh(string workspace)
        {

            bool isError = false;
            StringBuilder errorMessages = new StringBuilder();

            await Task.Delay(100);
            if (string.IsNullOrEmpty(projectName2.Text))
            {
                errorMessages.AppendLine(GetString("warningCreateProject2"));
                isError = true;
            }

            if (string.IsNullOrEmpty(cardName.Text))
            {
                errorMessages.AppendLine(GetString("warningCreateProject3"));
                isError = true;
            }

            if (chooisedCard.SelectedItem == null)
            {
                errorMessages.AppendLine(GetString("warningCreateProject4"));
                isError = true;
            }


            if (isError)
            {
                if (errorMessages.ToString().Split('\n').Length > 2)
                {
                    MessageBox.Show(GetString("warningCreateProject1"), "TENU", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(errorMessages.ToString(), "TENU", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }
            if (!isKartAdded)
            {
                MessageBox.Show(GetString("warningCreateProject5"), "TENU", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isSubFolderCreation)
            {
                string subFolderPath = Path.Combine(workspace, projectName2.Text);
                string workspaceFullPath = Path.Combine(subFolderPath, cardName.Text);


                if (Directory.Exists(workspaceFullPath))
                {
                    DialogResult a, b;
                    a = MessageBox.Show(GetString("warningCreateProject6"), "TENU", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (a == DialogResult.Yes)
                    {
                        b = MessageBox.Show(cardName.Text.ToUpper() + GetString("warningCreateProject7"), "TENU", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (b == DialogResult.Yes)
                        {
                            if (Directory.Exists(workspaceFullPath)) Directory.Delete(workspaceFullPath, true);
                        }

                        else return;
                    }
                    else
                    {
                        return;
                    }
 
                }

                Directory.CreateDirectory(workspaceFullPath);

                Directory.CreateDirectory(workspaceFullPath + "\\USER_DATA");
                Directory.CreateDirectory(workspaceFullPath + "\\USER_DATA\\Functions");
                Directory.CreateDirectory(workspaceFullPath + "\\USER_DATA\\Functions");
                Directory.CreateDirectory(workspaceFullPath + "\\SYSTEM_DATA");
                Directory.CreateDirectory(workspaceFullPath + "\\SYSTEM_DATA\\Functions");
                Directory.CreateDirectory(workspaceFullPath + "\\USER_DATA\\Variable");

                StreamWriter userWriter = new StreamWriter(workspaceFullPath + "\\USER_DATA\\CODE.txt");
                userWriter.Close();

                StreamWriter userWriter2 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Variable\\flags.txt");
                StreamWriter userWriter3 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Variable\\uint8_t.txt");
                StreamWriter userWriter4 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Variable\\uint16_t.txt");
                StreamWriter userWriter5 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Variable\\uint32_t.txt");
                StreamWriter userWriter6 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Variable\\float32_t.txt");
                StreamWriter userWriter7 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Variable\\float64_t.txt"); // double karşılık gelir
                StreamWriter userWriter8 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Variable\\uint8_array_t.txt");
                StreamWriter userWriter9 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Variable\\uint16_array_t.txt");
                StreamWriter userWriter13 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Variable\\uint32_array_t.txt");
                StreamWriter userWriter10 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Variable\\float32_array_t.txt");
                StreamWriter userWriter11 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Variable\\float64_array_t.txt"); // double array karşılık gelir
                StreamWriter userWriter12 = new StreamWriter(workspaceFullPath + "\\USER_DATA\\Functions\\Example.txt"); // double array karşılık gelir
                StreamWriter userWriter14 = new StreamWriter(workspaceFullPath + "\\SYSTEM_DATA\\Functions\\Example.txt"); // double array karşılık gelir


                
                userWriter2.Close();
                userWriter3.Close();
                userWriter4.Close();
                userWriter5.Close();

                StreamWriter systemWriter = new StreamWriter(workspaceFullPath + "\\SYSTEM_DATA\\INFO.txt");
                await systemWriter.WriteLineAsync("KART ADI:" + chooisedCard.SelectedItem.ToString().Split('_')[0]);
                await systemWriter.WriteLineAsync("VERSIYON:" + chooisedCard.SelectedItem.ToString().Split('_')[1]);
                systemWriter.Close();

                MessageBox.Show(GetString("warningCreateProject8"), "TENU", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PowerfulCSharpEditor PowerfulCSharpEditorForm = new PowerfulCSharpEditor();

                await Task.Delay(100);
                PowerfulCSharpEditorForm.Show();
                this.Hide();

            }
            else
            {

                string subFolderPath = Path.Combine(workspace, projectName2.Text);


                olusturkontrol = false;
                if (Directory.Exists(subFolderPath))
                {
                    DialogResult a, b;
                    a = MessageBox.Show(GetString("ProjectFolderExistsWarning"), "TENU", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (a == DialogResult.Yes)
                    {
                        b = MessageBox.Show(projectName2.Text.ToUpper() + " " +GetString("warningCreateProject9"), "TENU", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (b == DialogResult.Yes)
                        {
                            if (Directory.Exists(subFolderPath)) Directory.Delete(subFolderPath, true);
                            olusturkontrol = false;
                        }
                        else olusturkontrol = true;

                    }
                    else
                    {
                        olusturkontrol = true;

                    }
                }

                if (!olusturkontrol)
                {
                    Directory.CreateDirectory(workspace + "\\" + projectName2.Text);
                    for (int i = 0; i < kartisim.Count; i++) Directory.CreateDirectory(subFolderPath + "\\" + kartisim[i].ToString());
                    for (int i = 0; i < kartisim.Count; i++)
                    {
                        Directory.CreateDirectory(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA");
                        Directory.CreateDirectory(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Functions");
                        Directory.CreateDirectory(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\SYSTEM_DATA");
                        Directory.CreateDirectory(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\SYSTEM_DATA\\Functions");
                        Directory.CreateDirectory(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable");
                    } 
                     
                    for (int i = 0; i < kartisim.Count; i++)
                    {
                        StreamWriter Yaz = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\CODE.txt");
                        Yaz.Close();
                        StreamWriter Yaz1 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\SYSTEM_DATA\\INFO.txt");
                        await Yaz1.WriteLineAsync("KART ADI:" + kartno[i].ToString().Split('_')[0]);
                        await Yaz1.WriteLineAsync("VERSIYON:" + kartno[i].ToString().Split('_')[1]);
                        Yaz1.Close();
                        
                        StreamWriter userWriter2 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable\\flags.txt");
                        StreamWriter userWriter3 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable\\uint8_t.txt");
                        StreamWriter userWriter4 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable\\uint16_t.txt");
                        StreamWriter userWriter5 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable\\uint32_t.txt");
                        StreamWriter userWriter6 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable\\float32_t.txt");
                        StreamWriter userWriter7 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable\\float64_t.txt"); // double karşılık gelir
                        StreamWriter userWriter8 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable\\uint8_array_t.txt"); 
                        StreamWriter userWriter9 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable\\uint16_array_t.txt"); 
                        StreamWriter userWriter12 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable\\uint32_array_t.txt"); 
                        StreamWriter userWriter10 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable\\float32_array_t.txt"); 
                        StreamWriter userWriter11 = new StreamWriter(subFolderPath + "/" + kartisim[i].ToString().ToUpper() + "\\USER_DATA\\Variable\\float64_array_t.txt"); // double array karşılık gelir


                        userWriter2.Close();
                        userWriter3.Close();
                        userWriter4.Close();
                        userWriter5.Close();

                    }




                    string dosyaYolu = @"SONPROJELER.txt";
                    using (StreamWriter writer = new StreamWriter(dosyaYolu, true))
                    {
                        string today = DateTime.Today.Day.ToString();
                        string month = DateTime.Today.Month.ToString();
                        string year = DateTime.Today.Year.ToString();
                        await writer.WriteLineAsync($"{projectName2.Text},{today}.{month}.{year},{subFolderPath}");
                        
                    }

                    MessageBox.Show(GetString("warningCreateProject10"), "TENU", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PowerfulCSharpEditor PowerfulCSharpEditorForm = new PowerfulCSharpEditor();

                    PowerfulCSharpEditor powerfulCSharpEditor = new PowerfulCSharpEditor(workspace);

                    powerfulCSharpEditor.Show();
                    this.Close();
                }
            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            {
                if (addedCards.SelectedItem != null)
                {
                    string selectedItem = addedCards.SelectedItem.ToString();
                    addedCards.Items.Remove(addedCards.SelectedItem);

                    for (int i = cardInformations.Items.Count - 1; i >= 0; i--)
                    {
                        ListViewItem item = cardInformations.Items[i];

                        if (item.Text.StartsWith(selectedItem + ":"))
                        {
                            cardInformations.Items.Remove(item);

                            while (i < cardInformations.Items.Count && cardInformations.Items[i].Text.StartsWith("  "))
                            {
                                cardInformations.Items.RemoveAt(i);
                            }
                            break;
                        }
                    }

                    int index = kartisim.IndexOf(selectedItem);
                    if (index != -1)
                    {
                        kartisim.RemoveAt(index);
                        kartno.RemoveAt(index);
                    }

                    cardInformations.Items.Clear();
                }
            }
        }

        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chooisedCard.SelectedItem != null)
            {

                string kartNo = chooisedCard.SelectedItem.ToString();

                List<string> kartProperties = await ReadKartProperties(kartNo);

                cardInformations.Items.Clear();
                cardInformations.View = View.Details;
                cardInformations.FullRowSelect = true;
                cardInformations.Columns.Clear();

                // Sütunlar ekleyelim
                cardInformations.Columns.Add("", -2, HorizontalAlignment.Center);

                if (kartProperties != null && kartProperties.Count > 0)
                {
                    // Başlık
                    string kartInfoHeader = $"********* {kartNo} **********";
                    ListViewItem headerItem = new ListViewItem(kartInfoHeader);
                    headerItem.Font = new Font(cardInformations.Font, FontStyle.Bold);
                    cardInformations.Items.Add(headerItem);

                    // Özellikler
                    foreach (string property in kartProperties)
                    {
                        ListViewItem item = new ListViewItem("* " + property.PadRight(40) + " *");  // Özellikleri düzenleyelim
                        cardInformations.Items.Add(item);
                    }

                    // Son satıra bir sınır çizgisi ekleyelim
                    cardInformations.Items.Add(new ListViewItem(new string('*', 40)));
                }
                else
                {
                    cardInformations.Items.Add(new ListViewItem("Kart özellikleri bulunamadı."));
                }

                // Sütun genişliğini ayarlayalım
                cardInformations.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }
        private async Task<List<string>> ReadKartProperties(string kartNo)
        {
            string kartFilePath = $"DOSYALAR/{kartNo}/{kartNo}.txt";
            List<string> properties = new List<string>();

            await Task.Delay(100);
            if (File.Exists(kartFilePath))
            {
                string[] lines = File.ReadAllLines(kartFilePath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("KartBilgileri:"))
                    {
                        string[] items = line.Substring("KartBilgileri:".Length).Trim().Split(',');
                        properties.AddRange(items.Select(item => item.Trim()));
                    }
                }
            }
            return properties;


        }

       
        private bool isKartAdded = false;
        private async void button1_Click(object sender, EventArgs e)
        {
            bool isError = false;
            StringBuilder errorMessages = new StringBuilder();


            if (string.IsNullOrEmpty(cardName.Text))
            {
                errorMessages.AppendLine(GetString("warningCreateProject3"));
                isError = true;
            }

            if (chooisedCard.SelectedItem == null)
            {
                errorMessages.AppendLine(GetString("warningCreateProject4"));
                isError = true;
            }


            if (isError)
            {
                if (errorMessages.ToString().Split('\n').Length > 2)
                {
                    MessageBox.Show(GetString("warningCreateProject1"), "TENU", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(errorMessages.ToString(), "TENU", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }
            if (chooisedCard.SelectedItem != null && !string.IsNullOrEmpty(cardName.Text))
            {
                string kartIsmi = cardName.Text;
                string kartNo = chooisedCard.SelectedItem.ToString();
                // string kartBilgisi = kartIsmi + " (" + kartNo + ")";

                if (!addedCards.Items.Contains(kartIsmi))
                {
                    addedCards.Items.Add(kartIsmi);
                    kartisim.Add(kartIsmi);
                    kartno.Add(kartNo);

                    List<string> kartProperties = await ReadKartProperties(kartNo);
                    if (kartProperties != null && kartProperties.Count > 0)
                    {
                        cardInformations.Items.Clear();
                        string kartInfoHeader = $"{kartNo}:";
                        cardInformations.Items.Add(new ListViewItem(kartInfoHeader));

                        foreach (string property in kartProperties)
                        {
                            ListViewItem item = new ListViewItem("  " + property);
                            cardInformations.Items.Add(item);
                        }
                        isKartAdded = true;

                    }
                }
                else
                {
                    MessageBox.Show(GetString("warningCreateProject11"), "TENU", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            anasayfa anasayfaForm = new anasayfa();
            anasayfaForm.Show();
            this.Close();
        }

        private async void createProject_Load_1(object sender, EventArgs e)
        {
            StreamReader st = File.OpenText("DOSYALAR/KARTLAR.txt");


            string yaz;
            while ((yaz = st.ReadLine()) != null)
            {
                chooisedCard.Items.Add(yaz);
            }
            st.Close();

            if (isSubFolderCreation)
            {
                projectName2.Text = projectName;
                projectName2.ReadOnly = true;
            }
            else
            {
                projectName2.ReadOnly = false;
            }
        }


        private async void chooiseBrowse_Click(object sender, EventArgs e)
        {
            await Task.Delay(200);
            using (var openFileDialog = new OpenFileDialog())
            {
                
                openFileDialog.ValidateNames = false;
                openFileDialog.CheckFileExists = false;
                openFileDialog.CheckPathExists = true;
                openFileDialog.FileName = "Klasör Seç"; // Kullanıcıya dosya seçmesini engellemek için sahte bir dosya adı gösteriyoruz

                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Seçilen klasör yolu
                    string selectedPath = Path.GetDirectoryName(openFileDialog.FileName);
                    

                    workSpacePath.Items.Add(selectedPath);
                    // Yeni eklenen öğeyi seçili yap
                    workSpacePath.SelectedIndex = workSpacePath.Items.Count - 1;
                }
            }



        }



    }
}
