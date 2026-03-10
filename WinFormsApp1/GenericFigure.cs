using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Drawing;
using System.IO;

namespace WinFormsApp1;
internal class GenericFigure
{
    public Point[] ControlPoints { get; set; }
    public float Scale { get; set; }

    public GenericFigure()
    {
        ControlPoints = new Point[0];
        Scale = 1.0f;
    }
    public void InitializeFromFile(string filename)
    {
        // X1,Y1
        // X2,Y2
        // Scale

        string[] lines = File.ReadAllLines(filename);
        int pointCount = lines.Length - 1;

        ControlPoints = new Point[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            var coords = lines[i].Split(',');
            ControlPoints[i] = new Point(int.Parse(coords[0]), int.Parse(coords[1]));
        }

        Scale = float.Parse(lines[pointCount]);
    }

    public void Draw(Graphics g)
    {
        for (int i = 0; i < ControlPoints.Length; i++)
        {
            ControlPoints[i].X = (int)(ControlPoints[i].X * Scale);
            ControlPoints[i].Y = (int)(ControlPoints[i].Y * Scale);
        }

        if (ControlPoints.Length > 1)
        {
            g.DrawPolygon(Pens.Black, ControlPoints);
        }
    }

    public void Clear()
    {
        ControlPoints = new Point[0];
    }
}
