namespace ECUCodeEditor
{
    partial class createProject
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.addCard = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cardName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chooisedCard = new System.Windows.Forms.ComboBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.projectName2 = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chooiseBrowse = new System.Windows.Forms.Button();
            this.workSpacePath = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.addedCards = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cardInformations = new System.Windows.Forms.ListView();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.addCard);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox6);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.groupBox5);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(657, 614);
            this.panel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(16, 152);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Add a new project";
            // 
            // addCard
            // 
            this.addCard.Location = new System.Drawing.Point(15, 373);
            this.addCard.Margin = new System.Windows.Forms.Padding(2);
            this.addCard.Name = "addCard";
            this.addCard.Size = new System.Drawing.Size(197, 32);
            this.addCard.TabIndex = 5;
            this.addCard.Text = "Add ECU";
            this.addCard.UseVisualStyleBackColor = true;
            this.addCard.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cardName);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Location = new System.Drawing.Point(15, 313);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(426, 47);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ECU Name";
            // 
            // cardName
            // 
            this.cardName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardName.Location = new System.Drawing.Point(2, 15);
            this.cardName.Margin = new System.Windows.Forms.Padding(2);
            this.cardName.Name = "cardName";
            this.cardName.Size = new System.Drawing.Size(422, 20);
            this.cardName.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chooisedCard);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Location = new System.Drawing.Point(15, 258);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(426, 50);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Chooise ECU";
            // 
            // chooisedCard
            // 
            this.chooisedCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chooisedCard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.chooisedCard.FormattingEnabled = true;
            this.chooisedCard.Location = new System.Drawing.Point(2, 15);
            this.chooisedCard.Margin = new System.Windows.Forms.Padding(2);
            this.chooisedCard.Name = "chooisedCard";
            this.chooisedCard.Size = new System.Drawing.Size(422, 21);
            this.chooisedCard.TabIndex = 0;
            this.chooisedCard.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.projectName2);
            this.groupBox6.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox6.Location = new System.Drawing.Point(17, 183);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox6.Size = new System.Drawing.Size(424, 47);
            this.groupBox6.TabIndex = 11;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Project Name";
            // 
            // projectName2
            // 
            this.projectName2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectName2.Location = new System.Drawing.Point(2, 15);
            this.projectName2.Margin = new System.Windows.Forms.Padding(2);
            this.projectName2.Name = "projectName2";
            this.projectName2.Size = new System.Drawing.Size(420, 20);
            this.projectName2.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(657, 129);
            this.panel2.TabIndex = 7;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.08564F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.91436F));
            this.tableLayoutPanel1.Controls.Add(this.chooiseBrowse, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.workSpacePath, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 71);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(657, 58);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // chooiseBrowse
            // 
            this.chooiseBrowse.Dock = System.Windows.Forms.DockStyle.Left;
            this.chooiseBrowse.Location = new System.Drawing.Point(429, 2);
            this.chooiseBrowse.Margin = new System.Windows.Forms.Padding(2);
            this.chooiseBrowse.Name = "chooiseBrowse";
            this.chooiseBrowse.Size = new System.Drawing.Size(56, 25);
            this.chooiseBrowse.TabIndex = 2;
            this.chooiseBrowse.Text = "Browse";
            this.chooiseBrowse.UseVisualStyleBackColor = true;
            this.chooiseBrowse.Click += new System.EventHandler(this.chooiseBrowse_Click);
            // 
            // workSpacePath
            // 
            this.workSpacePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workSpacePath.FormattingEnabled = true;
            this.workSpacePath.Location = new System.Drawing.Point(15, 5);
            this.workSpacePath.Margin = new System.Windows.Forms.Padding(15, 5, 15, 8);
            this.workSpacePath.Name = "workSpacePath";
            this.workSpacePath.Size = new System.Drawing.Size(397, 21);
            this.workSpacePath.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select a workspace ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "TENU Code Editor";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.deleteButton);
            this.groupBox5.Controls.Add(this.addedCards);
            this.groupBox5.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox5.Location = new System.Drawing.Point(242, 435);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(197, 122);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Added ECU";
            // 
            // deleteButton
            // 
            this.deleteButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.deleteButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.deleteButton.Location = new System.Drawing.Point(2, 92);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(2);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(193, 28);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // addedCards
            // 
            this.addedCards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addedCards.FormattingEnabled = true;
            this.addedCards.Location = new System.Drawing.Point(2, 15);
            this.addedCards.Margin = new System.Windows.Forms.Padding(2);
            this.addedCards.Name = "addedCards";
            this.addedCards.Size = new System.Drawing.Size(193, 105);
            this.addedCards.TabIndex = 0;
            // 
            // button3
            // 
            this.button3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button3.Location = new System.Drawing.Point(0, 579);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(657, 35);
            this.button3.TabIndex = 6;
            this.button3.Text = "Create Project";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cardInformations);
            this.groupBox4.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox4.Location = new System.Drawing.Point(17, 435);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(197, 122);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ECU Information";
            // 
            // cardInformations
            // 
            this.cardInformations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardInformations.HideSelection = false;
            this.cardInformations.Location = new System.Drawing.Point(2, 15);
            this.cardInformations.Margin = new System.Windows.Forms.Padding(2);
            this.cardInformations.Name = "cardInformations";
            this.cardInformations.Size = new System.Drawing.Size(193, 105);
            this.cardInformations.TabIndex = 0;
            this.cardInformations.UseCompatibleStateImageBehavior = false;
            this.cardInformations.View = System.Windows.Forms.View.List;
            // 
            // createProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 614);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(480, 627);
            this.Name = "createProject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "createPeoject";
            this.Load += new System.EventHandler(this.createProject_Load_1);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button addCard;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ListBox addedCards;
        private System.Windows.Forms.ComboBox chooisedCard;
        private System.Windows.Forms.ListView cardInformations;
        private System.Windows.Forms.TextBox cardName;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button chooiseBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox projectName2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox workSpacePath;
    }
}