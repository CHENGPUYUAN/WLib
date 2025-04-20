
namespace WLib.WinCtrls.ArcGisCtrl
{
    partial class WorkspaceSelectorForm
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
            this.workspaceSelector1 = new WLib.WinCtrls.ArcGisCtrl.WorkspaceSelector();
            this.SuspendLayout();
            // 
            // workspaceSelector1
            // 
            this.workspaceSelector1.Description = "工作空间：";
            this.workspaceSelector1.Location = new System.Drawing.Point(12, 12);
            this.workspaceSelector1.Name = "workspaceSelector1";
            this.workspaceSelector1.OptEnable = true;
            this.workspaceSelector1.PathOrConnStr = "粘贴路径于此并按下回车，或点击选择按钮";
            this.workspaceSelector1.Size = new System.Drawing.Size(686, 31);
            this.workspaceSelector1.TabIndex = 0;
            this.workspaceSelector1.WorkspaceIndex = 0;
            this.workspaceSelector1.WorkspaceTypeFilter = "shp|gdb|mdb|sde|excel|sql";
            // 
            // WorkspaceSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 53);
            this.Controls.Add(this.workspaceSelector1);
            this.Name = "WorkspaceSelectorForm";
            this.Text = "选择工作空间";
            this.ResumeLayout(false);

        }

        #endregion

        private WorkspaceSelector workspaceSelector1;
    }
}