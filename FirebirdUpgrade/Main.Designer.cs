namespace FirebirdUpgrade
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label1 = new System.Windows.Forms.Label();
            this.txtScript = new System.Windows.Forms.TextBox();
            this.btnCreateScript = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblFirebirdPath = new System.Windows.Forms.Label();
            this.btnGetPath = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnGBAKPath = new System.Windows.Forms.Button();
            this.lblGBAKPath = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(353, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "FireBird Database Upgrader";
            // 
            // txtScript
            // 
            this.txtScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScript.Location = new System.Drawing.Point(207, 190);
            this.txtScript.Multiline = true;
            this.txtScript.Name = "txtScript";
            this.txtScript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtScript.Size = new System.Drawing.Size(670, 254);
            this.txtScript.TabIndex = 1;
            // 
            // btnCreateScript
            // 
            this.btnCreateScript.Location = new System.Drawing.Point(18, 190);
            this.btnCreateScript.Name = "btnCreateScript";
            this.btnCreateScript.Size = new System.Drawing.Size(179, 42);
            this.btnCreateScript.TabIndex = 2;
            this.btnCreateScript.Text = "Create script";
            this.btnCreateScript.UseVisualStyleBackColor = true;
            this.btnCreateScript.Click += new System.EventHandler(this.btnCreateScript_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(15, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(593, 41);
            this.label2.TabIndex = 3;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // lblFirebirdPath
            // 
            this.lblFirebirdPath.AutoSize = true;
            this.lblFirebirdPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFirebirdPath.Location = new System.Drawing.Point(217, 148);
            this.lblFirebirdPath.Name = "lblFirebirdPath";
            this.lblFirebirdPath.Size = new System.Drawing.Size(112, 24);
            this.lblFirebirdPath.TabIndex = 5;
            this.lblFirebirdPath.Text = "FirebirdPath";
            // 
            // btnGetPath
            // 
            this.btnGetPath.Location = new System.Drawing.Point(18, 141);
            this.btnGetPath.Name = "btnGetPath";
            this.btnGetPath.Size = new System.Drawing.Size(179, 43);
            this.btnGetPath.TabIndex = 6;
            this.btnGetPath.Text = "Set Firebird Gbak 2.# Path";
            this.btnGetPath.UseVisualStyleBackColor = true;
            this.btnGetPath.Click += new System.EventHandler(this.btnGetPath_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnGBAKPath
            // 
            this.btnGBAKPath.Location = new System.Drawing.Point(18, 92);
            this.btnGBAKPath.Name = "btnGBAKPath";
            this.btnGBAKPath.Size = new System.Drawing.Size(179, 43);
            this.btnGBAKPath.TabIndex = 8;
            this.btnGBAKPath.Text = "Set Firebird Gbak 1.# Path";
            this.btnGBAKPath.UseVisualStyleBackColor = true;
            this.btnGBAKPath.Click += new System.EventHandler(this.btnGBAKPath_Click);
            // 
            // lblGBAKPath
            // 
            this.lblGBAKPath.AutoSize = true;
            this.lblGBAKPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGBAKPath.Location = new System.Drawing.Point(217, 99);
            this.lblGBAKPath.Name = "lblGBAKPath";
            this.lblGBAKPath.Size = new System.Drawing.Size(103, 24);
            this.lblGBAKPath.TabIndex = 7;
            this.lblGBAKPath.Text = "GBAK Path";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 456);
            this.Controls.Add(this.btnGBAKPath);
            this.Controls.Add(this.lblGBAKPath);
            this.Controls.Add(this.btnGetPath);
            this.Controls.Add(this.lblFirebirdPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCreateScript);
            this.Controls.Add(this.txtScript);
            this.Controls.Add(this.label1);
            this.Name = "Main";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtScript;
        private System.Windows.Forms.Button btnCreateScript;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFirebirdPath;
        private System.Windows.Forms.Button btnGetPath;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnGBAKPath;
        private System.Windows.Forms.Label lblGBAKPath;
    }
}