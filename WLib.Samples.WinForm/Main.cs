using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using WLib.ArcGis.Control.MapAssociation;

namespace WLib.Samples.WinForm
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            initTabPage();
            string mxd = AppDomain.CurrentDomain.BaseDirectory + @"Data\SampleData.mxd";
            if (System.IO.File.Exists(mxd)) { }
            try
            {
                this.mapViewer1.MainMapControl.LoadMxFile(AppDomain.CurrentDomain.BaseDirectory + @"Data\SampleData.mxd");
                this.mapViewer1.MainMapControl.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.mapViewer1.MapNavigationTools.CurrentTool = EMapTools.Pan;
        }
        private void initTabPage()
        {
            Func<string, Button> creatrBtn = (name) =>
                new Button
                {
                    Text = name,
                    Width = 102,
                    Height = 52
                };

            Button BtnZoomIn = creatrBtn("放大");
            BtnZoomIn.Click += buttonZoomIn_Click;
            this.flowLayoutPanel地图操作.Controls.Add(BtnZoomIn);

            Button BtnZoomOut = creatrBtn("缩小");
            BtnZoomOut.Click += buttonZoomOut_Click;
            this.flowLayoutPanel地图操作.Controls.Add(BtnZoomOut);

            Button BtnPan = creatrBtn("漫游");
            BtnPan.Click += buttonPan_Click;
            this.flowLayoutPanel地图操作.Controls.Add(BtnPan);

            Button BtnIdentify = creatrBtn("识别");
            BtnIdentify.Click += buttonIdentify_Click;
            this.flowLayoutPanel地图操作.Controls.Add(BtnIdentify);

            Button BtnAddDWG = creatrBtn("添加DWG文件");
            BtnAddDWG.Click += button_addDWG_Click;
            this.flowLayoutPanel数据查询统计.Controls.Add(BtnAddDWG);

            Button BtnEPStoMdb = creatrBtn("EPS数据库转个人文件地理数据库");
            BtnEPStoMdb.Click += buttonEPStoMdb_Click;
            this.flowLayoutPanel数据查询统计.Controls.Add(BtnEPStoMdb);

            Button BtnMdbToEPS = creatrBtn("个人文件地理数据库转EPS数据库");
            BtnMdbToEPS.Click += buttonMdbToEPS_Click;
            this.flowLayoutPanel数据查询统计.Controls.Add(BtnMdbToEPS);

            Button BtnMergeEPS = creatrBtn("数据库合并");
            BtnMergeEPS.Click += buttonMergeEPS_Click;
            this.flowLayoutPanel数据查询统计.Controls.Add(BtnMergeEPS);

            Button BtnClipMDB = creatrBtn("个人文件地理数据库数据库裁剪");
            BtnClipMDB.Click += buttonClipMDB_Click;
            this.flowLayoutPanel数据查询统计.Controls.Add(BtnClipMDB);

        }

        private void buttonClipMDB_Click(object sender, EventArgs e)
        {
            ClipForm clipForm = new ClipForm(this.mapViewer1.MainMapControl.Map);
            clipForm.StartPosition = FormStartPosition.CenterParent;
            clipForm.Show(this);
        }

        private void buttonIdentify_Click(object sender, EventArgs e)
        {
            this.mapViewer1.MapNavigationTools.CurrentTool = EMapTools.Identify;
        }

        private void buttonZoomOut_Click(object sender, EventArgs e)
        {
            this.mapViewer1.MapNavigationTools.CurrentTool = EMapTools.ZoomOut;
        }

        private void buttonZoomIn_Click(object sender, EventArgs e)
        {
            this.mapViewer1.MapNavigationTools.CurrentTool = EMapTools.ZoomIn;
        }

        private void buttonPan_Click(object sender, EventArgs e)
        {
            this.mapViewer1.MapNavigationTools.CurrentTool = EMapTools.Pan;
        }
        private void buttonMergeEPS_Click(object sender, EventArgs e)
        {
            EPSMergeForm form = new EPSMergeForm();
            form.ShowDialog(this);
        }

        private void buttonMdbToEPS_Click(object sender, EventArgs e)
        {
            MdbToEPSForm form = new MdbToEPSForm();
            form.ShowDialog(this);
        }

        private void buttonEPStoMdb_Click(object sender, EventArgs e)
        {
            EPSToGDBForm form = new EPSToGDBForm();
            form.ShowDialog(this);
        }

        private void button_addDWG_Click(object sender, EventArgs e)
        {
            IWorkspaceFactory pWorkspaceFactory = new CadWorkspaceFactoryClass();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DWG文件(*.dwg)|*.dwg|所有文件(*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            FileInfo fileOpen = new FileInfo(openFileDialog.FileName);
            IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(fileOpen.DirectoryName, 0);
            //打开一个要素集
            IFeatureDataset pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset(fileOpen.Name);
            IFeatureClassContainer pFeatureClassContainer = (IFeatureClassContainer)pFeatureDataset;
            IGroupLayer pGroupLayer = new GroupLayerClass();
            pGroupLayer.Name = pFeatureDataset.Name;
            //遍历CAD文件中的每个要素
            for (int i = 0; i < pFeatureClassContainer.ClassCount; i++)
            {
                IFeatureClass pFeatureClass = pFeatureClassContainer.get_Class(i);
                //加载注记图层【esriFTCoverageAnnotation】
                if (pFeatureClass.FeatureType == esriFeatureType.esriFTCoverageAnnotation)
                {
                    IFeatureLayer pFeatureLayer = new CadAnnotationLayerClass();
                    pFeatureLayer.Name = pFeatureClass.AliasName;
                    pFeatureLayer.FeatureClass = pFeatureClass;
                    pFeatureLayer.DataSourceType = "CAD Annotation Feature Class";//设置后Annotation的默认符号化方式是注记而不是点
                    pGroupLayer.Add(pFeatureLayer);
                }
                //加载点线面图层
                else
                {
                    IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                    pFeatureLayer.Name = pFeatureClass.AliasName;
                    pFeatureLayer.FeatureClass = pFeatureClass;
                    pGroupLayer.Add(pFeatureLayer);
                }

            }
            this.mapViewer1.MainMapControl.AddLayer(pGroupLayer);
            this.mapViewer1.MainMapControl.Refresh();
        }
    }
}
