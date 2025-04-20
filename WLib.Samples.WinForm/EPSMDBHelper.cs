using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using WLib.ArcGis.GeoDatabase.FeatClass;
using WLib.ArcGis.GeoDatabase.Fields;
using WLib.ArcGis.Geometry;
using WLib.Database;
using WLib.ExtException;

namespace WLib.Samples.WinForm
{
    static class EPSMDBHelper
    {
        private static List<string> 英文层名;
        private static List<string> 中文层名;
        private static List<string> 点注记编码;
        private static List<string> 线注记编码;
        /// <summary>
        /// key:英文字段名
        /// value:中文字段名
        /// </summary>
        private static Dictionary<string, string> pointFieldDef;
        /// <summary>
        /// key:英文字段名
        /// value:中文字段名
        /// </summary>
        private static Dictionary<string, string> lineFieldDef;
        /// <summary>
        /// key:英文字段名
        /// value:中文字段名
        /// </summary>
        private static Dictionary<string, string> annexeFieldDef;
        /// <summary>
        /// key:英文字段名
        /// value:中文字段名
        /// </summary>
        private static Dictionary<string, string> textFieldDef;
        /// <summary>
        /// key:英文字段名
        /// value:中文字段名
        /// </summary>
        private static Dictionary<string, string> markFieldDef;

        static EPSMDBHelper()
        {
            英文层名 = new List<string> { "GD", "SD", "FD", "LD", "XH", "ZX", "DX", "YD", "LT", "SG", "GB", "DT", "RT", "JY", "BM", "JK", "GK", "YT", "TT", "RW", "SP", "HS", "YS", "WS", "ZS", "MQ", "YH", "TR", "ZQ", "RS", "QQ", "YY", "YQ", "SY", "NP", "ZH" };
            中文层名 = new List<string> { "供电", "输电", "发电", "城市照明", "交通信号", "直流专用线路", "中国电信", "中国移动", "中国联通", "省有线电视", "广播", "电力通讯", "热力通讯", "军用", "保密", "交通监控", "公共安全监控", "一汽通讯", "铁路通讯", "原水", "输配水", "雨污合流", "雨水", "污水", "中水", "煤气", "液化气", "天然气", "蒸汽", "热水", "氢气", "氧气", "乙炔", "石油", "不明管线", "综合管沟" };
            点注记编码 = new List<string> { "550142", "550642", "550742", "550242", "550342", "550842", "560542", "560642", "560742", "560242", "560442", "561142", "561242", "560842", "560942", "561342", "561542", "561442", "561042", "510142", "510242", "520342", "520142", "520242", "510342", "530142", "530242", "530342", "540242", "540142", "570142", "570242", "570342", "571142", "580242", "580142" };
            线注记编码 = new List<string> { "550141", "550641", "550741", "550241", "550341", "550841", "560541", "560641", "560741", "560241", "560441", "561141", "561241", "560841", "560941", "561341", "561541", "561441", "561041", "510141", "510241", "520341", "520141", "520241", "510341", "530141", "530241", "530341", "540241", "540141", "570141", "570241", "570341", "571141", "580241", "580141" };


            pointFieldDef = new Dictionary<string, string> { { "ID", "ID号" }, { "Prj_No", "普查测区的编号" }, { "Map_No", "图上点号" }, { "Exp_No", "管线点编号" }, { "Type", "管线点类别" }, { "X", "X坐标，单位m" }, { "Y", "Y坐标，单位m" }, { "High", "地面高程，单位m" }, { "Offset", "管偏井的点号" }, { "Rotation", "旋转角，单位弧度" }, { "Code", "管线点代码" }, { "Feature", "特征" }, { "Subsid", "附属物" }, { "SurfBldg", "地面建、构筑物" }, { "FeaMaterial", "特征点材质" }, { "Spec", "配件规格" }, { "Model", "类型" }, { "Interface", "接口方式" }, { "FeaCode", "特征点编号" }, { "WellDeep", "井底深，单位m" }, { "WellNeck", "井脖高度，单位mm" }, { "WellShape", "井盖形状" }, { "WellMaterial", "井盖材质" }, { "WellSize", "井盖尺寸，单位mm" }, { "WellPipes", "接入管数" }, { "Address", "管线点地址（道路名称）" }, { "Belong", "权属单位代码" }, { "MDate", "埋设日期" }, { "MapCode", "图幅号" }, { "SUnit", "探测单位代码" }, { "SDate", "探测日期" }, { "Note", "备注" }, { "Ehandler", "图元标识码" }, { "WaterDeep", "排水井内水深" }, { "MudDeep", "排水井内泥深" }, { "LampNum", "灯头数" }, { "PoleHigh", "灯杆高度，单位m" }, { "DesignUnit", "设计单位" }, { "BuildUnit", "建设单位" }, { "SupUnit", "监理单位" }, { "ConUnit", "施工单位" }, { "RepairUnit", "检修单位" }, { "LRDate", "最后一次检修日期" }, { "OperaStatus", "管线运行状况" }, { "Ctatus", "连接状态" }, { "PrjCode", "工程代码" }, { "Ustatus", "使用状态" } };
            lineFieldDef = new Dictionary<string, string> { { "ID", "ID号" }, { "Prj_No", "普查测区编号" }, { "S_Exp", "起点管线点号" }, { "S_X", "起点X坐标，单位m" }, { "S_Y", "起点Y坐标，单位m" }, { "S_Deep", "起点管线埋深，单位m" }, { "E_Exp", "下一点管线点号" }, { "E_X", "下一点X坐标，单位m" }, { "E_Y", "下一点Y坐标，单位m" }, { "E_Deep", "下一点管线埋深，单位m" }, { "Type", "管线种类" }, { "Code", "管线代码" }, { "Material", "材质(管、沟、块)" }, { "Mat_Cen", "线芯材质" }, { "Interface", "接口方式" }, { "PSize", "管径或断面尺寸，单位mm" }, { "Voltage", "电压值，单位kV" }, { "Pressure", "压力" }, { "Temperature", "温度，单位℃" }, { "CabNum", "电缆条数" }, { "TotalHole", "总孔数" }, { "UsedHole", "已用孔数" }, { "FlowDir", "排水流向（“＋”起点到下一点；“－”下一点到起点）" }, { "Address", "管线段地址（道路名称）" }, { "EmBed", "埋设方式" }, { "MDate", "埋设日期" }, { "Belong", "权属单位代码" }, { "SUnit", "探测单位代码" }, { "SDate", "探测日期" }, { "Note", "备注" }, { "Ehandler", "图元标识码" }, { "Lno", "管线段编号" }, { "DesignUnit", "设计单位" }, { "BuildUnit", "建设单位" }, { "SupUnit", "监理单位" }, { "ConUnit", "施工单位" }, { "RepairUnit", "检修单位" }, { "LRDate", "最后一次检修日期" }, { "Ctatus", "连接状态" }, { "PrjCode", "工程代码" }, { "Ustatus", "使用状态" } };
            annexeFieldDef = new Dictionary<string, string> { { "ID", "ID号" }, { "SeqId", "附属物中心点的管线点编号" }, { "Code", "图元标识码" }, { "Exp_No", "每个附属物的行序号" }, { "arrX", "X坐标数组" }, { "arrY", "Y坐标数组" }, { "arrZ", "Z坐标数组" }, { "PointType", "点类型数组" }, { "Ehandler", "图元标识码" } };
            textFieldDef = new Dictionary<string, string> { { "ID", "ID号" }, { "Map_No", "注记内容" }, { "Exp_No", "管线（点）编号" }, { "X", "X坐标" }, { "Y", "Y坐标" }, { "MapCode", "图幅号" }, { "Ehandler", "图元标识码" } };
            markFieldDef = new Dictionary<string, string> { { "ID", "表的行序号" }, { "X", "X坐标" }, { "Y", "Y坐标" }, { "Rotation", "旋转角" }, { "Mark", "专业标注的字符串" }, { "LNo", "标注对应的管线段编号" }, { "Ehandler", "图元标识码" } };

        }
        private static string 输出点表名(string 英文层名)
        {
            return 英文层名 + "Point";
        }
        private static string 输出线表名(string 英文层名)
        {
            return 英文层名 + "Line";
        }
        private static string 输出面表名(string 英文层名)
        {
            return 英文层名 + "Annexe";
        }
        private static string 输出注记表名1_点标注(string 英文层名)
        {
            return 英文层名 + "Text";
        }
        private static string 输出注记表名2_线标注(string 英文层名)
        {
            return 英文层名 + "Mark";
        }

        private static void CreateDB(string path)
        {
            //从Resource读取模板创建数据库
            File.WriteAllBytes(path, Properties.Resources.MDB格式模板);
        }
        private static OleDbConnection ConnectToDB(string path, string password = null)
        {
            try
            {
                var connStr = DbHelper.Access_OleDb12(path, password);
                var conn = new OleDbConnection(connStr);
                conn.Open();
                return conn;
            }
            catch (Exception)
            {
                var connStr = DbHelper.Access_OleDb4(path, password);
                var conn = new OleDbConnection(connStr);
                conn.Open();
                return conn;
            }
        }

        public static void insertToMDB(IFeatureClass featureClass, string path, bool createNewFileIfNotExist = true,
        Action<string> callback = null)
        {
            if (!File.Exists(path))//如果文件不存在，则创建
            {
                if (createNewFileIfNotExist)
                    CreateDB(path);
                else
                    throw new FileNotFoundException($"文件{path}不存在");
            }

            if (featureClass.GetName() == "")
                throw new Exception("要素类名称不能为空");
            using (var conn = ConnectToDB(path))//打开数据库连接
            {
                callback?.Invoke(featureClass.GetName());
                insertToMDB(featureClass, conn);
            }
        }




        private static void insertToMDB(IFeatureClass featureClass, OleDbConnection conn)
        {
            string mdbTableName = featureClass.GetName();
            if (mdbTableName.EndsWith("Point"))
            {
                //点表
                //insertToPointTable(featureClass, conn);
            }
            else if (mdbTableName.EndsWith("Line"))
            {
                //线表
                insertToLineTable(featureClass, conn);
            }
            else if (mdbTableName.EndsWith("Annexe"))
            {
                //面表
                //insertToAnnexeTable(featureClass, conn);
            }
            else if (mdbTableName.EndsWith("Text"))
            {
                //点标注表
            }
            else if (mdbTableName.EndsWith("Mark"))
            {
                //线标注表
            }
            else
            {
                return;
            }

        }

        private static void insertToPointTable(this IFeatureClass featureClass, OleDbConnection conn)
        {
            string[] keys = new string[lineFieldDef.Count];
            lineFieldDef.Keys.CopyTo(keys, 0);
            IEnumerable<int> idx = featureClass.GetFieldIndexs(keys);

            Dictionary<string, int> fieldIndexs = new Dictionary<string, int>();
            for (int i = 0; i < keys.Length; i++)
            {
                if (idx.ElementAt(i) == -1) continue;
                fieldIndexs.Add(keys[i], idx.ElementAt(i));
            }

            fieldIndexs.Add("ID", featureClass.GetFieldIndex(featureClass.OIDFieldName));


            string sql = $"INSERT INTO {featureClass.GetName()} ({string.Join(",", fieldIndexs.Keys.Select(k => $"[{k}]"))}) " +
                         $"VALUES ({string.Join(",", fieldIndexs.Keys.Select(k => $"@{k}"))})";
            OleDbTransaction trans = conn.BeginTransaction();
            try
            {
                using (OleDbCommand cmd = new OleDbCommand(sql, conn, trans))
                {
                    foreach (IFeature fea in featureClass.QueryFeatures(null))
                    {
                        cmd.Parameters.Clear();
                        foreach (var item in fieldIndexs)
                        {
                            cmd.Parameters.AddWithValue("@" + item.Key, fea.Value[item.Value]);
                        }
                        cmd.ExecuteNonQuery();
                    }

                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }

        }
        public static void insertToLineTable(this IFeatureClass featureClass, OleDbConnection conn)
        {
            string[] keys = new string[lineFieldDef.Count];
            lineFieldDef.Keys.CopyTo(keys, 0);
            IEnumerable<int> idx = featureClass.GetFieldIndexs(keys);

            Dictionary<string, int> fieldIndexs = new Dictionary<string, int>();
            for (int i = 0; i < keys.Length; i++)
            {
                if (idx.ElementAt(i) == -1) continue;
                fieldIndexs.Add(keys[i], idx.ElementAt(i));
            }

            fieldIndexs.Add("ID", featureClass.GetFieldIndex(featureClass.OIDFieldName));


            string sql = $"INSERT INTO {featureClass.GetName()} ({string.Join(",", fieldIndexs.Keys.Select(k => $"[{k}]"))}) " +
                         $"VALUES ({string.Join(",", fieldIndexs.Keys.Select(k => $"@{k}"))})";

            OleDbTransaction trans = conn.BeginTransaction();
            try
            {
                using (OleDbCommand cmd = new OleDbCommand(sql, conn, trans))
                {
                    foreach (IFeature fea in featureClass.QueryFeatures(null))
                    {
                        cmd.Parameters.Clear();
                        foreach (var item in fieldIndexs)
                        {
                            cmd.Parameters.AddWithValue("@" + item.Key, fea.Value[item.Value]);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }

        }
        public static void insertToAnnexeTable(this IFeatureClass featureClass, OleDbConnection conn)
        {

            Func<IFeature, string[]> createArray = (feature) =>
               {
                   IPoint[] pts = feature.ShapeCopy.GetPointArray();
                   string arrX = string.Join(",", pts.Select(p => p.X.ToString()));
                   string arrY = string.Join(",", pts.Select(p => p.Y.ToString()));
                   string arrZ = string.Join(",", pts.Select(p => p.Z.ToString()));
                   return new string[] { arrX, arrY, arrZ };
               };
            string sql = $"insert into {featureClass.GetName()} (ID,SeqId,Code,Exp_No,PointType,Ehandler,arrX,arrY,arrZ) Values(@ID,@SeqId,@Code,@Exp_No,@PointType,@Ehandler,@arrX,@arrY,@arrZ)";

            Dictionary<string, int> fieldIndexs = new Dictionary<string, int>();
            string[] fields = new string[] { "ID", "SeqId", "Code", "Exp_No", "Ehandler" };
            foreach (var item in fields)
            {
                fieldIndexs.Add(item, featureClass.GetFieldIndex(item.ToUpper()));
            }
            OleDbTransaction trans = conn.BeginTransaction();
            try
            {
                using (OleDbCommand cmd = new OleDbCommand(sql, conn, trans))
                {
                    foreach (IFeature fea in featureClass.QueryFeatures(null))
                    {
                        cmd.Parameters.Clear();
                        foreach (var item in fieldIndexs)
                        {
                            cmd.Parameters.AddWithValue("@" + item.Key, fea.Value[item.Value]);
                        }
                        //PointType 0#0#0#0#0
                        cmd.Parameters.AddWithValue("@PointType", "0#0#0#0#0");
                        //arr
                        string[] arr = createArray(fea);
                        cmd.Parameters.AddWithValue("@arrX", arr[0]);
                        cmd.Parameters.AddWithValue("@arrY", arr[1]);
                        cmd.Parameters.AddWithValue("@arrZ", arr[2]);
                        cmd.ExecuteNonQuery();
                    }
                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw ex;
            }
        }

    }
}
