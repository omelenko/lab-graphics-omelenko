using Microsoft.VisualBasic.Logging;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
    // task 4
    private Polyhedron polyhedron = new Polyhedron();
    public Form1()
    {
        InitializeComponent();
        genericFigure = new GenericFigure();
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

    private float GetMaxY()
    {
        float maxY = float.MinValue;
        for (float x = a; x <= b; x += h)
        {
            maxY = Math.Max(maxY, Function(x));
        }
        return maxY;
    }

    private float GetMinY()
    {
        float minY = float.MaxValue;
        for (float x = a; x <= b; x += h)
        {
            minY = Math.Min(minY, Function(x));
        }
        return minY;
    }

    private void button2_Click(object sender, EventArgs e)
    {
        scaleX++;
        scaleY++;
        button1_Click(sender, e);
    }

    private void button3_Click(object sender, EventArgs e)
    {
        if (scaleX > 1 && scaleY > 1)
        {
            scaleX--;
            scaleY--;
        }
        button1_Click(sender, e);
    }

    private void button4_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                genericFigure.InitializeFromFile(openFileDialog.FileName);
                pictureBox2.Invalidate();
            }
        }
    }

    private void button5_Click(object sender, EventArgs e)
    {
        pictureBox2.Invalidate();
    }

    private void button6_Click(object sender, EventArgs e)
    {
        genericFigure.Clear();
        pictureBox2.Invalidate();
    }

    private void pictureBox_Paint(object sender, PaintEventArgs e)
    {
        genericFigure.Draw(e.Graphics);
    }

    private void button7_Click(object sender, EventArgs e)
    {
        genericFigure.Move(-cordsStep, 0);
        pictureBox2.Invalidate();
    }

    private void button8_Click(object sender, EventArgs e)
    {
        genericFigure.Move(0, -cordsStep);
        pictureBox2.Invalidate();
    }

    private void button9_Click(object sender, EventArgs e)
    {
        genericFigure.Move(0, cordsStep);
        pictureBox2.Invalidate();
    }

    private void button10_Click(object sender, EventArgs e)
    {
        genericFigure.Move(cordsStep, 0);
        pictureBox2.Invalidate();
    }

    private void button12_Click(object sender, EventArgs e)
    {
        genericFigure.Scale += 1.0f;
        pictureBox2.Invalidate();
    }

    private void button11_Click(object sender, EventArgs e)
    {
        genericFigure.Scale -= 1.0f;
        pictureBox2.Invalidate();
    }

    private void button14_Click(object sender, EventArgs e)
    {
        genericFigure.Rotate(-angleStep);
        pictureBox2.Invalidate();
    }

    private void button13_Click(object sender, EventArgs e)
    {
        genericFigure.Rotate(angleStep);
        pictureBox2.Invalidate();
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

        pictureBox3.Invalidate();
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

        pictureBox4.Invalidate();
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

    private void textBox7_TextChanged(object sender, EventArgs e)
    {

    }

    //3.3
    private void button27_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                genericFigure.InitializeFromFile(openFileDialog.FileName);
                pictureBox5.Invalidate();
            }
        }
    }

    private void button26_Click(object sender, EventArgs e)
    {
        pictureBox5.Invalidate();
    }

    private void button25_Click(object sender, EventArgs e)
    {
        genericFigure.Clear();
        pictureBox5.Invalidate();
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
                pictureBox6.Invalidate();
            }
        }
    }

    private void button18_Click(object sender, EventArgs e)
    {
        pictureBox5.Invalidate();
    }

    private void button17_Click(object sender, EventArgs e)
    {
        genericFigure.Clear();
        pictureBox5.Invalidate();
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
                pictureBox7.Invalidate();
            }
        }
    }

    private void button21_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(-angleStep, "X");
        pictureBox7.Invalidate();
    }

    private void button20_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(-angleStep, "X");
        pictureBox7.Invalidate();
    }

    private void button23_Click(object sender, EventArgs e)
    {
        polyhedron.Scale(1.1);
        pictureBox7.Invalidate();
    }

    private void button22_Click(object sender, EventArgs e)
    {
        polyhedron.Scale(0.9);
        pictureBox7.Invalidate();
    }

    private void pictureBox7_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        if (polyhedron.Vertices != null)
            polyhedron.Draw(e.Graphics, pictureBox7.Width, pictureBox7.Height);
    }

    private void button30_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(-angleStep, "Y");
        pictureBox7.Invalidate();
    }

    private void button29_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(angleStep, "X");
        pictureBox7.Invalidate();
    }

    private void button28_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(-angleStep, "X");
        pictureBox7.Invalidate();
    }

    private void button24_Click(object sender, EventArgs e)
    {
        polyhedron.Rotate(angleStep, "Y");
        pictureBox7.Invalidate();
    }

    private void textBox10_TextChanged(object sender, EventArgs e)
    {
        if (float.TryParse(textBox10.Text, out float newAngleStep))
        {
            angleStep = newAngleStep;
        }
    }
}
