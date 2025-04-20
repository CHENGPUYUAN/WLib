using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WLib.ArcGis.Analysis.Gp;
using WLib.ArcGis.Data;
using WLib.ArcGis.GeoDatabase.FeatClass;
using WLib.ArcGis.GeoDatabase.Fields;
using WLib.ArcGis.GeoDatabase.Table;
using WLib.ArcGis.GeoDatabase.WorkSpace;
using WLib.ArcGis.Geometry;
using WLib.Database.TableInfo;
using WLib.Samples.WinForm.Properties;

namespace WLib.Samples.WinForm
{
    class EPSHelper
    {
        #region GDB转EPS

        public static void GDBToEPS(string gdb_path, string eps_path, ProgressBar progressBar1, bool CreataNewIfNotExist = true)
        {

            if (CreataNewIfNotExist && !File.Exists(eps_path))
            {
                CreateEPS(eps_path);
            }
            //获取所有图层
            IWorkspace workspace_MDB = WorkspaceEx.GetWorkSpace(gdb_path);
            if (workspace_MDB == null)
            {
                MessageBox.Show($"无法打开{gdb_path}！");
                return;
            }
            IEnumerable<IFeatureClass> featureClasses = workspace_MDB.GetFeatureClasses();

           progressBar1?.InvokeIfRequired(() =>
           {
               progressBar1.Maximum = workspace_MDB.GetFeatureClassNames().Select(x => x).Count();
               progressBar1.Value = 0;
           });

            IWorkspace workspace_EPS = WorkspaceEx.GetWorkSpace(eps_path);

            foreach (IFeatureClass featureClass in featureClasses)
            {
                string name = featureClass.GetName();

                //this.progressBarTextLabel.Text = $"开始处理{name}";
                ITable table = workspace_EPS.GetITableByName(name);
                if (table != null)
                {
                    EPSHelper.toEPS(featureClass, table);
                    Marshal.ReleaseComObject(table);
                }
                else
                {
                    //progressBarTextLabel.Text = $"跳过{name}";
                }
                Marshal.ReleaseComObject(featureClass);
                progressBar1?.InvokeIfRequired(() =>
                {
                    progressBar1.Value += 1;
                });
            }
            Marshal.ReleaseComObject(workspace_MDB);
            Marshal.ReleaseComObject(workspace_EPS);
        }
        public static void CreateEPS(string path)
        {
            if (File.Exists(path))
            {
                return;
            }
            string dir = System.IO.Path.GetDirectoryName(path);
            Directory.CreateDirectory(dir);
            File.WriteAllBytes(path, Properties.Resources.MDB格式模板);
        }
        public static void toEPS(IFeatureClass featureClass, ITable table)
        {
            if (table == null)
            {
                return;
            }
            string table_name = table.GetName();
            if (table_name.EndsWith("Point"))
            {
                //点表
                insertToPointTable(featureClass, table);
            }
            else if (table_name.EndsWith("Line"))
            {
                //线表
                insertToLineTable(featureClass, table);
            }
            else if (table_name.EndsWith("Annexe"))
            {
                //面表
                insertToAnnexeTable(featureClass, table);
            }
            else if (table_name.EndsWith("Text"))
            {
                //点标注表
                insertToPointTable(featureClass, table);
            }
            else if (table_name.EndsWith("Mark"))
            {
                //线标注表
                insertToPointTable(featureClass, table);
            }
            else
            {
                return;
            }
        }
        private static void insertToPointTable(IFeatureClass featureClass, ITable table)
        {
            ICursor cursor = table.Insert(true);
            //根据table的字段，将featureClass中的数据插入到table中，没有的字段不插入
            List<string> tn = table.GetFieldsNames().Select(i => i.ToUpper()).ToList();
            List<string> fn = featureClass.GetFieldsNames();
            int size = tn.Count;
            Dictionary<int, int> map = new Dictionary<int, int>();
            for (int i = 0; i < size; i++)
            {
                if (fn.Contains(tn[i]))
                {
                    map.Add(fn.IndexOf(tn[i]), i);
                }
            }
            Func<IFeature, IRowBuffer> toRow = (fea) =>
            {
                IRowBuffer row = table.CreateRowBuffer();
                foreach (var pair in map)
                {
                    row.set_Value(pair.Value, fea.get_Value(pair.Key));
                }
                return row;
            };
            foreach (IFeature item in featureClass.QueryFeatures())
            {
                cursor.InsertRow(toRow(item));
            }
            cursor.Flush(); Marshal.ReleaseComObject(cursor);
        }
        private static void insertToLineTable(IFeatureClass featureClass, ITable table)
        {
            ICursor cursor = table.Insert(true);
            //根据table的字段，将featureClass中的数据插入到table中，没有的字段不插入
            List<string> tn = table.GetFieldsNames().Select(i => i.ToUpper()).ToList();
            List<string> fn = featureClass.GetFieldsNames();
            int size = tn.Count;
            Dictionary<int, int> map = new Dictionary<int, int>();
            for (int i = 0; i < size; i++)
            {
                if (fn.Contains(tn[i]))
                {
                    map.Add(fn.IndexOf(tn[i]), i);
                }
            }
            int idIndex = table.GetFieldIndex("ID");

            Func<IFeature, IRowBuffer> toRow = (fea) =>
            {
                IRowBuffer row = table.CreateRowBuffer();
                foreach (var pair in map)
                {
                    row.set_Value(pair.Value, fea.get_Value(pair.Key));
                }
                //row.set_Value(idIndex, fea.OID);
                return row;
            };
            foreach (IFeature item in featureClass.QueryFeatures())
            {
                cursor.InsertRow(toRow(item));
            }
            cursor.Flush(); Marshal.ReleaseComObject(cursor);
        }
        private static void insertToAnnexeTable(IFeatureClass featureClass, ITable table)
        {
            ICursor cursor = table.Insert(true);
            //根据table的字段，将featureClass中的数据插入到table中，没有的字段不插入
            List<string> tn = table.GetFieldsNames().Select(i => i.ToUpper()).ToList();
            List<string> fn = featureClass.GetFieldsNames();
            int size = tn.Count;
            Dictionary<int, int> map = new Dictionary<int, int>();
            for (int i = 1; i < size; i++)//跳过ID
            {
                if (fn.Contains(tn[i]))
                {
                    map.Add(fn.IndexOf(tn[i]), i);
                }
            }
            int PointTypeIndex = tn.IndexOf("PointType".ToUpper());
            int arrXIndex = tn.IndexOf("arrX".ToUpper());
            int arrYIndex = tn.IndexOf("arrY".ToUpper());
            int arrZIndex = tn.IndexOf("arrZ".ToUpper());
            Func<IFeature, string[]> createArray = (feature) =>
            {
                IPoint[] pts = feature.ShapeCopy.GetPointArray();
                string arrX = string.Join("#", pts.Select(p => p.X.ToString()));
                string arrY = string.Join("#", pts.Select(p => p.Y.ToString()));
                string arrZ = string.Join("#", pts.Select(p => p.Z == double.NaN ? "0" : p.Z.ToString()));
                string PointType = string.Join("#", pts.Select(p => "0"));

                return new string[] { arrX, arrY, arrZ, PointType };
            };
            Func<IFeature, IRowBuffer> toRow = (fea) =>
            {
                IRowBuffer row = table.CreateRowBuffer();
                foreach (var pair in map)
                {
                    row.set_Value(pair.Value, fea.get_Value(pair.Key));
                }
                string[] arr = createArray(fea);

                if (arrXIndex >= 0 && arrYIndex >= 0 && arrZIndex >= 0)
                {
                    row.set_Value(arrXIndex, arr[0]);
                    row.set_Value(arrYIndex, arr[1]);
                    row.set_Value(arrZIndex, arr[2]);
                }
                if (PointTypeIndex >= 0)
                {
                    row.set_Value(PointTypeIndex, arr[3]);
                }
                return row;
            };
            foreach (IFeature item in featureClass.QueryFeatures())
            {
                cursor.InsertRow(toRow(item));
            }
            cursor.Flush(); Marshal.ReleaseComObject(cursor);
        }
        #endregion GDB转EPS


        #region EPS转GDB
        public static void EPSToGDB(string eps_path, string mdb_path, ProgressBar progressBar1, bool CreataNewIfNotExist = true)
        {
            if (!File.Exists(eps_path))
            {
                MessageBox.Show($"{eps_path}文件不存在！");
                return;
            }
            if (CreataNewIfNotExist && !File.Exists(mdb_path))
            {
                string dir = System.IO.Path.GetDirectoryName(mdb_path);
                Directory.CreateDirectory(dir);
                CreateGeoDB(dir, System.IO.Path.GetFileName(mdb_path));
                //File.WriteAllBytes(mdb_path, Properties.Resources.template_mdb);
            }


            IWorkspace workspace_MDB = WorkspaceEx.GetWorkSpace(mdb_path);

            IWorkspace workspace_EPS = WorkspaceEx.GetWorkSpace(eps_path);
            IEnumerable<ITable> tables = workspace_EPS.GetTables();

            progressBar1.InvokeIfRequired(() =>
            {
                progressBar1.Maximum = workspace_EPS.GetTableNames().Select(x => x).Count();
                progressBar1.Value = 0;
            });

            foreach (ITable t in tables)
            {
                string name = (t as IDataset)?.Name;

                //this.progressBarTextLabel.Text = $"开始处理{name}";
                IFeatureClass fc = workspace_MDB.GetFeatureClassByName(name, false, false);
                if (fc == null)
                {
                    fc = EPSHelper.createEPSFeatureClass(workspace_MDB, name);
                }
                if (fc != null)
                {
                    EPSHelper.fromEPS(t, fc);
                    Marshal.ReleaseComObject(fc);
                }
                Marshal.ReleaseComObject(t);
                progressBar1.InvokeIfRequired(() =>
                {
                    progressBar1.Value += 1;
                });
            }
            //删除template
            workspace_MDB.DeleteFeatureClassesByKeyWord("template");
            Marshal.ReleaseComObject(workspace_MDB);
            Marshal.ReleaseComObject(workspace_EPS);
        }
        public static void CreateGeoDB(string path, string dbName)
        {

            try
            {
                IWorkspace ws = WorkspaceEx.CreateWorkspace(path, dbName);
                if (ws == null)
                {
                    MessageBox.Show($"创建数据库失败！");
                    return;
                }
                Marshal.ReleaseComObject(ws);
                /**string xmlPath = System.IO.Path.Combine(path, "schema.xml");
                GpHelper.CheckClassesValidate(xmlPath);
                File.WriteAllText(System.IO.Path.Combine(path, "schema.xml"), Resources.WorkspaceDocument);

                // 设置工具参数
                object missing = Type.Missing;
                var tool = new ImportXMLWorkspaceDocument();
                tool.in_file = xmlPath;               // XML 文件路径
                tool.target_geodatabase = System.IO.Path.Combine(path, dbName);         // 目标地理数据库路径
                tool.import_type = "SCHEMA_ONLY";
                string msg;
                GpHelper.RunTool(tool, out msg);*/
            }
            catch (Exception ex)
            {
                MessageBox.Show($"错误: {ex.Message}");
            }

        }
        public static void fromEPS(ITable table, IFeatureClass featureClass)
        {
            if (table == null)
            {
                return;
            }
            string table_name = table.GetName();
            IFeatureCursor cursor = featureClass.Insert(true);
            List<string> tn = table.GetFieldsNames().Select(i => i.ToUpper()).ToList();
            List<string> fn = featureClass.GetFieldsNames();
            int size = fn.Count;
            Dictionary<int, int> map = new Dictionary<int, int>();
            for (int i = 0; i < size; i++)
            {
                if (tn.Contains(fn[i]))
                {
                    map.Add(tn.IndexOf(fn[i]), i);
                }
            }
            int xIndex = tn.IndexOf("X".ToUpper());
            int yIndex = tn.IndexOf("Y".ToUpper());

            int S_XIndex = tn.IndexOf("S_X".ToUpper());
            int S_YIndex = tn.IndexOf("S_Y".ToUpper());
            int E_XIndex = tn.IndexOf("E_X".ToUpper());
            int E_YIndex = tn.IndexOf("E_Y".ToUpper());


            int arrXIndex = tn.IndexOf("arrX".ToUpper());
            int arrYIndex = tn.IndexOf("arrY".ToUpper());
            int arrZIndex = tn.IndexOf("arrZ".ToUpper());
            Func<IRow, IFeatureBuffer> toAnnexeFeature = (row) =>
            {
                IFeatureBuffer fea = featureClass.CreateFeatureBuffer();
                foreach (var pair in map)
                {
                    fea.set_Value(pair.Value, row.get_Value(pair.Key));
                }

                if (arrXIndex >= 0 && arrYIndex >= 0 && arrZIndex >= 0)
                {
                    double[] arrX = row.get_Value(arrXIndex).ToString().Split('#').Select(x => double.Parse(x)).ToArray();
                    double[] arrY = row.get_Value(arrYIndex).ToString().Split('#').Select(y => double.Parse(y)).ToArray();
                    double[] arrZ = row.get_Value(arrZIndex).ToString().Split('#').Select(z => double.Parse(z)).ToArray();
                    IPoint[] pts = new IPoint[arrX.Length];
                    for (int i = 0; i < arrX.Length; i++)
                    {
                        IPoint pt = new PointClass
                        {
                            X = arrX[i],
                            Y = arrY[i],
                            //Z = arrZ[i]
                        };
                        pts[i] = pt;
                    }
                    IPolyline line = GeometryOpt.CreatePolyline(pts);
                    /**IZAware pZAware = (IZAware)line;
                     pZAware.ZAware = true;
                     IZ iz1 = (IZ)line;
                     iz1.SetConstantZ(0);*/
                    fea.Shape = line;
                }
                return fea;
            };
            Func<IRow, IFeatureBuffer> toLineFeature = (row) =>
                        {
                            IFeatureBuffer fea = featureClass.CreateFeatureBuffer();
                            foreach (var pair in map)
                            {
                                fea.set_Value(pair.Value, row.get_Value(pair.Key));
                            }

                            if (S_XIndex >= 0 && S_YIndex >= 0 && E_XIndex >= 0 && E_YIndex >= 0)
                            {

                                IPoint[] pts = new IPoint[2];
                                IPoint pt = new PointClass
                                {
                                    X = (double)row.get_Value(S_XIndex),
                                    Y = (double)row.get_Value(S_YIndex),
                                    //Z = 0
                                };
                                pts[0] = pt;
                                IPoint ept = new PointClass
                                {
                                    X = (double)row.get_Value(E_XIndex),
                                    Y = (double)row.get_Value(E_YIndex),
                                    //Z = 0
                                };
                                pts[1] = ept;

                                IPolyline line = GeometryOpt.CreatePolyline(pts);
                                /**IZAware pZAware = (IZAware)line;
                                pZAware.ZAware = true;
                                IZ iz1 = (IZ)line;
                                iz1.SetConstantZ(0);*/
                                fea.Shape = line;
                            }
                            return fea;
                        };
            Func<IRow, IFeatureBuffer> toPointFeature = (row) =>
                                    {
                                        IFeatureBuffer fea = featureClass.CreateFeatureBuffer();
                                        foreach (var pair in map)
                                        {
                                            fea.set_Value(pair.Value, row.get_Value(pair.Key));
                                        }
                                        if (xIndex >= 0 && yIndex >= 0)
                                        {
                                            IPoint pt = new PointClass
                                            {
                                                X = (double)row.get_Value(xIndex),
                                                Y = (double)row.get_Value(yIndex),
                                                //Z = 0
                                            };
                                            /**IZAware pZAware = (IZAware)pt;
                                            pZAware.ZAware = true;
                                            IZ iz1 = (IZ)pt;
                                            iz1.SetConstantZ(0);*/
                                            fea.Shape = pt;
                                        }
                                        return fea;
                                    };
            Func<IRow, IFeatureBuffer> func;
            if (table_name.EndsWith("Annexe"))
            {
                func = toAnnexeFeature;
            }
            else if (table_name.EndsWith("Line"))
            {
                func = toLineFeature;
            }
            else if (table_name.EndsWith("Point"))
            {
                func = toPointFeature;
            }
            else
            {
                func = toPointFeature;
            }
            foreach (IRow item in table.QueryRows())
            {
                cursor.InsertFeature(func(item));
            }
            cursor.Flush();
            Marshal.ReleaseComObject(cursor);

        }

        public static IFeatureClass createEPSFeatureClass(IWorkspace workspace, string name)
        {
            IFields fields;
            if (name.EndsWith("Annexe"))
            {
                List<IField> otherFields = new List<IField>
                {
                    FieldEx.CreateField("ID", "表的行序号", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("SEQID", "行序号", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("CODE", "附属物代码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("EXP_NO", "管线点编号", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("X1", "第一点x", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("Y1", "第一点y", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("Z1", "第一点z", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("X2", "第二点x", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("Y2", "第二点y", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("Z2", "第二点z", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("X3", "第三点x", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("Y3", "第三点y", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("Z3", "第三点z", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("X4", "第四点x", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("Y4", "第四点y", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("Z4", "第四点z", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("EHANDLER", "图元标识码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("ISLOCK", "ISLOCK", esriFieldType.esriFieldTypeString)
                };
                fields = FieldEx.CreateBaseFields(esriGeometryType.esriGeometryPolyline, CreateSpatialReference(), true, false);
                FieldEx.AddFields(fields, otherFields);
            }
            else if (name.EndsWith("Line"))
            {
                List<IField> otherFields = new List<IField>
                {
                    FieldEx.CreateField("PRJ_NO", "工程编号", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("S_EXP", "起点管线点号", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("S_X", "起点X坐标", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("S_Y", "起点Y坐标", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("S_DEEP", "起点管线埋深", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("E_EXP", "下一点管线点号", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("E_X", "下一点X坐标", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("E_Y", "下一点Y坐标", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("E_DEEP", "下一点管线埋深", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("TYPE", "管线种类", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("CODE", "管线代码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("MATERIAL", "材质", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("MAT_CEN", "线芯材质", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("INTERFACE", "接口方式", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("PSIZE", "管径", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("VOLTAGE", "电压值", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("PRESSURE", "压力", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("TEMPERATURE", "温度", esriFieldType.esriFieldTypeInteger),
                    FieldEx.CreateField("CABNUM", "电缆条数", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("TOTALHOLE", "总孔数", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("USEDHOLE", "已用孔数", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("FLOWDIR", "排水流向", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("ADDRESS", "管线段地址", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("EMBED", "埋设方式", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("MDATE", "埋设日期", esriFieldType.esriFieldTypeDate),
                    FieldEx.CreateField("BELONG", "权属单位代码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("SUNIT", "探测单位代码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("SDATE", "探测日期", esriFieldType.esriFieldTypeDate),
                    FieldEx.CreateField("NOTE", "备注", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("EHANDLER", "图元标识码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("LNO", "管线段编号", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("S_HIGH", "起点地面高程", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("E_HIGH", "终点地面高程", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("DESIGNUNIT", "设计单位", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("BUILDUNIT", "建设单位", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("SUPUNIT", "监理单位", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("CONUNIT", "施工单位", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("REPAIRUNIT", "检修单位", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("LRDATE", "最后一次检修日期", esriFieldType.esriFieldTypeDate),
                    FieldEx.CreateField("CTATUS", "连接状态", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("PRJCODE", "工程代码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("USTATUS", "使用状态", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("ISLOCK", "ISLOCK", esriFieldType.esriFieldTypeString)
                };
                fields = FieldEx.CreateBaseFields(esriGeometryType.esriGeometryPolyline, CreateSpatialReference(), true, false);
                FieldEx.AddFields(fields, otherFields);
            }
            else if (name.EndsWith("Point"))
            {
                List<IField> otherFields = new List<IField>
                {
                    FieldEx.CreateField("PRJ_NO", "工程编号", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("MAP_NO", "图上点号", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("EXP_NO", "管线点编号", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("TYPE", "管线点类别", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("X", "X坐标", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("Y", "Y坐标", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("HIGH", "地面高程", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("OFFSET", "管偏井的点号", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("ROTATION", "旋转角", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("CODE", "管线点代码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("FEATURE", "特征", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("SUBSID", "附属物", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("SURFBLDG", "建筑物", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("FEAMATERIAL", "特征点材质", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("SPEC","配件规格",esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("MODEL", "类型", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("INTERFACE", "接口方式", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("FEACODE", "特征点编号", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("WELLDEEP", "井底深", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("WELLNECK", "井脖高度", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("WELLSHAPE", "井盖形状", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("WELLMATERIAL", "井盖材质", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("WELLSIZE", "井盖尺寸", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("WELLPIPES", "接入管数", esriFieldType.esriFieldTypeInteger),
                    FieldEx.CreateField("ADDRESS", "管线点地址", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("BELONG", "权属单位代码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("MDATE", "埋设日期", esriFieldType.esriFieldTypeDate),
                    FieldEx.CreateField("MAPCODE", "图幅号", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("SUNIT", "探测单位代码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("SDATE", "探测日期", esriFieldType.esriFieldTypeDate),
                    FieldEx.CreateField("NOTE", "备注", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("EHANDLER", "图元标识码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("WATERDEEP", "排水井内水深", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("MUDDEEP", "排水井内泥深", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("LAMPNUM", "灯头数", esriFieldType.esriFieldTypeDouble),
                    FieldEx.CreateField("POLEHIGH", "灯杆高度", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("DESIGNUNIT", "设计单位", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("BUILDUNIT", "建设单位", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("SUPUNIT", "监理单位", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("CONUNIT", "施工单位", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("REPAIRUNIT", "检修单位", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("LRDATE", "最近检修日期", esriFieldType.esriFieldTypeDate),
                    FieldEx.CreateField("OPERASTATUS", "管线运行状况", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("CTATUS", "连接状态", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("PRJCODE", "工程代码", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("USTATUS", "使用状态", esriFieldType.esriFieldTypeString),
                    FieldEx.CreateField("ISLOCK", "ISLOCK", esriFieldType.esriFieldTypeString),
                };
                fields = FieldEx.CreateBaseFields(esriGeometryType.esriGeometryPolyline, CreateSpatialReference(), true, false);
                FieldEx.AddFields(fields, otherFields);
            }
            else
            {
                return null;
            }

            return workspace.CreateFeatureClass(name, fields);
            /**if (name.EndsWith("Annexe"))
            {
                return workspace.GetFeatureClassByName("TemplateAnnexe").CopyStruct(workspace, name);
            }
            else if (name.EndsWith("Line"))
            {
                return workspace.GetFeatureClassByName("TemplateLine").CopyStruct(workspace, name);
            }
            else if (name.EndsWith("Point"))
            {
                return workspace.GetFeatureClassByName("TemplatePoint").CopyStruct(workspace, name);
            }
            else
            {
                return null;
            }*/

        }


        public static ISpatialReference CreateSpatialReference()
        {
            string prj = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Data\EPSToMDB.prj");
            SpatialReferenceEnvironment environment = new SpatialReferenceEnvironmentClass();
            return environment.CreateESRISpatialReferenceFromPRJ(prj);
        }


        #endregion EPS转GDB
    }


}
