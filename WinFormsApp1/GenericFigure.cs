using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace WinFormsApp1;

internal class GenericFigure
{
    public PointF[] ControlPoints { get; set; }
    public float Scale { get; set; }
    public PointF Center { get; set; }

    public GenericFigure()
    {
        ControlPoints = new PointF[0];
        Scale = 1.0f;
        Center = new PointF(0, 0);
    }

    public void InitializeFromFile(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        int pointCount = lines.Length - 1;

        ControlPoints = new PointF[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            var coords = lines[i].Split(',');
            ControlPoints[i] = new PointF(float.Parse(coords[0]), float.Parse(coords[1]));
        }

        Scale = float.Parse(lines[pointCount]);
        UpdateCenter();
    }

    public void UpdateCenter()
    {
        if (ControlPoints.Length == 0) return;
        float avgX = ControlPoints.Average(p => p.X);
        float avgY = ControlPoints.Average(p => p.Y);
        Center = new PointF(avgX, avgY);
    }

    public void Draw(Graphics g)
    {
        if (ControlPoints.Length <= 1) return;

        PointF[] pointsToDraw = new PointF[ControlPoints.Length];
        for (int i = 0; i < ControlPoints.Length; i++)
        {
            pointsToDraw[i] = new PointF(
                Center.X + (ControlPoints[i].X - Center.X) * Scale,
                Center.Y + (ControlPoints[i].Y - Center.Y) * Scale
            );
        }

        g.DrawPolygon(Pens.Black, pointsToDraw);
    }

    public void Clear()
    {
        ControlPoints = new PointF[0];
        Center = new PointF(0, 0);
        Scale = 1.0f;
    }

    public void Move(float deltaX, float deltaY)
    {
        for (int i = 0; i < ControlPoints.Length; i++)
        {
            ControlPoints[i].X += deltaX;
            ControlPoints[i].Y += deltaY;
        }
        Center = new PointF(Center.X + deltaX, Center.Y + deltaY);
    }

    public void Rotate(float angleDegrees)
    {
        float angleRadians = angleDegrees * (float)Math.PI / 180f;
        float cosTheta = (float)Math.Cos(angleRadians);
        float sinTheta = (float)Math.Sin(angleRadians);

        for (int i = 0; i < ControlPoints.Length; i++)
        {
            float x = ControlPoints[i].X - Center.X;
            float y = ControlPoints[i].Y - Center.Y;

            float rotatedX = x * cosTheta - y * sinTheta;
            float rotatedY = x * sinTheta + y * cosTheta;

            ControlPoints[i] = new PointF(rotatedX + Center.X, rotatedY + Center.Y);
        }
    }
    //3.3
    public string AnalyzePolygon()
    {
        if (ControlPoints.Length < 3) return "Недостатньо точок для багатокутника.";

        int n = ControlPoints.Length;
        bool hasPositive = false;
        bool hasNegative = false;

        for (int i = 0; i < n; i++)
        {
            PointF p1 = ControlPoints[i];
            PointF p2 = ControlPoints[(i + 1) % n];
            PointF p3 = ControlPoints[(i + 2) % n];

            // За правилом векторного добутку якщо
            //    тіки поворити в одну сторону - фігура опукла
            //    в обидні - фігура ввігнута
            float crossProduct = (p2.X - p1.X) * (p3.Y - p2.Y) - (p2.Y - p1.Y) * (p3.X - p2.X);

            if (crossProduct > 0) hasPositive = true;
            if (crossProduct < 0) hasNegative = true;

            if (hasPositive && hasNegative) return "Фігура ввігнута";
        }

        if (hasPositive) return "Фігура опукла, обхід: за годинниковою стрілкою";
        if (hasNegative) return "Фігура опукла, обхід: проти годинникової стрілки";

        return "Точки лежать на одній лінії";
    }
    //3.4
    public void BuildConvexHull(PointF[] points)
    {
        if (points.Length < 3)
        {
            ControlPoints = points;
            return;
        }

        PointF lowestPoint = points.OrderBy(p => p.Y).ThenBy(p => p.X).First();

        var sortedPoints = points
            .Where(p => p != lowestPoint)
            .OrderBy(p => Math.Atan2(p.Y - lowestPoint.Y, p.X - lowestPoint.X)) //кут тангенс якого є
                                                                                //різницею двох значень в параметрах,
                                                                                //відповідно (різниця Y, різниця X)
            .ToList();

        List<PointF> hull = new List<PointF> { lowestPoint, sortedPoints[0] };

        //по логіці з 3.3 видаляємо праві поворити, щоб сформувати оболонку
        for (int i = 1; i < sortedPoints.Count; i++)
        {
            while (hull.Count >= 2)
            {
                PointF p1 = hull[hull.Count - 2];
                PointF p2 = hull[hull.Count - 1];
                PointF p3 = sortedPoints[i];

                float crossProduct = (p2.X - p1.X) * (p3.Y - p2.Y) - (p2.Y - p1.Y) * (p3.X - p2.X);

                if (crossProduct <= 0)
                    hull.RemoveAt(hull.Count - 1);
                else
                    break;
            }
            hull.Add(sortedPoints[i]);
        }

        ControlPoints = hull.ToArray();
        UpdateCenter();
    }
}