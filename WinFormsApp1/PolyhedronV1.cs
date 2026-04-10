using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace WinFormsApp1;

public class PolyhedronV1
{
    public int VertexCount;
    public double[,] Vertices;
    public int[,] AdjacencyMatrix;
    public int[][] Faces;

    public void InitializeFromFile(string path)
    {
        string[] lines = File.ReadAllLines(path);
        VertexCount = int.Parse(lines[0]);
        Vertices = new double[VertexCount, 3];
        AdjacencyMatrix = new int[VertexCount, VertexCount];

        for (int i = 0; i < VertexCount; i++)
        {
            string[] coords = lines[i + 1].Split(' ');
            Vertices[i, 0] = double.Parse(coords[0]);
            Vertices[i, 1] = double.Parse(coords[1]);
            Vertices[i, 2] = double.Parse(coords[2]);
        }

        for (int i = 0; i < VertexCount; i++)
        {
            string[] row = lines[i + VertexCount + 1].Split(' ');
            for (int j = 0; j < VertexCount; j++)
                AdjacencyMatrix[i, j] = int.Parse(row[j]);
        }

        InitializeFaces();
    }

    private void InitializeFaces()
    {
        if (VertexCount == 4)
        {
            Faces = new int[][]
            {
                new int[] {0, 1, 2},
                new int[] {0, 2, 3},
                new int[] {0, 1, 3},
                new int[] {1, 2, 3}
            };
        }
        else if (VertexCount == 8)
        {
            Faces = new int[][]
            {
                new int[] {0, 1, 2, 3},
                new int[] {5, 4, 7, 6},
                new int[] {4, 0, 3, 7},
                new int[] {1, 5, 6, 2},
                new int[] {4, 5, 1, 0},
                new int[] {3, 2, 6, 7}
            };
        }
    }

    protected double[] GetCentroid()
    {
        double cx = 0, cy = 0, cz = 0;
        for (int i = 0; i < VertexCount; i++)
        {
            cx += Vertices[i, 0];
            cy += Vertices[i, 1];
            cz += Vertices[i, 2];
        }
        return new double[] { cx / VertexCount, cy / VertexCount, cz / VertexCount };
    }

    public void Scale(double factor)
    {
        double[] center = GetCentroid();
        for (int i = 0; i < VertexCount; i++)
        {
            Vertices[i, 0] = center[0] + (Vertices[i, 0] - center[0]) * factor;
            Vertices[i, 1] = center[1] + (Vertices[i, 1] - center[1]) * factor;
            Vertices[i, 2] = center[2] + (Vertices[i, 2] - center[2]) * factor;
        }
    }

    public void Rotate(double angleDeg, string axis)
    {
        double rad = angleDeg * Math.PI / 180.0;
        double[] center = GetCentroid();

        for (int i = 0; i < VertexCount; i++)
        {
            double x = Vertices[i, 0] - center[0];
            double y = Vertices[i, 1] - center[1];
            double z = Vertices[i, 2] - center[2];
            double nx = x, ny = y, nz = z;

            if (axis == "X")
            {
                ny = y * Math.Cos(rad) - z * Math.Sin(rad);
                nz = y * Math.Sin(rad) + z * Math.Cos(rad);
            }
            else if (axis == "Y")
            {
                nx = x * Math.Cos(rad) + z * Math.Sin(rad);
                nz = -x * Math.Sin(rad) + z * Math.Cos(rad);
            }
            else if (axis == "Z")
            {
                nx = x * Math.Cos(rad) - y * Math.Sin(rad);
                ny = x * Math.Sin(rad) + y * Math.Cos(rad);
            }

            Vertices[i, 0] = nx + center[0];
            Vertices[i, 1] = ny + center[1];
            Vertices[i, 2] = nz + center[2];
        }
    }

    public bool IsFaceVisible(int[] faceIndices)
    {
        if (faceIndices.Length < 3) return false;

        double x1 = Vertices[faceIndices[0], 0];
        double y1 = Vertices[faceIndices[0], 1];
        double z1 = Vertices[faceIndices[0], 2];

        double x2 = Vertices[faceIndices[1], 0];
        double y2 = Vertices[faceIndices[1], 1];
        double z2 = Vertices[faceIndices[1], 2];

        double x3 = Vertices[faceIndices[2], 0];
        double y3 = Vertices[faceIndices[2], 1];
        double z3 = Vertices[faceIndices[2], 2];
        
        double v1x = x2 - x1;
        double v1y = y2 - y1;
        double v1z = z2 - z1;

        double v2x = x3 - x1;
        double v2y = y3 - y1;
        double v2z = z3 - z1;

        double nz = v1x * v2y - v1y * v2x;

        if (Math.Abs(nz) < 0.0001)
        {
            return false;
        }

        return nz > 0; // цікаво виглядає коли nz < 0 || nz > 0;
    }

    public void DrawWithBackFaceCulling(Graphics g, int width, int height, float scaleX, float scaleY)
    {
        if (Faces == null) return;

        int offsetX = width / 2;
        int offsetY = height / 2;

        foreach (var face in Faces)
        {
            if (IsFaceVisible(face))
            {
                PointF[] points = new PointF[face.Length];
                for (int i = 0; i < face.Length; i++)
                {
                    int idx = face[i];
                    points[i] = new PointF(
                        (float)Vertices[idx, 0] * scaleX + offsetX,
                        (float)Vertices[idx, 1] * scaleY + offsetY
                    );
                }

                g.FillPolygon(Brushes.LightBlue, points);
                g.DrawPolygon(new Pen(Color.Black, 2), points);
            }
        }
    }
    public void DrawPerspective(Graphics g, int width, int height, double d)
    {
        int offsetX = width / 2;
        int offsetY = height / 2;
        double offsetZ = /* (d / 100) * */ 400;
        Pen pen = new Pen(Color.Cyan, 2);

        PointF[] projectedPoints = new PointF[VertexCount];

        for (int i = 0; i < VertexCount; i++)
        {
            double[] center = GetCentroid();
            double x = Vertices[i, 0];
            double y = Vertices[i, 1];
            double z = Vertices[i, 2] + offsetZ;

            if (z < 1) z = 1;

            float xe = (float)((x * d) / z);
            float ye = (float)((y * d) / z);

            projectedPoints[i] = new PointF(xe + offsetX, ye + offsetY);
        }

        for (int i = 0; i < VertexCount; i++)
        {
            for (int j = i + 1; j < VertexCount; j++)
            {
                if (AdjacencyMatrix[i, j] == 1)
                {
                    g.DrawLine(pen, projectedPoints[i], projectedPoints[j]);
                }
            }
        }
    }
    public void Draw(Graphics g, int width, int height)
    {
        int offsetX = width / 2;
        int offsetY = height / 2;
        Pen pen = new Pen(Color.Blue, 1);

        for (int i = 0; i < VertexCount; i++)
        {
            for (int j = i + 1; j < VertexCount; j++)
            {
                if (AdjacencyMatrix[i, j] == 1)
                {
                    g.DrawLine(pen,
                        (float)Vertices[i, 0] + offsetX, (float)Vertices[i, 1] + offsetY,
                        (float)Vertices[j, 0] + offsetX, (float)Vertices[j, 1] + offsetY);
                }
            }
        }
    }
}