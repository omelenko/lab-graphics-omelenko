using System.Drawing.Drawing2D;
using System.Reflection;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    // task 1.1
    private float a;
    private float b;
    private float h;
    private int scaleX = 1;
    private int scaleY = 1;
    // task 1.2
    private float angleStep = 10;
    private int cordsStep = 10;
    private GenericFigure genericFigure;
    // task 3.1
    private PointF A = new Point(0, 0);
    private PointF Centre = new Point(0, 0);
    private PointF lineStart = new Point(0, 0);
    private PointF lineEnd = new Point(0, 0);
    private PointF Result;
    // task 4-6
    private PolyhedronV1 polyhedron = new PolyhedronV1();
    // task 6
    private double d = 200;
    // task 7.1
    private float[] topHorizon;
    private float[] bottomHorizon;
    private int stepX = 10;
    private int stepY = 10;
    private float pointA = 10.0f;
    private float pointB = 10.0f;
    private float offsetX = 0;
    private float offsetY = 0;
    public Form1()
    {
        InitializeComponent();
        genericFigure = new GenericFigure();
    }
    private void InvalidateAll()
    {
        pictureBox1.Invalidate();
        pictureBox2.Invalidate();
        pictureBox3.Invalidate();
        pictureBox4.Invalidate();
        pictureBox5.Invalidate();
        pictureBox6.Invalidate();
        pictureBox7.Invalidate();
        pictureBox8.Invalidate();
    }
    private void clearButton_Click(object sender, EventArgs e)
    {
        genericFigure.Clear();
        InvalidateAll();
    }
    private void button1_Click(object sender, EventArgs e)
    {
        try
        {
            a = float.Parse(textBox1.Text);
            b = float.Parse(textBox2.Text);
            h = float.Parse(textBox3.Text);
            DrawGraph();
        }
        catch (Exception ex)
        {
            MessageBox.Show("помилка: " + ex.Message);
        }
    }
    private void DrawGraph()
    {
        int width = pictureBox1.Width;
        int height = pictureBox1.Height;

        Bitmap bmp = new Bitmap(width, height);
        Graphics g = Graphics.FromImage(bmp);
        g.Clear(Color.White);
        Pen pen = new Pen(Color.Blue, 2);
        Pen pen1 = new Pen(Color.Red, 2);

        g.DrawLine(pen, 0, height / 2, width, height / 2); // ось X
        g.DrawLine(pen, width / 2, 0, width / 2, height); // ось Y


        int centerX = width / 2;
        int centerY = height / 2;

        float prevX = centerX + (a * scaleX);
        float prevY = centerY - (Function(a) * scaleY);
        for (float x = a; x <= b; x += h)
        {
            float y = Function(x);
            float screenX = centerX + (x * scaleX);
            float screenY = centerY - (y * scaleY);

            if (x < screenX) g.DrawLine(pen1, prevX, prevY, screenX, screenY);

            prevX = screenX;
            prevY = screenY;
        }

        pictureBox1.Image = bmp;
    }
    private float Function(float x)
    {
        return x * x;
    }
    private void button4_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                genericFigure.InitializeFromFile(openFileDialog.FileName);
                InvalidateAll();
            }
        }
    }

    private void pictureBox_Paint(object sender, PaintEventArgs e)
    {
        genericFigure.Draw(e.Graphics);
    }

    private void button7_Click(object sender, EventArgs e)
    {
        genericFigure.Move(-cordsStep, 0);
        InvalidateAll();
    }

    private void button8_Click(object sender, EventArgs e)
    {
        genericFigure.Move(0, -cordsStep);
        InvalidateAll();
    }

    private void button9_Click(object sender, EventArgs e)
    {
        genericFigure.Move(0, cordsStep);
        InvalidateAll();
    }

    private void button10_Click(object sender, EventArgs e)
    {
        genericFigure.Move(cordsStep, 0);
        InvalidateAll();
    }

    private void scaleButtonPlus_Click(object sender, EventArgs e)
    {
        scaleX++;
        scaleY++;
        genericFigure.Scale += 1.0f;
        InvalidateAll();
    }

    private void scaleButtonMinus_Click(object sender, EventArgs e)
    {
        if (scaleX > 1 && scaleY > 1)
        {
            scaleX--;
            scaleY--;
        }
        if (genericFigure.Scale > 1.0f)
        {
            genericFigure.Scale -= 1.0f;
        }
        InvalidateAll();
    }

    private void button14_Click(object sender, EventArgs e)
    {
        genericFigure.Rotate(-angleStep);
        InvalidateAll();
    }

    private void button13_Click(object sender, EventArgs e)
    {
        genericFigure.Rotate(angleStep);
        InvalidateAll();
    }

    private void textBox4_TextChanged(object sender, EventArgs e)
    {
        if (float.TryParse(textBox4.Text.Split(';')[0], out float newAngleStep))
        {
            angleStep = newAngleStep;
        }
        if (int.TryParse(textBox4.Text.Split(';')[1], out int newCordsStep))
        {
            cordsStep = newCordsStep;
        }
    }

    private void button15_Click(object sender, EventArgs e)
    {
        if (float.TryParse(textBox5.Text.Split(';')[0], out float Ax) &&
            float.TryParse(textBox5.Text.Split(';')[1], out float Ay))
        {
            A = new PointF(Ax, Ay);
        }
        if (float.TryParse(textBox6.Text.Split(';')[0], out float Cx) &&
            float.TryParse(textBox6.Text.Split(';')[1], out float Cy))
        {
            Centre = new PointF(Cx, Cy);
        }

        Result = new PointF(2 * Centre.X - A.X, 2 * Centre.X - A.Y);

        InvalidateAll();
    }

    private void pictureBox3_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TranslateTransform(pictureBox3.Width / 2, pictureBox3.Height / 2);

        Pen dashPen = new Pen(Color.Gray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };

        g.DrawLine(dashPen, A, Result);

        DrawPoint(g, Brushes.Blue, A, "A");
        DrawPoint(g, Brushes.Red, Centre, "C");
        DrawPoint(g, Brushes.Green, Result, "A'");
    }

    private void DrawPoint(Graphics g, Brush brush, PointF pt, string label)
    {
        float radius = 4;
        g.FillEllipse(brush, pt.X - radius, pt.Y - radius, radius * 2, radius * 2);
        g.DrawString($"{label} ({pt.X}; {pt.Y})", this.Font, brush, pt.X + 5, pt.Y + 5);
    }

    private void button16_Click(object sender, EventArgs e)
    {
        if (float.TryParse(textBox7.Text.Split(',')[0].Split(';')[0], out float lsx) &&
            float.TryParse(textBox7.Text.Split(',')[0].Split(';')[1], out float lsy) &&
            float.TryParse(textBox7.Text.Split(',')[1].Split(';')[0], out float lex) &&
            float.TryParse(textBox7.Text.Split(',')[1].Split(';')[1], out float ley))
        {
            lineStart = new PointF(lsx, lsy);
            lineEnd = new PointF(lex, ley);
        }
        if (float.TryParse(textBox6.Text.Split(';')[0], out float Ax) &&
            float.TryParse(textBox6.Text.Split(';')[1], out float Ay))
        {
            A = new PointF(Ax, Ay);
        }
        if (float.TryParse(textBox8.Text.Split(';')[0], out float Cx) &&
            float.TryParse(textBox8.Text.Split(';')[1], out float Cy))
        {
            Centre = new PointF(Cx, Cy);
        }


        // a = y1 - y2, b = x2 - x1, c = x1*y2 - x2*y1
        float a = lineStart.Y - lineEnd.Y;
        float b = lineEnd.X - lineStart.X;
        float c = lineStart.X * lineEnd.Y - lineEnd.X * lineStart.Y;

        float factor = (a * A.X + b * A.Y + c) / (a * a + b * b);

        Result = new PointF(
            A.X - 2 * a * factor,
            A.Y - 2 * b * factor
        );

        InvalidateAll();
    }

    private void pictureBox4_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        Pen axisPen = new Pen(Color.Red, 2);
        g.DrawLine(axisPen, lineStart, lineEnd);

        Pen dashPen = new Pen(Color.Gray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
        if (!Result.IsEmpty)
            g.DrawLine(dashPen, A, Result);

        DrawLabeledPoint(g, Brushes.Blue, A, "A");
        if (!Result.IsEmpty)
            DrawLabeledPoint(g, Brushes.Green, Result, "A'");
    }

    private void DrawLabeledPoint(Graphics g, Brush brush, PointF pt, string name)
    {
        g.FillEllipse(brush, pt.X - 4, pt.Y - 4, 8, 8);
        g.DrawString($"{name} ({Math.Round(pt.X)}, {Math.Round(pt.Y)})",
                     this.Font, brush, pt.X + 10, pt.Y);
    }

    //3.3
    private void button27_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                genericFigure.InitializeFromFile(openFileDialog.FileName);
                InvalidateAll();
            }
        }
    }

    private void pictureBox5_Paint(object sender, PaintEventArgs e)
    {
        genericFigure.Draw(e.Graphics);
        MessageBox.Show(genericFigure.AnalyzePolygon());
    }
    //3.4
    private void button19_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                genericFigure.InitializeFromFile(openFileDialog.FileName);
                InvalidateAll();
            }
        }
    }
    private void pictureBox6_Paint(object sender, PaintEventArgs e)
    {
        genericFigure.BuildConvexHull(genericFigure.ControlPoints);
        genericFigure.Draw(e.Graphics);
    }
    //4
    private void button33_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                polyhedron.InitializeFromFile(openFileDialog.FileName);
                if (float.TryParse(textBox10.Text, out float newAngleStep)) angleStep = newAngleStep;
                else angleStep = 10;
                if (double.TryParse(textBox11.Text, out double d)) this.d = d;
                else this.d = 200;
                InvalidateAll();
            }
        }
    }

    private void button21_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(-angleStep, "X");
        InvalidateAll();
    }

    private void button20_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(-angleStep, "X");
        InvalidateAll();
    }

    private void button23_Click(object sender, EventArgs e)
    {
        polyhedron.Scale(1.1);
        InvalidateAll();
    }

    private void button22_Click(object sender, EventArgs e)
    {
        polyhedron.Scale(0.9);
        InvalidateAll();
    }

    private void pictureBox7_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        if (polyhedron.Vertices != null)
            // 6
            if (checkBox2.Checked)
                polyhedron.DrawPerspective(e.Graphics, pictureBox7.Width, pictureBox7.Height, d);
            // 7
            else if(checkBox3.Checked)
                polyhedron.DrawWithBackFaceCulling(e.Graphics, pictureBox7.Width, pictureBox7.Height, scaleX, scaleY);
            else
                polyhedron.Draw(e.Graphics, pictureBox7.Width, pictureBox7.Height);
    }

    private void button30_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(-angleStep, "Y");
        InvalidateAll();
    }

    private void button29_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(angleStep, "X");
        InvalidateAll();
    }

    private void button28_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(-angleStep, "X");
        InvalidateAll();
    }

    private void button24_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(angleStep, "Y");
        InvalidateAll();
    }
    // 5
    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
        if (checkBox1.Checked)
        {
            checkBox1.Text = "Лаб 5";
            polyhedron = new PolyhedronV2();
        }
        else
        {
            checkBox1.Text = "Лаб 4";
            polyhedron = new PolyhedronV1();
        }
    }
    // 6
    private void checkBox2_CheckedChanged(object sender, EventArgs e)
    {
        if (checkBox2.Checked)
        {
            textBox11.Enabled = true;
            textBox11.Visible = true;
            label11.Enabled = true;
            label11.Visible = true;
        }
        else
        {
            textBox11.Enabled = true;
            textBox11.Visible = true;
            label11.Enabled = true;
            label11.Visible = true;
        }
    }

    // 7.1
    private float CalculateFunction(float x, float y)
    {
        if (comboBox1.SelectedIndex == 0)
        {
            // пік посередині
            float cx = x - pointA / 2;
            float cy = y - pointB / 2;
            double r = Math.Sqrt(cx * cx + cy * cy);

            return (float)(5 * Math.Sin(r) / (r + 1)); // 5sin(r)/r+1
        }
        else
        {
            return (float)(Math.Sin(x) + Math.Cos(y)); // sin(x) + cos(y)
        }
    }
    private PointF Project(float x, float y, float z)
    {
        float x0 = pictureBox8.Width / 2 + offsetX;
        float y0 = pictureBox8.Height / 2 + offsetY; 
        double angle = Math.PI / 4; // 45

        float x_screen = x0 + (x - y * (float)Math.Cos(angle)) * scaleX;
        float y_screen = y0 - (z + y * (float)Math.Sin(angle)) * scaleY;

        return new PointF(x_screen, y_screen);
    }
    private void DrawFloatingHorizon(Graphics g, int width, int height)
    {
        topHorizon = new float[width];
        bottomHorizon = new float[width];
        Pen penTop = new Pen(Color.Blue, 2);
        Pen penBottom = new Pen(Color.Red, 2);

        for (int i = 0; i < width; i++)
        {
            topHorizon[i] = height;
            bottomHorizon[i] = 0;
        }

        for (float y = 0; y <= pointB; y += stepY)
        {
            PointF prevPoint = Point.Empty;

            for (float x = 0; x <= pointA; x += stepX)
            {
                float z = CalculateFunction(x, y);
                PointF screenPt = Project(x, y, z);

                int xIdx = (int)screenPt.X;
                if (xIdx >= 0 && xIdx < width)
                {
                    if (screenPt.Y < topHorizon[xIdx])
                    {
                        if (!prevPoint.IsEmpty) g.DrawLine(penTop, prevPoint, screenPt);
                        topHorizon[xIdx] = screenPt.Y;
                    }
                    else if (screenPt.Y > bottomHorizon[xIdx])
                    {
                        if (!prevPoint.IsEmpty) g.DrawLine(penBottom, prevPoint, screenPt);
                        bottomHorizon[xIdx] = screenPt.Y;
                    }
                }
                prevPoint = screenPt;
            }
        }
    }

    private void button20_Click_1(object sender, EventArgs e)
    {
        pointA = (float)numericUpDown4.Value;
        pointB = (float)numericUpDown3.Value;
        stepX = numericUpDown1.Value > 0 ? (int)numericUpDown1.Value : stepX;
        stepY = numericUpDown2.Value > 0 ? (int)numericUpDown2.Value : stepY;
        InvalidateAll();
    }

    private void pictureBox8_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        DrawFloatingHorizon(e.Graphics, pictureBox8.Width, pictureBox8.Height);
    }
    private void btnUp_Click(object sender, EventArgs e)
    {
        offsetY -= cordsStep;
        InvalidateAll();
    }

    private void btnDown_Click(object sender, EventArgs e)
    {
        offsetY += cordsStep;
        InvalidateAll();
    }

    private void btnLeft_Click(object sender, EventArgs e)
    {
        offsetX -= cordsStep;
        InvalidateAll();
    }

    private void btnRight_Click(object sender, EventArgs e)
    {
        offsetX += cordsStep;
        InvalidateAll();
    }
}
