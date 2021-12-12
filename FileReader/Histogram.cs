using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PCXDecoder;
using BMPDecoder;
namespace FileReader
{
    public partial class Histogram : Form
    {
        public Histogram(Bitmap image)
        {
            InitializeComponent();
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart2.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart2.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart2.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart3.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart3.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart3.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart5.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart5.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart5.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart4.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart4.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart4.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            image1 = image;
            pictureBox1.Image = image;
            pictureBox2.Image = image2Graylevel(image);
            RGBHistogram(image);
            for (int i = 0; i < 256; i++)
            {
                chart1.Series[0].Points.AddXY(i, Convert.ToString(countR[i]));
                chart1.Series[1].Points.AddXY(i, Convert.ToString(countG[i]));
                chart1.Series[2].Points.AddXY(i, Convert.ToString(countB[i]));
            }
            GrayHistogram(image2Graylevel(image));
            GrayhistogramEqualization(image2Graylevel(image));
            for (int i = 0; i < 256; i++)
            {
                chart4.Series[0].Points.AddXY(i, Convert.ToString(countGray[i]));
                chart4.Series[1].Points.AddXY(i, Convert.ToString(CDFGray[i]));
                chart2.Series[0].Points.AddXY(i, Convert.ToString(EcountGray[i]));
                chart2.Series[1].Points.AddXY(i, Convert.ToString(EqCDFGray[i]));
            }
        }
        //======================================================================
        public Bitmap ImageResize(Bitmap image, double Wzoom, double Hzoom)
        {
            double Width = Math.Round(Wzoom * (double)image.Width);
            double Height = Math.Round(Hzoom * (double)image.Height);
            Bitmap zoomimage = new Bitmap((int)Width, (int)Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int i = (int)Math.Round(x * (1 / Wzoom));
                    int j = (int)Math.Round(y * (1 / Hzoom));
                    if (i > (image.Width - 1))
                    {
                        i = image.Width - 1;
                    }
                    if (j > (image.Height - 1))
                    {
                        j = image.Height - 1;
                    }
                    zoomimage.SetPixel(x, y, image.GetPixel(i, j));
                }
            }
            return zoomimage;
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

        int[] countR, countG, countB, countGray, EcountGray, ScountGray;
        int[] CDFGray,EqCDFGray,SpCDFGray;
        byte[] EqualGray,SpecGray,Gray1,Gray2;
        PReadHeader PCX_header = new PReadHeader();
        PDecodeFile PCX_decode = new PDecodeFile();
        BReadHeader BMP_header = new BReadHeader();
        BDecodeFile BMP_decode = new BDecodeFile();
        Bitmap image1;
        Bitmap image2;
        Bitmap image3;
        double width1, height1, width2, height2;
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            openFileDialog1.Filter = "Image Files (PCX,BMP)|*.PCX;*.BMP;";
            string filename = openFileDialog1.FileName;
            byte[] imagefile = File.ReadAllBytes(filename);
            if (filename.Contains(".pcx"))
            {
                PCX_header.readheader(imagefile);
                PCX_decode.decoPixel(imagefile);
                image2 = PCX_decode.decode_image;
            }
            else if (filename.Contains(".bmp"))
            {
                BMP_header.readheader(imagefile);
                BMP_decode.decoPixel(imagefile);
                BReadPalette BMP_palette = new BReadPalette();
                BMP_palette.readpalette(imagefile);
                image2 = BMP_decode.decode_image;
            }
            width2 = image2.Width;
            height2 = image2.Height;
            width1 = image1.Width;
            height1 = image1.Height;
            double Wzoom, Hzoom;
            if (width1 != width2) { Wzoom = width1 / width2; }
            else { Wzoom = 1.0; }
            if (height1 != height2) { Hzoom = height1 / height2; }
            else { Hzoom = 1.0; }
            image2 = ImageResize(image2, Wzoom, Hzoom);
            pictureBox5.Image = image2Graylevel(image2);
            chart3.Series[0].Points.Clear();
            chart3.Series[1].Points.Clear();
            GrayhistogramSpecification(image1, image2Graylevel(image2));
        }
        //==================RGB=============================
        public void RGBHistogram(Bitmap image)
        {
            countR = new int[256];
            countG = new int[256];
            countB = new int[256];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color RGB = image.GetPixel(x, y);
                    countR[RGB.R]++;
                    countG[RGB.G]++;
                    countB[RGB.B]++;
                }
            }
        }
        //==================Gray============================
        public int[] GrayHistogram(Bitmap image)
        {
            countGray = new int[256];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color gray = image.GetPixel(x, y);
                    countGray[gray.R]++;
                }
            }
            return countGray;
        }
        //==================Equal===========================
        public void GrayhistogramEqualization(Bitmap image)
        {
            GrayHistogram(image);
            EqualGray = new byte[256];
            CDFGray = new int[256];
            EqCDFGray = new int[256];
            Bitmap Eimage = new Bitmap(image.Width, image.Height);
            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    CDFGray[i] = CDFGray[i - 1] + countGray[i];
                }
                else
                {
                    CDFGray[0] = CDFGray[0];
                }
                EqualGray[i] = (byte)(255 * CDFGray[i] / (image.Width * image.Height) + 0.5);
            }
            EcountGray = new int[256];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color gray = image.GetPixel(x, y);
                    Eimage.SetPixel(x, y, Color.FromArgb(EqualGray[gray.R], EqualGray[gray.R], EqualGray[gray.R]));
                    EcountGray[Eimage.GetPixel(x, y).R]++;
                }
            }
            pictureBox3.Image = Eimage;

            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    EqCDFGray[i] = EqCDFGray[i - 1] + EcountGray[i];
                }
                else
                {
                    EqCDFGray[0] = EqCDFGray[0];
                }
            }
        }
        //==================Specification===================
        public void GrayhistogramSpecification(Bitmap image1, Bitmap image2) 
        {
            ScountGray = new int[256];
            SpecGray = new byte[256];
            SpCDFGray = new int[256];
            image3 = new Bitmap(image1.Width, image1.Height);
            int[] his1 =  GrayHistogram(image1);
            int[] his2 = GrayHistogram(image2);
            double[] CDF1 = new double[256];
            double[] CDF2 = new double[256];
            Gray1 = new byte[256];
            Gray2 = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    CDF1[i] = CDF1[i - 1] + his1[i];
                    CDF2[i] = CDF2[i - 1] + his2[i];
                }
                else
                {
                    CDF1[0] = CDF1[0];
                    CDF2[0] = CDF2[0];
                }
                Gray1[i] = (byte)(255 * CDF1[i] / (image1.Width * image1.Height) + 0.5);
                Gray2[i] = (byte)(255 * CDF2[i] / (image2.Width * image2.Height) + 0.5);
            }
            double diff1,diff2;
            byte k = 0;
            for (int i = 0; i < 256; i++)
            {
                diff2 = 1;
                for (int j = k; j < 256; j++)
                {
                    //找到Gray1 Gray2中最相似的位置  
                    diff1 = Math.Abs(Gray1[i] - Gray2[j]);
                    if ((diff1 - diff2) < 1.0E-08)
                    { 
                        diff2 = diff1;
                        k = (byte)j;
                    }
                    else
                    {
                        k  = (byte)Math.Abs(j - 1);
                        break;
                    }
                }
                if (k  == 255)
                {
                    for (int l = i; l < 256; l++)
                    {
                        SpecGray[l] = k;
                    }
                    break;
                }
                SpecGray[i] = k;
            }
            for (int y = 0; y < image1.Height; y++)
            {
                for (int x = 0; x < image1.Width; x++)
                {
                    Color gray = image1.GetPixel(x, y);
                    image3.SetPixel(x, y, Color.FromArgb(SpecGray[gray.R], SpecGray[gray.R], SpecGray[gray.R]));
                    ScountGray[image3.GetPixel(x, y).R]++;
                }
            }
            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    SpCDFGray[i] = SpCDFGray[i - 1] + ScountGray[i];
                }
                else
                {
                    SpCDFGray[0] = SpCDFGray[0];
                }
            }
            chart5.Series[0].Points.Clear();
            chart5.Series[1].Points.Clear();
            for (int i = 0; i < 256; i++)
            {
                chart3.Series[0].Points.AddXY(i, Convert.ToString(his2[i]));
                chart3.Series[1].Points.AddXY(i, Convert.ToString(CDF2[i]));

                chart5.Series[0].Points.AddXY(i, Convert.ToString(ScountGray[i]));
                chart5.Series[1].Points.AddXY(i, Convert.ToString(SpCDFGray[i]));
            }
            pictureBox6.Image = image3;
        }

        private void Histogram_Load(object sender, EventArgs e)
        {

        }
        private void chart1_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

    }
}
/*public void RGBhistogramEqualization(Bitmap image)
        {
            RGBHistogram(image);
            EcountR = new int[256];
            EcountG = new int[256];
            EcountB = new int[256];
            EqualR = new byte[256];
            EqualG = new byte[256];
            EqualB = new byte[256];
            CDFR = new int[256];
            CDFG = new int[256];
            CDFB = new int[256];
            newCDFR = new int[256];
            newCDFG = new int[256];
            newCDFB = new int[256];
            Bitmap Eimage = new Bitmap(image.Width, image.Height);
            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    CDFR[i] = CDFR[i - 1] + countR[i];
                    CDFG[i] = CDFG[i - 1] + countG[i];
                    CDFB[i] = CDFB[i - 1] + countB[i];
                }
                else
                {
                    CDFR[0] = CDFR[0];
                    CDFG[0] = CDFG[0];
                    CDFB[0] = CDFB[0];
                }
                EqualR[i] = (byte)(255 * CDFR[i] / (image.Width * image.Height) + 0.5);
                EqualG[i] = (byte)(255 * CDFG[i] / (image.Width * image.Height) + 0.5);
                EqualB[i] = (byte)(255 * CDFB[i] / (image.Width * image.Height) + 0.5);
            }
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color RGB = image.GetPixel(x, y);
                    Eimage.SetPixel(x, y, Color.FromArgb(EqualR[RGB.R], EqualG[RGB.G], EqualB[RGB.B]));
                    EcountR[Eimage.GetPixel(x, y).R]++;
                    EcountG[Eimage.GetPixel(x, y).G]++;
                    EcountB[Eimage.GetPixel(x, y).B]++;
                }
            }
            pictureBox4.Image = Eimage;
            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    newCDFR[i] = newCDFR[i - 1] + EcountR[i];
                    newCDFG[i] = newCDFG[i - 1] + EcountG[i];
                    newCDFB[i] = newCDFB[i - 1] + EcountB[i];
                }
                else
                {
                    newCDFR[0] = newCDFR[0];
                    newCDFG[0] = newCDFG[0];
                    newCDFB[0] = newCDFB[0];
                }
            }
        }*/