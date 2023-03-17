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
            lblUserProfileName = new Label();
            btnSignIn = new Button();
            btnGenerate = new Button();
            label2 = new Label();
            txtClientId = new TextBox();
            txtSaveFilePath = new TextBox();
            label3 = new Label();
            txtTenantId = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // lblUserProfileName
            // 
            lblUserProfileName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblUserProfileName.AutoSize = true;
            lblUserProfileName.Location = new Point(827, 9);
            lblUserProfileName.Name = "lblUserProfileName";
            lblUserProfileName.Size = new Size(173, 32);
            lblUserProfileName.TabIndex = 1;
            lblUserProfileName.Text = "[Not signed in]";
            // 
            // btnSignIn
            // 
            btnSignIn.Location = new Point(791, 88);
            btnSignIn.Name = "btnSignIn";
            btnSignIn.Size = new Size(150, 46);
            btnSignIn.TabIndex = 2;
            btnSignIn.Text = "&Sign In";
            btnSignIn.UseVisualStyleBackColor = true;
            btnSignIn.Click += btnSignIn_Click;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(41, 366);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(293, 94);
            btnGenerate.TabIndex = 3;
            btnGenerate.Text = "Generate presentation";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(41, 50);
            label2.Name = "label2";
            label2.Size = new Size(166, 32);
            label2.TabIndex = 4;
            label2.Text = "Application Id:";
            // 
            // txtClientId
            // 
            txtClientId.Location = new Point(46, 92);
            txtClientId.Name = "txtClientId";
            txtClientId.Size = new Size(714, 39);
            txtClientId.TabIndex = 5;
            txtClientId.Text = "e580347d-d0aa-4aa1-9113-5daa0bb1c805";
            // 
            // txtSaveFilePath
            // 
            txtSaveFilePath.Location = new Point(41, 304);
            txtSaveFilePath.Name = "txtSaveFilePath";
            txtSaveFilePath.Size = new Size(714, 39);
            txtSaveFilePath.TabIndex = 6;
            txtSaveFilePath.Text = "C:\\Users\\meferna\\Documents\\CAdoc.pptx";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(41, 269);
            label3.Name = "label3";
            label3.Size = new Size(279, 32);
            label3.TabIndex = 7;
            label3.Text = "Save presentation to file:";
            // 
            // txtTenantId
            // 
            txtTenantId.Location = new Point(51, 192);
            txtTenantId.Name = "txtTenantId";
            txtTenantId.Size = new Size(714, 39);
            txtTenantId.TabIndex = 9;
            txtTenantId.Text = "0817c655-a853-4d8f-9723-3a333b5b9235";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(46, 150);
            label1.Name = "label1";
            label1.Size = new Size(118, 32);
            label1.TabIndex = 8;
            label1.Text = "Tenant Id:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1012, 552);
            Controls.Add(txtTenantId);
            Controls.Add(label1);
            Controls.Add(label3);
            Controls.Add(txtSaveFilePath);
            Controls.Add(txtClientId);
            Controls.Add(label2);
            Controls.Add(btnGenerate);
            Controls.Add(btnSignIn);
            Controls.Add(lblUserProfileName);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "MainForm";
            Text = "Conditional Access Documenter";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lblUserProfileName;
        private Button btnSignIn;
        private Button btnGenerate;
        private Label label2;
        private TextBox txtClientId;
        private TextBox txtSaveFilePath;
        private Label label3;
        private TextBox txtTenantId;
        private Label label1;
    }
}