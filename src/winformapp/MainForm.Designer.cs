namespace ConditionalAccessDocumenter
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            lblUserProfileName = new Label();
            label4 = new Label();
            label5 = new Label();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            lblStatus = new Label();
            progressBar = new ProgressBar();
            label7 = new Label();
            label3 = new Label();
            txtSaveFolderPath = new TextBox();
            btnGenerate = new Button();
            tabPage2 = new TabPage();
            btnResetSettings = new Button();
            txtTenantId = new TextBox();
            label1 = new Label();
            txtClientId = new TextBox();
            label2 = new Label();
            label6 = new Label();
            tabPage3 = new TabPage();
            btnSignIn = new Button();
            txtManualCaPolicy = new RichTextBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // lblUserProfileName
            // 
            lblUserProfileName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUserProfileName.AutoSize = true;
            lblUserProfileName.Location = new Point(982, 27);
            lblUserProfileName.Name = "lblUserProfileName";
            lblUserProfileName.Size = new Size(173, 32);
            lblUserProfileName.TabIndex = 1;
            lblUserProfileName.Text = "[Not signed in]";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(35, 27);
            label4.Name = "label4";
            label4.Size = new Size(700, 65);
            label4.TabIndex = 10;
            label4.Text = "Conditional Access Documenter";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(48, 111);
            label5.Name = "label5";
            label5.Size = new Size(895, 32);
            label5.TabIndex = 11;
            label5.Text = "Visualize your conditional access policies with the Conditional Access Documenter.\r\n";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(48, 177);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1096, 579);
            tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(txtManualCaPolicy);
            tabPage1.Controls.Add(lblStatus);
            tabPage1.Controls.Add(progressBar);
            tabPage1.Controls.Add(label7);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(txtSaveFolderPath);
            tabPage1.Controls.Add(btnGenerate);
            tabPage1.Location = new Point(8, 46);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1080, 525);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Automatic Generation";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(49, 315);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(392, 32);
            lblStatus.TabIndex = 20;
            lblStatus.Text = "Presentation generated succesfully!";
            // 
            // progressBar
            // 
            progressBar.Location = new Point(405, 205);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(308, 46);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 19;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(42, 19);
            label7.Name = "label7";
            label7.Size = new Size(976, 32);
            label7.TabIndex = 18;
            label7.Text = "See the setup guide for instructions on creating a custom app registration for your tenant.";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(42, 85);
            label3.Name = "label3";
            label3.Size = new Size(333, 32);
            label3.TabIndex = 15;
            label3.Text = "Save presentation to folder:";
            // 
            // txtSaveFolderPath
            // 
            txtSaveFolderPath.Location = new Point(42, 120);
            txtSaveFolderPath.Name = "txtSaveFolderPath";
            txtSaveFolderPath.Size = new Size(714, 39);
            txtSaveFolderPath.TabIndex = 14;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(42, 182);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(293, 94);
            btnGenerate.TabIndex = 11;
            btnGenerate.Text = "Generate presentation";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(btnResetSettings);
            tabPage2.Controls.Add(txtTenantId);
            tabPage2.Controls.Add(label1);
            tabPage2.Controls.Add(txtClientId);
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(label6);
            tabPage2.Location = new Point(8, 46);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1080, 525);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Setup guide";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnResetSettings
            // 
            btnResetSettings.Location = new Point(652, 366);
            btnResetSettings.Name = "btnResetSettings";
            btnResetSettings.Size = new Size(222, 46);
            btnResetSettings.TabIndex = 22;
            btnResetSettings.Text = "Reset to defaults";
            btnResetSettings.UseVisualStyleBackColor = true;
            btnResetSettings.Click += btnResetSettings_Click;
            // 
            // txtTenantId
            // 
            txtTenantId.Location = new Point(42, 469);
            txtTenantId.Name = "txtTenantId";
            txtTenantId.Size = new Size(545, 39);
            txtTenantId.TabIndex = 21;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(42, 427);
            label1.Name = "label1";
            label1.Size = new Size(261, 32);
            label1.TabIndex = 20;
            label1.Text = "Directory (tenant) ID:";
            // 
            // txtClientId
            // 
            txtClientId.Location = new Point(42, 373);
            txtClientId.Name = "txtClientId";
            txtClientId.Size = new Size(545, 39);
            txtClientId.TabIndex = 19;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(37, 331);
            label2.Name = "label2";
            label2.Size = new Size(274, 32);
            label2.TabIndex = 18;
            label2.Text = "Application (client) ID:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(30, 19);
            label6.Name = "label6";
            label6.Size = new Size(973, 288);
            label6.TabIndex = 12;
            label6.Text = resources.GetString("label6.Text");
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(8, 46);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1080, 525);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Manual Generation";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnSignIn
            // 
            btnSignIn.Location = new Point(994, 97);
            btnSignIn.Name = "btnSignIn";
            btnSignIn.Size = new Size(150, 46);
            btnSignIn.TabIndex = 10;
            btnSignIn.Text = "&Sign In";
            btnSignIn.UseVisualStyleBackColor = true;
            btnSignIn.Click += btnSignIn_Click;
            // 
            // txtManualCaPolicy
            // 
            txtManualCaPolicy.Location = new Point(513, 277);
            txtManualCaPolicy.Name = "txtManualCaPolicy";
            txtManualCaPolicy.Size = new Size(514, 192);
            txtManualCaPolicy.TabIndex = 22;
            txtManualCaPolicy.Text = "";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1205, 809);
            Controls.Add(tabControl1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(lblUserProfileName);
            Controls.Add(btnSignIn);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            Text = "idPowerApp - Conditional Access Documenter";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lblUserProfileName;
        private Label label4;
        private Label label5;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private Label label7;
        private Label label3;
        private TextBox txtSaveFolderPath;
        private Button btnGenerate;
        private Button btnSignIn;
        private TabPage tabPage2;
        private Label label6;
        private ProgressBar progressBar;
        private Button btnResetSettings;
        private TextBox txtTenantId;
        private Label label1;
        private TextBox txtClientId;
        private Label label2;
        private Label lblStatus;
        private TabPage tabPage3;
        private RichTextBox txtManualCaPolicy;
    }
}