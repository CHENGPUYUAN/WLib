using System;
using System.Windows.Forms;

namespace WLib.WinCtrls.ArcGisCtrl
{
    public partial class WorkspaceSelectorForm : Form
    {

        public string workspace => this.workspaceSelector1.PathOrConnStr;

        /// <summary>
        /// 获取工作空间
        /// </summary>
        public WorkspaceSelectorForm(string WorkspaceTypeFilter)
        {
            InitializeComponent();
            this.workspaceSelector1.WorkspaceTypeFilter = WorkspaceTypeFilter;
            this.workspaceSelector1.AfterSelectPath += workspaceSelector1_AfterSelectPath;
        }
        private void workspaceSelector1_AfterSelectPath(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
