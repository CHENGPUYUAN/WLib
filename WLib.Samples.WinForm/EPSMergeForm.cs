using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using WLib.ArcGis.GeoDatabase.FeatClass;
using WLib.ArcGis.GeoDatabase.Table;
using WLib.ArcGis.GeoDatabase.WorkSpace;

namespace WLib.Samples.WinForm
{
    public partial class EPSMergeForm : Form
    {
        public EPSMergeForm()
        {
            InitializeComponent();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = this.radioButtonEPS.Checked ? "EPS数据库文件（*.mdb）|*.mdb" : "个人文件地理数据库文件（*.mdb）|*.mdb"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    if (this.listBox1.Items.Contains(filePath))
                    {
                        continue;
                    }
                    this.listBox1.Items.Add(filePath);
                }

            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItems.Count > 0)
            {
                for (int i = listBox1.Items.Count - 1; i > -1; i--)
                {
                    if (listBox1.GetSelected(i))
                    {
                        listBox1.Items.RemoveAt(i);
                    }
                }
            }
        }

        private void buttonSelectOutPath_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Merge.mdb";
            saveFileDialog.Filter = this.radioButtonEPS.Checked ? "EPS数据库文件（*.mdb）|*.mdb" : "个人文件地理数据库文件（*.mdb）|*.mdb";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBoxOutPath.Text = saveFileDialog.FileName;
            }
        }

        private void buttonMerge_Click(object sender, EventArgs e)
        {

            if (this.listBox1.Items.Count == 0)
            {
                MessageBox.Show("请先选择要合并的EPS文件！", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(this.textBoxOutPath.Text))
            {
                MessageBox.Show("请先选择输出路径！");
                return;
            }
            buttonAdd.Enabled = false;
            buttonRemove.Enabled = false;
            buttonSelectOutPath.Enabled = false;
            buttonMerge.Enabled = false;

            try
            {
                if (this.radioButtonEPS.Checked)
                {
                    EPSHelper.CreateEPS(this.textBoxOutPath.Text);
                }
                else
                {
                    string dir = System.IO.Path.GetDirectoryName(this.textBoxOutPath.Text);
                    System.IO.Directory.CreateDirectory(dir);
                    EPSHelper.CreateGeoDB(dir, System.IO.Path.GetFileName(this.textBoxOutPath.Text));
                }
                ;
                IWorkspace workspace_result = WorkspaceEx.GetWorkSpace(this.textBoxOutPath.Text);
                this.progressBar1.InvokeIfRequired(() =>
               {
                   this.progressBar1.Value = 0;
                   this.progressBar1.Maximum = 100;
               });
                if (this.radioButtonEPS.Checked)
                {
                    MergeEPS(workspace_result);
                }
                else
                {
                    MergeMDB(workspace_result);
                }
                Marshal.ReleaseComObject(workspace_result);
                this.progressBar1.InvokeIfRequired(() =>
               {
                   this.progressBar1.Value = this.progressBar1.Maximum;
               });
                buttonAdd.Enabled = true;
                buttonRemove.Enabled = true;
                buttonSelectOutPath.Enabled = true;
                buttonMerge.Enabled = true;
                MessageBox.Show("合并完成！", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MergeEPS(IWorkspace workspace_EPS)
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                string item = (string)this.listBox1.Items[i];
                IWorkspace workspace_MDB = WorkspaceEx.GetWorkSpace(item);
                IEnumerable<string> tables = workspace_MDB.GetTableNames();
                int tableCount = tables.Count();
                int tableIndex = 0;
                foreach (string table in tables)
                {
                    tableIndex++;
                    if (workspace_EPS.IsTableExsit(table))
                    {
                        ITable table_EPS = workspace_EPS.GetITableByName(table);
                        ITable table_MDB = workspace_MDB.GetITableByName(table);
                        table_MDB.CopyDataToTable(table_EPS);
                        Marshal.ReleaseComObject(table_MDB);
                        Marshal.ReleaseComObject(table_EPS);
                    }
                    this.progressBar1.InvokeIfRequired(() =>
                        {
                            this.progressBar1.Value = (i * this.progressBar1.Maximum / this.listBox1.Items.Count)
                            + tableIndex * this.progressBar1.Maximum / (tableCount * this.listBox1.Items.Count);
                        });
                }
                Marshal.ReleaseComObject(workspace_MDB);
                this.progressBar1.InvokeIfRequired(() =>
                {
                    this.progressBar1.Value = (i + 1) * this.progressBar1.Maximum / this.listBox1.Items.Count;
                });
            }

        }

        private void MergeMDB(IWorkspace workspace_Result)
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                string item = (string)this.listBox1.Items[i];
                IWorkspace workspace_MDB = WorkspaceEx.GetWorkSpace(item);
                IEnumerable<string> features = workspace_MDB.GetFeatureClassNames();
                int tableCount = features.Count();
                int tableIndex = 0;
                foreach (string name in features)
                {
                    tableIndex++;
                    IFeatureClass table_MDB = workspace_MDB.GetFeatureClassByName(name);
                    if (workspace_Result.IsTableExsit(name))
                    {
                        IFeatureClass table_res = workspace_Result.GetFeatureClassByName(name);
                        table_MDB.CopyTo(table_res);
                        Marshal.ReleaseComObject(table_MDB);
                        Marshal.ReleaseComObject(table_res);
                    }
                    else
                    {
                        table_MDB.CopyTo(workspace_Result);
                    }
                    this.progressBar1.InvokeIfRequired(() =>
                        {
                            this.progressBar1.Value = (i * this.progressBar1.Maximum / this.listBox1.Items.Count)
                            + tableIndex * this.progressBar1.Maximum / (tableCount * this.listBox1.Items.Count);
                        });
                }
                Marshal.ReleaseComObject(workspace_MDB);
                this.progressBar1.InvokeIfRequired(() =>
                {
                    this.progressBar1.Value = (i + 1) * this.progressBar1.Maximum / this.listBox1.Items.Count;
                });
            }
        }
    }
}
