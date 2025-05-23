﻿namespace WLib.Samples.WinForm
{
    partial class ClipForm
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBarTextLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_in = new WatermarkedTextBox.WatermarkedTextBox();
            this.textBox_clip = new WatermarkedTextBox.WatermarkedTextBox();
            this.textBox_out = new WatermarkedTextBox.WatermarkedTextBox();
            this.button_select_clip = new System.Windows.Forms.Button();
            this.button_select_out = new System.Windows.Forms.Button();
            this.textBox_cad_lyr_name = new WatermarkedTextBox.WatermarkedTextBox();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 273);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(567, 18);
            this.progressBar1.TabIndex = 5;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(96, 186);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(408, 20);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 153);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "输出目录";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 186);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "输出格式";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(448, 224);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 27);
            this.button1.TabIndex = 9;
            this.button1.Text = "开始";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBarTextLabel
            // 
            this.progressBarTextLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.progressBarTextLabel.AutoSize = true;
            this.progressBarTextLabel.Location = new System.Drawing.Point(249, 278);
            this.progressBarTextLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.progressBarTextLabel.Name = "progressBarTextLabel";
            this.progressBarTextLabel.Size = new System.Drawing.Size(95, 12);
            this.progressBarTextLabel.TabIndex = 10;
            this.progressBarTextLabel.Text = "progressBarText";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 88);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "裁剪范围";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "待裁剪数据";
            // 
            // textBox_in
            // 
            this.textBox_in.Lines = new string[0];
            this.textBox_in.Location = new System.Drawing.Point(96, 52);
            this.textBox_in.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_in.Name = "textBox_in";
            this.textBox_in.Size = new System.Drawing.Size(407, 21);
            this.textBox_in.TabIndex = 19;
            // 
            // textBox_clip
            // 
            this.textBox_clip.Lines = new string[0];
            this.textBox_clip.Location = new System.Drawing.Point(96, 88);
            this.textBox_clip.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_clip.Name = "textBox_clip";
            this.textBox_clip.Size = new System.Drawing.Size(370, 21);
            this.textBox_clip.TabIndex = 20;
            this.textBox_clip.Click += new System.EventHandler(this.textBox_clip_Click);
            // 
            // textBox_out
            // 
            this.textBox_out.Lines = new string[0];
            this.textBox_out.Location = new System.Drawing.Point(96, 153);
            this.textBox_out.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_out.Name = "textBox_out";
            this.textBox_out.Size = new System.Drawing.Size(370, 21);
            this.textBox_out.TabIndex = 21;
            this.textBox_out.Click += new System.EventHandler(this.textBox_out_Click);
            // 
            // button_select_clip
            // 
            this.button_select_clip.Location = new System.Drawing.Point(470, 91);
            this.button_select_clip.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_select_clip.Name = "button_select_clip";
            this.button_select_clip.Size = new System.Drawing.Size(33, 18);
            this.button_select_clip.TabIndex = 22;
            this.button_select_clip.Text = "...";
            this.button_select_clip.UseVisualStyleBackColor = true;
            this.button_select_clip.Click += new System.EventHandler(this.textBox_clip_Click);
            // 
            // button_select_out
            // 
            this.button_select_out.Location = new System.Drawing.Point(470, 153);
            this.button_select_out.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_select_out.Name = "button_select_out";
            this.button_select_out.Size = new System.Drawing.Size(33, 18);
            this.button_select_out.TabIndex = 23;
            this.button_select_out.Text = "...";
            this.button_select_out.UseVisualStyleBackColor = true;
            this.button_select_out.Click += new System.EventHandler(this.textBox_out_Click);
            // 
            // textBox_cad_lyr_name
            // 
            this.textBox_cad_lyr_name.Lines = new string[0];
            this.textBox_cad_lyr_name.Location = new System.Drawing.Point(96, 113);
            this.textBox_cad_lyr_name.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_cad_lyr_name.Name = "textBox_cad_lyr_name";
            this.textBox_cad_lyr_name.Size = new System.Drawing.Size(407, 21);
            this.textBox_cad_lyr_name.TabIndex = 24;
            // 
            // ClipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 291);
            this.Controls.Add(this.textBox_cad_lyr_name);
            this.Controls.Add(this.button_select_out);
            this.Controls.Add(this.button_select_clip);
            this.Controls.Add(this.textBox_out);
            this.Controls.Add(this.textBox_clip);
            this.Controls.Add(this.textBox_in);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressBarTextLabel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ClipForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Clip";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label progressBarTextLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private WatermarkedTextBox.WatermarkedTextBox textBox_in;
        private WatermarkedTextBox.WatermarkedTextBox textBox_clip;
        private WatermarkedTextBox.WatermarkedTextBox textBox_out;
        private System.Windows.Forms.Button button_select_clip;
        private System.Windows.Forms.Button button_select_out;
        private WatermarkedTextBox.WatermarkedTextBox textBox_cad_lyr_name;
    }
}