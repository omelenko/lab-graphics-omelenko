using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace WinFormsApp1
{
    public class Polyhedron
    {
        public int VertexCount;
        public double[,] Vertices;
        public int[,] AdjacencyMatrix;

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
        }

        private double[] GetCentroid()
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
}
