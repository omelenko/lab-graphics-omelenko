namespace WinFormsApp1;

public class PolyhedronV2 : PolyhedronV1
{

    // множення вектора на матрицю 4x4
    private double[] MultiplyMatrix(double[] v, double[,] m)
    {
        double[] res = new double[3];
        double w = v[0] * m[0, 3] + v[1] * m[1, 3] + v[2] * m[2, 3] + 1 * m[3, 3];

        res[0] = (v[0] * m[0, 0] + v[1] * m[1, 0] + v[2] * m[2, 0] + 1 * m[3, 0]) / w;
        res[1] = (v[0] * m[0, 1] + v[1] * m[1, 1] + v[2] * m[2, 1] + 1 * m[3, 1]) / w;
        res[2] = (v[0] * m[0, 2] + v[1] * m[1, 2] + v[2] * m[2, 2] + 1 * m[3, 2]) / w;
        return res;
    }

    public void ApplyTransformation(double[,] matrix)
    {
        for (int i = 0; i < VertexCount; i++)
        {
            double[] v = { Vertices[i, 0], Vertices[i, 1], Vertices[i, 2] };
            double[] transformed = MultiplyMatrix(v, matrix);
            Vertices[i, 0] = transformed[0];
            Vertices[i, 1] = transformed[1];
            Vertices[i, 2] = transformed[2];
        }
    }
    public new void Scale(double s)
    {
        double[] c = GetCentroid();
        // зміщення в центр -> масштаб -> повернення
        double[,] matrix = {
            { s, 0, 0, 0 },
            { 0, s, 0, 0 },
            { 0, 0, s, 0 },
            { c[0]*(1-s), c[1]*(1-s), c[2]*(1-s), 1 }
        };
        ApplyTransformation(matrix);
    }

    public new void Rotate(double angleDeg, string axis)
    {
        double rad = angleDeg * Math.PI / 180.0;
        double cos = Math.Cos(rad);
        double sin = Math.Sin(rad);
        double[] c = GetCentroid();

        // поворот навколо центру
        double[,] rotate = new double[4, 4];
        for (int i = 0; i < 4; i++) rotate[i, i] = 1;

        if (axis == "X") { rotate[1, 1] = cos; rotate[1, 2] = sin; rotate[2, 1] = -sin; rotate[2, 2] = cos; }
        else if (axis == "Y") { rotate[0, 0] = cos; rotate[0, 2] = -sin; rotate[2, 0] = sin; rotate[2, 2] = cos; }
        else if (axis == "Z") { rotate[0, 0] = cos; rotate[0, 1] = sin; rotate[1, 0] = -sin; rotate[1, 1] = cos; }

        // розрахунок крутки навколо центру
        rotate[3, 0] = -c[0] * rotate[0, 0] - c[1] * rotate[1, 0] - c[2] * rotate[2, 0] + c[0];
        rotate[3, 1] = -c[0] * rotate[0, 1] - c[1] * rotate[1, 1] - c[2] * rotate[2, 1] + c[1];
        rotate[3, 2] = -c[0] * rotate[0, 2] - c[1] * rotate[1, 2] - c[2] * rotate[2, 2] + c[2];

        ApplyTransformation(rotate);
    }
}
