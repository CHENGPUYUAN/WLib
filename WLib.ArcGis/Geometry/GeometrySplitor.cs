﻿/*---------------------------------------------------------------- 
// auth： Windragon
// date： 2020
// desc： None
// mdfy:  None
//----------------------------------------------------------------*/

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WLib.ArcGis.Geometry
{
    /// <summary>
    /// 几何图形的切分
    /// </summary>
    public static class GeometrySplitor
    {
        /// <summary>
        /// 按照指定的各部分的权重将一个多边形切分成多个
        /// </summary>
        /// <param name="polygon">被切分的多边形</param>
        /// <param name="weights">切分后的多边形各个部分的权重</param>
        /// <param name="direction">切分多边形的方向，0为横向，1为纵向</param>
        /// <param name="tolerance">面积容差</param>
        /// <returns></returns>
        public static List<IPolygon> SplitPolygonByAreaWeights(this IPolygon polygon, double[] weights, int direction, double tolerance = 0.001)
        {
            if (polygon.IsEmpty)
                throw new Exception("几何图形不能为空(Empty)！");
            if (tolerance < 0)
                throw new Exception("指定的容差不能小于0");

            var resultPolygons = new List<IPolygon>();
            var tmpPolygon = polygon as IPolygon;
            for (int i = 0; i < weights.Length; i++)
            {
                var rate = weights[i] / weights.Skip(i).Sum();
                if (rate == 1)
                {
                    resultPolygons.Add(tmpPolygon);
                    continue;
                }

                var resultPolygon = SplitPolygonByAreaRate(tmpPolygon, rate, direction, tolerance) as IPolygon;//截取多边形
                ITopologicalOperator logicalOpt = tmpPolygon as ITopologicalOperator;
                tmpPolygon = logicalOpt.Difference(resultPolygon) as IPolygon;//获取多边形截取后的剩余部分

                resultPolygons.Add(resultPolygon);
            }
            return resultPolygons;
        }
        /// <summary>
        /// 按照指定的各部分的面积将一个多边形切分成多个
        /// （注意：指定的各部分的面积总和大于原多边形面积时，抛出异常；小于原多边形面积时，剩余的部分也将加入返回结果中）
        /// </summary>
        /// <param name="polygon">被切分的多边形</param>
        /// <param name="areas">切分后的多边形各个部分的面积，面积不能小于容差</param>
        /// <param name="direction">切分多边形的方向，0为横向，1为纵向</param>
        /// <param name="tolerance">面积容差</param>
        /// <returns></returns>
        public static List<IPolygon> SplitPolygonByAreas(this IPolygon polygon, double[] areas, int direction, double tolerance = 0.001)
        {
            if (polygon.IsEmpty)
                throw new Exception("几何图形不能为空(Empty)！");
            if (tolerance < 0)
                throw new Exception("指定的容差不能小于0");

            var polygonArea = (polygon as IArea).Area;
            if (areas.Sum() - polygonArea > tolerance * areas.Length)
                throw new Exception("指定的各部分的面积总和，大于原多边形面积！");
            if (areas.Any(v => v <= tolerance))
                throw new Exception("指定的各部分的面积，存在小于容差的面积！");

            var resultPolygons = new List<IPolygon>();
            var tmpPolygon = polygon as IPolygon;
            for (int i = 0; i < areas.Length; i++)
            {
                var resultPolygon = SplitPolygonByArea(tmpPolygon, areas[i], direction, tolerance) as IPolygon;//截取多边形
                ITopologicalOperator logicalOpt = tmpPolygon as ITopologicalOperator;
                tmpPolygon = logicalOpt.Difference(resultPolygon) as IPolygon;//获取多边形截取后的剩余部分

                resultPolygons.Add(resultPolygon);
            }

            //剩余部分也加入返回结果中
            if (!tmpPolygon.IsEmpty)
            {
                double tmpArea = (tmpPolygon as IArea).Area;
                resultPolygons.Add(tmpPolygon);
            }

            return resultPolygons;
        }
        /// <summary>
        /// 按照指定的面积占比截取多边形
        /// </summary>
        /// <param name="polygon">被切分的多边形</param>
        /// <param name="rate">切分后的多边形各个部分的占比，值应小于或等于1</param>
        /// <param name="direction">切分多边形的方向，0为横向，1为纵向</param>
        /// <param name="tolerance">面积容差</param>
        /// <returns></returns>
        private static IGeometry SplitPolygonByAreaRate(IPolygon polygon, double rate, int direction, double tolerance = 0.001)
        {
            if (rate > 1)
                throw new ArgumentException("从多边形截取的面积占比(参数：rate)应该小于或等于1");

            var sumArea = (polygon as IArea).Area;
            double rateArea = rate * sumArea;//被分割的多边形的面积

            return SplitPolygonByArea(polygon, rateArea, direction, tolerance);
        }
        /// <summary>
        /// 按照指定的面积截取多边形
        /// </summary>
        /// <param name="polygon">被切分的多边形</param>
        /// <param name="clipArea">从多边形中截取区块的面积</param>
        /// <param name="direction">切分多边形的方向，0为横向，1为纵向</param>
        /// <param name="tolerance">面积容差</param>
        /// <returns></returns>
        private static IGeometry SplitPolygonByArea(IPolygon polygon, double clipArea, int direction, double tolerance = 0.001)
        {
            IGeometry resultGeometry = null;
            ITopologicalOperator logicalOpt = polygon as ITopologicalOperator;

            //获取多边形的包围盒(Envelop)，不断移动包围盒并与多边形相交，相交面积符合要求时相交部分的图斑就是要截取的图斑
            var envelope = polygon.Envelope;
            double offsetX = 0.0, minOffsetX = 0.0;//Envelop在X方向上移动的距离、Envelop在X方向上最小移动距离
            double offsetY = 0.0, minOffsetY = 0.0;//Envelop在Y方向上移动的距离、Envelop在Y方向上最小移动距离

            if (direction == 0)//横切多边形，Envolpe在Y方向上移动
            {
                offsetY = envelope.Height / 2;
                minOffsetY = tolerance / envelope.Width;
            }
            else if (direction == 1)//纵切多边形，Envolpe在X方向上移动
            {
                offsetX = envelope.Width / 2;
                minOffsetX = tolerance / envelope.Height;
            }
            envelope.Offset(-offsetX, offsetY);

            while (true)
            {
                IGeometry intersectGeo = logicalOpt.Intersect(envelope, esriGeometryDimension.esriGeometry2Dimension);
                double tmpArea = ((intersectGeo as IPolygon) as IArea).Area;
                double difArea = Math.Abs(tmpArea - clipArea);
                if (difArea <= tolerance)
                {
                    resultGeometry = intersectGeo;
                    break;
                }

                if (direction == 0)
                    offsetY = offsetY <= minOffsetY ? minOffsetY : offsetY / 2;
                else
                    offsetX = offsetX <= minOffsetX ? minOffsetX : offsetX / 2;

                if (tmpArea > clipArea)
                    envelope.Offset(-offsetX, offsetY);
                else
                    envelope.Offset(offsetX, -offsetY);

                //if (direction == 0)
                //{
                //    offsetY = offsetY <= minOffsetY ? minOffsetY : offsetY / 2;
                //    if (tmpArea > clipArea)
                //        envelope.Offset(0, offsetY);
                //    else
                //        envelope.Offset(0, -offsetY);
                //}
                //else
                //{
                //    offsetX = offsetX <= minOffsetX ? offsetX : offsetX / 2;
                //    if (tmpArea > clipArea)
                //        envelope.Offset(-offsetX, 0);
                //    else
                //        envelope.Offset(offsetX, 0);
                //}
            }
            return resultGeometry;
        }


        /// <summary>
        /// 按照指定的分割线切分要素，返回被切分的要素
        /// </summary>
        /// <param name="feature">要切分的要素</param>
        /// <param name="polyline">分割线</param>
        /// <returns></returns>
        public static List<IFeature> SplitFeatureByLine(this IFeature feature, IPolyline polyline)
        {
            IFeatureEdit featureEdit = feature as IFeatureEdit;
            ISet set = featureEdit.Split(polyline);//直接用线分割
            set.Reset();
            var resultFeatures = new List<IFeature>();

            IFeature resultFeature;
            while ((resultFeature = set.Next() as IFeature) != null)
            {
                resultFeatures.Add(resultFeature);
            }
            return resultFeatures;
        }
        /// <summary>
        /// 按照指定的分割线切分要素，返回被切分的要素图斑
        /// </summary>
        /// <param name="feature">要切分的要素</param>
        /// <param name="polyline">分割线</param>
        /// <returns></returns>
        public static List<IGeometry> SplitFeatureByLine2(this IFeature feature, IPolyline polyline)
        {
            return SplitFeatureByLine(feature, polyline).Select(f => f.Shape).ToList();
        }
    }
}
