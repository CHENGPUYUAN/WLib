
namespace WLib.Samples.WinForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage地图操作 = new System.Windows.Forms.TabPage();
            this.tabPage数据查询统计 = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel地图操作 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel数据查询统计 = new System.Windows.Forms.FlowLayoutPanel();
            this.mapViewer1 = new WLib.WinCtrls.ArcGisCtrl.MapViewer();
            this.tabControl1.SuspendLayout();
            this.tabPage地图操作.SuspendLayout();
            this.tabPage数据查询统计.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage地图操作);
            this.tabControl1.Controls.Add(this.tabPage数据查询统计);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1230, 130);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage地图操作
            // 
            this.tabPage地图操作.Controls.Add(this.flowLayoutPanel地图操作);
            this.tabPage地图操作.Location = new System.Drawing.Point(4, 25);
            this.tabPage地图操作.Name = "tabPage地图操作";
            this.tabPage地图操作.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage地图操作.Size = new System.Drawing.Size(957, 101);
            this.tabPage地图操作.TabIndex = 0;
            this.tabPage地图操作.Text = "地图操作";
            this.tabPage地图操作.UseVisualStyleBackColor = true;
            // 
            // tabPage数据查询统计
            // 
            this.tabPage数据查询统计.Controls.Add(this.flowLayoutPanel数据查询统计);
            this.tabPage数据查询统计.Location = new System.Drawing.Point(4, 25);
            this.tabPage数据查询统计.Name = "tabPage数据查询统计";
            this.tabPage数据查询统计.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage数据查询统计.Size = new System.Drawing.Size(1222, 101);
            this.tabPage数据查询统计.TabIndex = 1;
            this.tabPage数据查询统计.Text = "数据查询统计";
            this.tabPage数据查询统计.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel地图操作
            // 
            this.flowLayoutPanel地图操作.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel地图操作.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel地图操作.Name = "flowLayoutPanel地图操作";
            this.flowLayoutPanel地图操作.Size = new System.Drawing.Size(951, 95);
            this.flowLayoutPanel地图操作.TabIndex = 0;
            // 
            // flowLayoutPanel数据查询统计
            // 
            this.flowLayoutPanel数据查询统计.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel数据查询统计.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel数据查询统计.Name = "flowLayoutPanel数据查询统计";
            this.flowLayoutPanel数据查询统计.Size = new System.Drawing.Size(1216, 95);
            this.flowLayoutPanel数据查询统计.TabIndex = 0;
            // 
            // mapViewer1
            // 
            this.mapViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapViewer1.EagleMapCollapsed = false;
            this.mapViewer1.Location = new System.Drawing.Point(0, 130);
            this.mapViewer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mapViewer1.Name = "mapViewer1";
            this.mapViewer1.Size = new System.Drawing.Size(1230, 609);
            this.mapViewer1.TabIndex = 1;
            this.mapViewer1.TOCCollapsed = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1230, 739);
            this.Controls.Add(this.mapViewer1);
            this.Controls.Add(this.tabControl1);
            this.Name = "Main";
            this.Text = "Main";
            this.tabControl1.ResumeLayout(false);
            this.tabPage地图操作.ResumeLayout(false);
            this.tabPage数据查询统计.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage地图操作;
        private System.Windows.Forms.TabPage tabPage数据查询统计;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel地图操作;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel数据查询统计;
        private WinCtrls.ArcGisCtrl.MapViewer mapViewer1;
    }
}