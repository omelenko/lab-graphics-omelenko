using System.Runtime.CompilerServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsApp1;

public partial class Form1 : Form
{
    private float a;
    private float b;
    private float h;
    private int scaleX = 1;
    private int scaleY = 1;
    private GenericFigure genericFigure;

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
}
