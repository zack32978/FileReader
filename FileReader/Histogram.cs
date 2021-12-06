using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileReader
{
    public partial class Histogram : Form
    {
        public Histogram(Bitmap image)
        {
            InitializeComponent();
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            /*chart1.ChartAreas["ChartArea1"].BorderWidth = 2;
            chart1.Series[0]["PointWidth"] = "2";*/
            chart2.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart2.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            //chart2.ChartAreas["ChartArea1"].BorderWidth = 2;
            chart3.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart3.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            //chart3.ChartAreas["ChartArea1"].BorderWidth = 2;
            chart4.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart4.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            //chart4.ChartAreas["ChartArea1"].BorderWidth = 2;
            RGBHistogram(image);
            GrayHistogram(image);
        }
        int[] countR, countG, countB,countgray;

        public void RGBHistogram(Bitmap image)
        {
            countR = new int[256];
            countG = new int[256];
            countB = new int[256];
            for (int y = 0; y < image.Height; y++) 
            {
                for(int x =0;x<image.Width; x++)
                {
                    Color RGB = image.GetPixel(x, y);
                    countR[RGB.R] ++;
                    countG[RGB.G] ++;
                    countB[RGB.B] ++;
                }
            }
            for (int i = 0; i < 256; i++) 
            {
                chart1.Series[0].Points.AddXY(i, Convert.ToString(countR[i]));
                chart2.Series[0].Points.AddXY(i, Convert.ToString(countG[i]));
                chart3.Series[0].Points.AddXY(i, Convert.ToString(countB[i]));
            }
            
        }
        public Bitmap image2Graylevel(Bitmap image)
        {
            int Width = image.Width;
            int Height = image.Height;
            Bitmap Grayimage = new Bitmap(Width, Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int gray = ((image.GetPixel(x, y).R + image.GetPixel(x, y).G + image.GetPixel(x, y).B) / 3);
                    Color color = Color.FromArgb(gray, gray, gray);
                    Grayimage.SetPixel(x, y, color);
                }
            }
            return Grayimage;
        }
        public void GrayHistogram(Bitmap image)
        {
            Bitmap grayimage = image2Graylevel(image);
            countgray = new int[256];
            for (int y = 0; y < grayimage.Height; y++)
            {
                for (int x = 0; x < grayimage.Width; x++)
                {
                    Color gray = grayimage.GetPixel(x, y);
                    countgray[gray.R]++;
                }
            }
            for (int i = 0; i < 256; i++)
            {
                chart4.Series[0].Points.AddXY(i, Convert.ToString(countgray[i]));
            }
        }
        private void Histogram_Load(object sender, EventArgs e)
        {

        }
    }
}
