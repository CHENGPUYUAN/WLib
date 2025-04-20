
namespace WLib.Samples.WinForm
{
    partial class EPSMergeForm
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
            this.buttonAdd = new System.Windows.Forms.Button();
            this.textBoxOutPath = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonMerge = new System.Windows.Forms.Button();
            this.buttonSelectOutPath = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.radioButtonGDB = new System.Windows.Forms.RadioButton();
            this.radioButtonEPS = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(501, 40);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(2);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(56, 18);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "添加";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // textBoxOutPath
            // 
            this.textBoxOutPath.Location = new System.Drawing.Point(18, 278);
            this.textBoxOutPath.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxOutPath.Name = "textBoxOutPath";
            this.textBoxOutPath.Size = new System.Drawing.Size(463, 21);
            this.textBoxOutPath.TabIndex = 2;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 342);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(600, 18);
            this.progressBar1.TabIndex = 3;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(501, 82);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(2);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(56, 18);
            this.buttonRemove.TabIndex = 4;
            this.buttonRemove.Text = "移除";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonMerge
            // 
            this.buttonMerge.Location = new System.Drawing.Point(501, 318);
            this.buttonMerge.Margin = new System.Windows.Forms.Padding(2);
            this.buttonMerge.Name = "buttonMerge";
            this.buttonMerge.Size = new System.Drawing.Size(77, 18);
            this.buttonMerge.TabIndex = 5;
            this.buttonMerge.Text = "开始合并";
            this.buttonMerge.UseVisualStyleBackColor = true;
            this.buttonMerge.Click += new System.EventHandler(this.buttonMerge_Click);
            // 
            // buttonSelectOutPath
            // 
            this.buttonSelectOutPath.Location = new System.Drawing.Point(501, 278);
            this.buttonSelectOutPath.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSelectOutPath.Name = "buttonSelectOutPath";
            this.buttonSelectOutPath.Size = new System.Drawing.Size(77, 18);
            this.buttonSelectOutPath.TabIndex = 6;
            this.buttonSelectOutPath.Text = "输出路径";
            this.buttonSelectOutPath.UseVisualStyleBackColor = true;
            this.buttonSelectOutPath.Click += new System.EventHandler(this.buttonSelectOutPath_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(18, 25);
            this.listBox1.Margin = new System.Windows.Forms.Padding(2);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(463, 232);
            this.listBox1.TabIndex = 7;
            // 
            // radioButtonGDB
            // 
            this.radioButtonGDB.AutoSize = true;
            this.radioButtonGDB.Location = new System.Drawing.Point(13, 49);
            this.radioButtonGDB.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonGDB.Name = "radioButtonGDB";
            this.radioButtonGDB.Size = new System.Drawing.Size(83, 16);
            this.radioButtonGDB.TabIndex = 8;
            this.radioButtonGDB.Text = "空间数据库";
            this.radioButtonGDB.UseVisualStyleBackColor = true;
            // 
            // radioButtonEPS
            // 
            this.radioButtonEPS.AutoSize = true;
            this.radioButtonEPS.Checked = true;
            this.radioButtonEPS.Location = new System.Drawing.Point(13, 29);
            this.radioButtonEPS.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonEPS.Name = "radioButtonEPS";
            this.radioButtonEPS.Size = new System.Drawing.Size(77, 16);
            this.radioButtonEPS.TabIndex = 9;
            this.radioButtonEPS.TabStop = true;
            this.radioButtonEPS.Text = "EPS数据库";
            this.radioButtonEPS.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonGDB);
            this.groupBox1.Controls.Add(this.radioButtonEPS);
            this.groupBox1.Location = new System.Drawing.Point(501, 135);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(90, 78);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "合并格式";
            // 
            // EPSMergeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 360);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.buttonSelectOutPath);
            this.Controls.Add(this.buttonMerge);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.textBoxOutPath);
            this.Controls.Add(this.buttonAdd);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "EPSMergeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MDB合并";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.TextBox textBoxOutPath;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonMerge;
        private System.Windows.Forms.Button buttonSelectOutPath;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.RadioButton radioButtonGDB;
        private System.Windows.Forms.RadioButton radioButtonEPS;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}