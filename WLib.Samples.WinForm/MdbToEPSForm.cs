using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using WLib.ArcGis.GeoDatabase.FeatClass;
using WLib.ArcGis.GeoDatabase.FeatDataset;
using WLib.ArcGis.GeoDatabase.WorkSpace;
using WLib.Database;

namespace WLib.Samples.WinForm
{
    public partial class MdbToEPSForm : Form
    {
        public MdbToEPSForm()
        {
            InitializeComponent();
            //this.textBox1.Text = @"G:\data\111\MyProject2.gdb";
            //this.textBox2.Text = @"G:\data\111\eps.mdb";

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string mdb = this.textBox1.Text.Trim();
            if (mdb.Length == 0)
            {
                MessageBox.Show("请输入ArcGIS MDB路径");
                if (!WorkspaceEx.IsWorkspacePath(mdb))
                {
                    MessageBox.Show("请输入有效的ArcGIS MDB路径");
                    return;
                }
            }
            string eps = this.textBox2.Text.Trim();
            if (eps.Length == 0)
            {
                MessageBox.Show("请输入EPS MDB保存路径");
                return;
            }
            button3.Enabled = false;
            EPSHelper.GDBToEPS( mdb, eps, this.progressBar1);
            button3.Enabled = true;
            MessageBox.Show("完成");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = dialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = dialog.FileName;
            }
        }


    }
}
