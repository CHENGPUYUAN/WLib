using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WLib.ArcGis.GeoDatabase.FeatClass;
using WLib.ArcGis.GeoDatabase.WorkSpace;
using WLib.ArcGis.Geometry;
using WLib.WinCtrls.ArcGisCtrl;
using WLib.WinCtrls.PathCtrl;

namespace WLib.Samples.WinForm
{
    public partial class ClipForm : Form
    {

        SaveFormat outFormat;


        enum SaveFormat
        {
            MDB,
            MDB_EPS,
            DWG

        }
        public ClipForm(IMap pMap)
        {
            InitializeComponent();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Items.Add(SaveFormat.MDB);
            comboBox1.Items.Add(SaveFormat.DWG);
            comboBox1.Items.Add(SaveFormat.MDB_EPS);
            comboBox1.SelectedIndex = 0;
            progressBarTextLabel.Text = "";
            outFormat = SaveFormat.MDB;
            this.textBox_in.WatermarkText = "请输入需要裁剪的数据库路径（.mdb/.gdb）";
            this.textBox_clip.WatermarkText = "请选择裁剪范围对象（.dwg）";
            this.textBox_out.WatermarkText = "请选择保存路径";
            this.textBox_cad_lyr_name .WatermarkText = "请输入CAD图层名称";
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                outFormat = (SaveFormat)comboBox1.SelectedItem;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IWorkspace workspace_in = WorkspaceEx.GetWorkSpace(textBox_in.Text, EWorkspaceType.Default);

            if (workspace_in == null)
            {
                MessageBox.Show("请选择数据库！");
                return;
            }

            IEnumerable<IFeatureClass> features = workspace_in.GetFeatureClasses();
            int count = workspace_in.GetFeatureClassNames().Select(f => f).Count();
            progressBar1.Maximum = count;
            progressBar1.Value = 0;
            //IFeatureClass featureclass_clip = FeatureClassEx.FromPath(textBox_clip.Text);
            IFeatureClass featureclass_clip =  CADPolygonFeatureClass(textBox_clip.Text);
            if (featureclass_clip == null)
            {
                MessageBox.Show("请选择裁剪范围对象！");
                return;
            }
            this.button1.Enabled = false;
            this.textBox_in.Enabled = false;
            this.textBox_clip.Enabled = false;
            this.textBox_out.Enabled = false;



            string temp = System.IO.Path.Combine(System.IO.Path.GetTempPath().ToString(), "cpy");

            Directory.CreateDirectory(temp);
            int i = 0;

            IWorkspace out_ws = null;
            IWorkspace eps_ws = null;
            if (outFormat == SaveFormat.MDB)
            {
                out_ws = WorkspaceEx.GetOrCreateWorkspace(textBox_out.Text);
            }
            else if (outFormat == SaveFormat.MDB_EPS)
            {

                out_ws = WorkspaceEx.CreateInMemoryWorkspace("outdb");
                EPSHelper.CreateEPS(textBox_out.Text);
                eps_ws = WorkspaceEx.GetWorkSpace(textBox_out.Text);
            }

            foreach (IFeatureClass featureClass in features)
            {
                i++;
                //progressBarTextLabel.Text = "正在处理  " + i + "/" + count;
                if (featureClass.GetSourcePath() == featureclass_clip.GetSourcePath())
                {
                    continue;
                }
                try
                {
                    string name = featureClass.GetName();
                    if (outFormat == SaveFormat.DWG)
                    {
                        string outputfile = textBox_out.Text + "\\" + name + ".dwg";
                        string shp = System.IO.Path.Combine(temp, Guid.NewGuid().ToString().Replace("-", "") + ".shp");
                        Clip(featureClass,
                         featureclass_clip,
                          shp);
                        toCAD(shp, outputfile);
                    }
                    else if (outFormat == SaveFormat.MDB || outFormat == SaveFormat.MDB_EPS)
                    {
                        string outputfile = textBox_out.Text + "\\" + name;



                        IFeatureClass of = out_ws.GetFeatureClassByName(name, false);
                        if (of == null)
                        {
                            of = featureClass.CopyStruct(out_ws, name);
                        }
                        Clip(featureClass,
                           featureclass_clip,
                            of);
                        if (outFormat == SaveFormat.MDB_EPS)
                        {
                            EPSHelper.GDBToEPS(of, eps_ws, name);
                        }
                        Marshal.ReleaseComObject(of);
                        out_ws.DeleteFeatureClasses(name);
                    }
                    /**  else if (outFormat == SaveFormat.MDB_EPS)
                    {
                      string gdb = System.IO.Path.Combine(temp, "temp.gdb");
                        if (!Directory.Exists(gdb))
                        {
                            IWorkspace ws = WorkspaceEx.CreateWorkspace(EWorkspaceType.FileGDB, temp, "temp");
                            Marshal.ReleaseComObject(ws);
                        }
                        string outputfile = System.IO.Path.Combine(gdb, name);


                        IFeatureClass of = out_ws.GetFeatureClassByName(name, false);
                        if (of == null)
                        {
                            of = out_ws.CreateFeatureClass(featureClass);
                        }
                        Clip(featureClass,
                           featureclass_clip,
                            of);
                    }*/
                }

                catch (Exception ex)
                {
                    progressBarTextLabel.Text = "处理第" + i + "份数据时发生错误：" + ex.Message;
                    MessageBox.Show("处理第" + i + "份数据时发生错误：" + ex.Message);
                }
                Marshal.ReleaseComObject(featureClass);
                progressBar1.Value += 1;
            }
            //if (outFormat == SaveFormat.MDB_EPS)
            //{
            //    string dir = System.IO.Path.GetDirectoryName(textBox_out.Text);
            //    string filename = System.IO.Path.GetFileName(textBox_out.Text);
            //    string eps = System.IO.Path.Combine(dir, "eps" + filename);
            //    EPSHelper.GDBToEPS(textBox_out.Text, eps, progressBar1, true);
            //    File.Delete(textBox_out.Text);
            //    File.Move(eps, textBox_out.Text);
            //}
            DeleteDirectory(temp);
            Marshal.ReleaseComObject(featureclass_clip);
            Marshal.ReleaseComObject(workspace_in);
            Marshal.ReleaseComObject(out_ws);
            if (eps_ws != null)
            {
                Marshal.ReleaseComObject(eps_ws);
            }
            progressBar1.Value = progressBar1.Maximum;
            //progressBarTextLabel.Text = "处理完成";
            MessageBox.Show("处理完成");
            this.button1.Enabled = true;
            this.textBox_in.Enabled = true;
            this.textBox_clip.Enabled = true;
            this.textBox_out.Enabled = true;
        }

        public void Clip(IFeatureClass pInputFeatureClass, IFeatureClass pClipFeatureClass, IFeatureClass targetClass)
        {
            IGeometry clip = pClipFeatureClass.QueryFirstFeature().ShapeCopy;
            clip.Project(pInputFeatureClass.GetSpatialRef());
            ISpatialFilter spatialFilter = new SpatialFilterClass
            {
                Geometry = clip
            };
            IFeatureCursor featureCursor = pInputFeatureClass.Search(spatialFilter, true);
            IFeature feature = featureCursor.NextFeature();
            if (feature == null)
            {
                return;
            }
            //获取源要素类与目标要素类相同的字段的索引
            var dict = new Dictionary<int, int>();//key：字段在源要素类的索引；value：在目标要素类中的索引

            var tarFeatureCursor = targetClass.Insert(true);
            var tarFeatureBuffer = targetClass.CreateFeatureBuffer();
            var sourceFields = feature.Fields;
            int geometryIndex = 0;
            for (var i = 0; i < sourceFields.FieldCount; i++)
            {
                if (pInputFeatureClass.ShapeFieldName == sourceFields.get_Field(i).Name)
                {
                    geometryIndex = i;
                }
                var index1 = tarFeatureBuffer.Fields.FindField(sourceFields.get_Field(i).Name);
                if (index1 > -1 && tarFeatureBuffer.Fields.get_Field(index1).Editable)
                    dict.Add(i, index1);
            }


            esriGeometryDimension resultDimension = esriGeometryDimension.esriGeometry2Dimension;
            if (targetClass.ShapeType == esriGeometryType.esriGeometryPoint)
            {
                resultDimension = esriGeometryDimension.esriGeometry0Dimension;
            }
            else if (targetClass.ShapeType == esriGeometryType.esriGeometryPolyline)
            {
                resultDimension = esriGeometryDimension.esriGeometry1Dimension;
            }
            int count = 1;
            while (feature != null)
            {
                count++;
                try
                {
                    bool skip = false;
                    foreach (var pair in dict)
                    {
                        if (pair.Key == geometryIndex)
                        {
                            IGeometry outputGeometry = null;    //裁剪后的图形
                            ITopologicalOperator2 topo = clip as ITopologicalOperator2;
                            topo.IsKnownSimple_2 = true;
                            topo.Simplify();
                            outputGeometry = topo.Intersect(feature.ShapeCopy, resultDimension);
                            skip = outputGeometry.IsEmpty;
                            if (skip)
                            {
                                break;
                            }
                            tarFeatureBuffer.Shape = outputGeometry;
                        }
                        else
                            tarFeatureBuffer.set_Value(pair.Value, feature.get_Value(pair.Key));
                    }
                    if (!skip)
                    {
                        tarFeatureCursor.InsertFeature(tarFeatureBuffer);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                feature = featureCursor.NextFeature();
            }

            tarFeatureCursor.Flush();
            Marshal.ReleaseComObject(featureCursor);

            Marshal.ReleaseComObject(tarFeatureCursor);
        }
        public void Clip(IFeatureClass pInputFeatureClass, IFeatureClass pClipFeatureClass, string outFeatureClass)
        {
            ESRI.ArcGIS.Geoprocessor.Geoprocessor gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();

            try
            {
                gp.OverwriteOutput = true; //是否覆盖原文件
                ESRI.ArcGIS.AnalysisTools.Clip clipTool = new ESRI.ArcGIS.AnalysisTools.Clip(
                   pInputFeatureClass,
                   pClipFeatureClass,
                    outFeatureClass);
                gp.Execute(clipTool, null);

            }
            catch (Exception ex)
            {
                StringBuilder str = new StringBuilder();
                for (int i = 0; i < gp.MessageCount; i++)
                {
                    str.AppendLine(gp.GetMessage(i));
                }

                throw new Exception(str.ToString());
            }

        }

        public void toCAD(string input, string output)
        {
            Geoprocessor GP = new Geoprocessor();
            GP.OverwriteOutput = true;
            ////Types include DGN-V8, DWG-R14, DWG-R2000, DWG-R2004, DWG-R2005, DWG-R2006, DWG-R2007, DWG-R2010, DXF-R14, DXF-R2000, DXF-R2004, DXF-R2005, DXF-R2006, DXF-R2007, and DXF-R2010.
            string type = "DWG_R2010";

            ExportCAD tool = new ExportCAD(input, type, output);
            GP.Execute(tool, null);

        }

        private IFeatureClass CADPolygonFeatureClass(string path)
        {
            string directoryPath = System.IO.Path.GetDirectoryName(path);
            string fileName = System.IO.Path.GetFileName(path);
             IWorkspaceFactory pWorkspaceFactory = new CadWorkspaceFactoryClass();
            //--打开相应的工作空间，并赋值给要素空间，OpenFromFile（） 
            //--中的参数为CAD文件夹的路径 
            IFeatureWorkspace pFeatureWorkspace = pWorkspaceFactory.OpenFromFile(directoryPath, 0) as IFeatureWorkspace;
            /*--打开线要素类，如果要打开点类型的要素，需要把下边的代码该成： 
            *-- IFeatureClass pFeatureClass = pFeatureWorkspace.OpenFeatureClass (fileName + ":point"); 
            *-- 由此可见fileName为CAD图的名字，后边加上要打开的要素类的类型，中间用冒号　　
            *-- 隔开，大家可以想想多边形和标注是怎么打开的。 */
            //point          polyline
            IFeatureClass pFeatureClass = pFeatureWorkspace.OpenFeatureClass(fileName + ":"+textBox_cad_lyr_name.Text);
            Marshal.ReleaseComObject(pFeatureWorkspace);
            if (pFeatureClass == null)
            {
                MessageBox.Show("打开CAD要素类失败！");
                return null;
            }
            return pFeatureClass;
        }

        void DeleteDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                DirectoryInfo[] childs = dir.GetDirectories();
                foreach (DirectoryInfo child in childs)
                {
                    child.Delete(true);
                }
                dir.Delete(true);
            }
        }



        private void textBox_in_Click(object sender, EventArgs e)
        {
            //WorkspaceSelectorForm selectorForm = new WorkspaceSelectorForm("gdb|mdb");
            //selectorForm.StartPosition = FormStartPosition.CenterParent;
            //if (selectorForm.ShowDialog() == DialogResult.OK)
            //{
            //    this.textBox_in.Text = selectorForm.workspace;
            //}
        }


        private void textBox_clip_Click(object sender, EventArgs e)
        {
            //DataSelectorForm dataSelectorForm = new DataSelectorForm(EObjectFilter.FeatureLayer, false);
            //dataSelectorForm.StartPosition = FormStartPosition.CenterParent;
            //if (dataSelectorForm.ShowDialog() == DialogResult.OK)
            //{
            //    string path = dataSelectorForm.SelectWorkspacePath;
            //    string name = dataSelectorForm.SelectedObjectName;
            //    this.textBox_clip.Text = path + "\\" + name;
            //}
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "dwg|*.dwg";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox_clip.Text = openFileDialog.FileName;
            }
        }

        private void textBox_out_Click(object sender, EventArgs e)
        {
            if (outFormat == SaveFormat.DWG)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox_out.Text = dialog.SelectedPath;
                }
            }
            else if (outFormat == SaveFormat.MDB || outFormat == SaveFormat.MDB_EPS)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "mdb|*.mdb";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox_out.Text = dialog.FileName;
                }
            }
        }
    }
}
