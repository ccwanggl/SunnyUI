﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace Sunny.UI
{
    public sealed class UILineOption : UIOption, IDisposable
    {
        public UIAxis XAxis { get; set; } = new UIAxis(UIAxisType.Value);

        public UIAxis YAxis { get; set; } = new UIAxis(UIAxisType.Value);

        public UIBarToolTip ToolTip { get; set; }

        public void Dispose()
        {
            Clear();
        }

        public UIChartGrid Grid = new UIChartGrid();

        public UIAxisType XAxisType { get; set; } = UIAxisType.Value;

        public UIAxisType YAxisType { get; set; } = UIAxisType.Value;

        public ConcurrentDictionary<string, UILineSeries> Series = new ConcurrentDictionary<string, UILineSeries>();

        public readonly List<UIScaleLine> XAxisScaleLines = new List<UIScaleLine>();

        public readonly List<UIScaleLine> YAxisScaleLines = new List<UIScaleLine>();

        public UILineWarningArea GreaterWarningArea { get; set; }
        public UILineWarningArea LessWarningArea { get; set; }

        public UILineSeries AddSeries(UILineSeries series)
        {
            if (series.Name.IsNullOrEmpty()) return null;
            Series.TryAdd(series.Name, series);
            return series;
        }

        public UILineSeries AddSeries(string name)
        {
            if (name.IsNullOrEmpty()) return null;
            UILineSeries series = new UILineSeries(name);
            Series.TryAdd(series.Name, series);
            return series;
        }

        public void AddData(string name, double x, double y)
        {
            if (!Series.ContainsKey(name)) return;
            Series[name].Add(x, y);
        }

        public void AddData(string name, DateTime x, double y)
        {
            if (!Series.ContainsKey(name)) return;
            Series[name].Add(x, y);
        }

        public void AddData(string name, string x, double y)
        {
            if (!Series.ContainsKey(name)) return;
            Series[name].Add(x, y);
        }

        public void AddData(string name, List<double> x, List<double> y)
        {
            if (x.Count != y.Count) return;
            for (int i = 0; i < x.Count; i++)
            {
                AddData(name, x[i], y[i]);
            }
        }

        public void AddData(string name, List<DateTime> x, List<double> y)
        {
            if (x.Count != y.Count) return;
            for (int i = 0; i < x.Count; i++)
            {
                AddData(name, x[i], y[i]);
            }
        }

        public void AddData(string name, List<string> x, List<double> y)
        {
            if (x.Count != y.Count) return;
            for (int i = 0; i < x.Count; i++)
            {
                AddData(name, x[i], y[i]);
            }
        }

        public void AddData(string name, double[] x, double[] y)
        {
            if (x.Length != y.Length) return;
            for (int i = 0; i < x.Length; i++)
            {
                AddData(name, x[i], y[i]);
            }
        }

        public void AddData(string name, DateTime[] x, double[] y)
        {
            if (x.Length != y.Length) return;
            for (int i = 0; i < x.Length; i++)
            {
                AddData(name, x[i], y[i]);
            }
        }

        public void AddData(string name, string[] x, double[] y)
        {
            if (x.Length != y.Length) return;
            for (int i = 0; i < x.Length; i++)
            {
                AddData(name, x[i], y[i]);
            }
        }

        public void Clear()
        {
            foreach (var series in Series.Values)
            {
                series.Clear();
            }

            Series.Clear();
        }

        public void Clear(string name)
        {
            if (Series.ContainsKey(name))
            {
                Series[name].Clear();
            }
        }

        public void SetLabels(string[] labels)
        {
            XAxis.Clear();
            if (XAxis.Type == UIAxisType.Category)
            {
                foreach (var label in labels)
                {
                    AddLabel(label);
                }
            }
        }

        public void AddLabel(string label)
        {
            if (XAxis.Type == UIAxisType.Category)
            {
                XAxis.Data.Add(label);
            }
        }
    }

    public class UILineSeries
    {
        public string Name { get; private set; }

        public float Width { get; set; } = 2;
        public Color Color { get; set; }

        public UILinePointSymbol Symbol { get; set; } = UILinePointSymbol.None;
        public int SymbolSize { get; set; } = 4;

        public int SymbolLineWidth { get; set; } = 1;

        public Color SymbolColor { get; set; }

        public bool CustomColor { get; set; }

        public bool Smooth { get; set; }

        public UILineSeries(string name)
        {
            Name = name;
            Color = UIColor.Blue;
        }

        public UILineSeries(string name, Color color)
        {
            Name = name;
            Color = color;
            CustomColor = true;
        }

        public readonly List<double> XData = new List<double>();

        public readonly List<double> YData = new List<double>();

        public readonly List<PointF> Points = new List<PointF>();

        private readonly List<double> PointsX = new List<double>();

        private readonly List<double> PointsY = new List<double>();

        public int DataCount => XData.Count;

        public bool GetNearestPoint(Point p, int offset, out double x, out double y, out int index)
        {
            index = PointsX.BinarySearchNearIndex(p.X);
            if (p.X >= PointsX[index] - offset && p.X <= PointsX[index] + offset &&
                p.Y >= PointsY[index] - offset && p.Y <= PointsY[index] + offset)
            {
                x = XData[index];
                y = YData[index];
                return true;
            }

            x = 0;
            y = 0;
            return false;
        }

        public void Clear()
        {
            XData.Clear();
            YData.Clear();
            ClearPoints();
        }

        public void ClearPoints()
        {
            Points.Clear();
            PointsX.Clear();
            PointsY.Clear();
        }

        public void AddPoint(PointF point)
        {
            Points.Add(point);
            PointsX.Add(point.X);
            PointsY.Add(point.Y);
        }

        public void Add(double x, double y)
        {
            XData.Add(x);
            YData.Add(y);
        }

        public void Add(DateTime x, double y)
        {
            DateTimeInt64 t = new DateTimeInt64(x);
            XData.Add(t);
            YData.Add(y);
        }

        public void Add(string x, double y)
        {
            int cnt = XData.Count;
            XData.Add(cnt);
            YData.Add(y);
        }
    }

    public enum UILinePointSymbol
    {
        None,
        Square,
        Diamond,
        Triangle,
        Circle,
        Plus,
        Star
    }

    public struct UILineSelectPoint
    {
        public string Name { get; set; }

        public int Index { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }

    public class UILineWarningArea
    {
        public double Value { get; set; }

        public Color Color { get; set; } = Color.Red;

        public UILineWarningArea()
        {

        }

        public UILineWarningArea(double value)
        {
            Value = value;
        }

        public UILineWarningArea(double value, Color color)
        {
            Value = value;
            Color = color;
        }
    }
}
