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
    public partial class Filter : Form
    {
        Bitmap image;
        public Filter(Bitmap Image)
        {
            InitializeComponent();
            pictureBox1.Image = Image;
            image = Image;
        }
        public Bitmap Outlier(Bitmap image,int size)
        {
            pictureBox1.Image = image;
            Bitmap Outlierimage = new Bitmap(image.Width, image.Height);
            //Color[] mask = new Color[9];
            if (size != 0)
            {
                Color[] mask = new Color[size * size];
                int tmp = size / 2;
                int index = 0;
                for (int y = size / 2; y < image.Height - size / 2; y++)
                {
                    for (int x = size / 2; x < image.Width - size / 2; x++)
                    {
                        for (int j = y - tmp; j <= y + tmp; j++)
                        {
                            for (int i = x - tmp; i <= x + tmp; i++)
                            {
                                if (index >= size * size) { index = 0; }
                                else
                                {
                                    mask[index++] = image.GetPixel(i, j);
                                }
                            }
                        }
                        int pointr = mask[size * size / 2].R;
                        int pointg = mask[size * size / 2].G;
                        int pointb = mask[size * size / 2].B;
                        int dr = 0, dg = 0, db = 0;
                        for(int i =0;i<mask.Length;i++)
                        {
                            dr += mask[i].R;
                            dg += mask[i].G;
                            db += mask[i].B;
                        }
                        dr = (dr - pointr) / (mask.Length - 1);
                        dg = (dg - pointg) / (mask.Length - 1);
                        db = (db - pointb) / (mask.Length - 1);
                        if (Math.Abs(pointr - dr) > 50)
                        {
                            pointr = dr;
                        }
                        if (Math.Abs(pointg - dg) > 50)
                        {
                            pointg = dg;
                        }
                        if (Math.Abs(pointb - db) > 50)
                        {
                            pointb = db;
                        }
                        Outlierimage.SetPixel(x, y, Color.FromArgb(pointr, pointg, pointb));
                    }
                }
            }         
            return Outlierimage;
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
        public Bitmap SquareMedian(Bitmap image,int size)
        {
            Bitmap grayimage =  image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap Medianimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] mask = new int[size * size];
                int tmp = size / 2;
                int index = 0;
                for (int y = size / 2; y < grayimage.Height - size / 2; y++)
                {
                    for (int x = size / 2; x < grayimage.Width - size / 2; x++)
                    {
                        for (int j = y - tmp; j <= y + tmp; j++)
                        {
                            for (int i = x - tmp; i <= x + tmp; i++)
                            {
                                if (index >= size * size) { index = 0; }
                                else
                                {
                                    mask[index++] = grayimage.GetPixel(i, j).R;
                                }
                            }
                        }
                        Array.Sort(mask);
                        int median = mask[size * size / 2];
                        Medianimage.SetPixel(x, y, Color.FromArgb(median, median, median));

                    }
                }
            }
            return Medianimage;
            
        }

        public Bitmap CrossMedian(Bitmap image, int size)
        {
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap Medianimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] mask = new int[(size-1)*4+1];
                int index = 0;
                for (int y = (size -1); y < (grayimage.Height - size -1); y++)
                {
                    for (int x = (size -1); x < (grayimage.Width - size -1); x++)
                    {
                        if (index >= (size - 1) * 4 ) { index = 0; }
                        else
                        {
                            for (int i = (x - (size - 1)); i < x; i++)
                            {
                                mask[index++] = grayimage.GetPixel(i, y).R;
                            }
                            for (int i = x; i < (x + size - 1); i++)
                            {
                                mask[index++] = grayimage.GetPixel(i, y).R;
                            }
                            for (int i = (y - (size - 1)); i < y; i++)
                            {
                                mask[index++] = grayimage.GetPixel(x, i).R;
                            }
                            for (int i = y; i < (y + size - 1); i++)
                            {
                                mask[index++] = grayimage.GetPixel(x, i).R;
                            }
                            mask[index] = grayimage.GetPixel(x, y).R; // n
                        }
                        Array.Sort(mask);

                        int median = mask[mask.Length/2];
                        Medianimage.SetPixel(x, y, Color.FromArgb(median, median, median));

                    }
                }
            }
            return Medianimage;

        }

        public Bitmap PseudoMedian(Bitmap image, int size)
        {
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap Medianimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] xmask = new int[(size - 1) * 2 + 1];
                int[] ymask = new int[(size - 1) * 2 + 1];
                int xindex = 0, yindex = 0;
                for (int y = (size - 1); y < (grayimage.Height - size - 1); y++)
                {
                    for (int x = (size - 1); x < (grayimage.Width - size - 1); x++)
                    {
                        if((yindex >= (size - 1) * 2)&& (xindex >= (size - 1) * 2)) { yindex = 0; xindex = 0; }
                        else
                        {
                            //PMED= 0.5*max(MAXMIN_x,MAXMIN_y)+0.5*min(MINMAX_x,MINMAX_y)
                            //==================橫=====================
                            for (int i = (x - (size - 1)); i < (x + size); i++)
                            {
                                xmask[xindex++] = grayimage.GetPixel(i, y).R;
                            }
                            //==================直======================
                            for (int i = (y - (size - 1)); i < (y + size); i++)
                            {
                                ymask[yindex++] = grayimage.GetPixel(x, i).R;
                            }
                        }
                        //==================MAXMIN========================
                        int MAXMIN_x = 0, MAXMIN_y = 0;
                        for (int j = 0; j < xmask.Length - 1; j++)
                        {
                            MAXMIN_x = Math.Max(MAXMIN_x, Math.Min(xmask[j], xmask[j + 1]));
                            MAXMIN_y = Math.Max(MAXMIN_y, Math.Min(ymask[j], ymask[j + 1]));
                        }
                        //==================MINMAX========================
                        int MINMAX_x = 999, MINMAX_y = 999;
                        for (int j = 0; j < xmask.Length - 1; j++)
                        {
                            MINMAX_x = Math.Min(MINMAX_x, Math.Max(xmask[j], xmask[j + 1]));
                            MINMAX_y = Math.Min(MINMAX_y, Math.Max(ymask[j], ymask[j + 1]));
                        }
                        int PMED =(int)(0.5 * Math.Max(MAXMIN_x, MAXMIN_y) + 0.5 * Math.Min(MINMAX_x, MINMAX_y));
                        Medianimage.SetPixel(x, y, Color.FromArgb(PMED, PMED, PMED));

                    }
                }
            }
            return Medianimage;

        }
        public Bitmap Highpass(Bitmap image, int size)
        {
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap Medianimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] mask = new int[size * size];
                int tmp = size / 2;
                int index = 0;
                for (int y = size / 2; y < grayimage.Height - size / 2; y++)
                {
                    for (int x = size / 2; x < grayimage.Width - size / 2; x++)
                    {
                        for (int j = y - tmp; j <= y + tmp; j++)
                        {
                            for (int i = x - tmp; i <= x + tmp; i++)
                            {
                                if (index >= size * size) { index = 0; }
                                else
                                {
                                    mask[index++] = grayimage.GetPixel(i, j).R;
                                }
                            }
                        }
                        double sum = 0;
                        for(int i =0;i<mask.Length;i++)
                        {
                            sum += mask[i];
                        }
                        double d = (1 - (1 / mask.Length)* (mask[size * size / 2]/255)) * mask.Length;
                        Medianimage.SetPixel(x, y, Color.FromArgb((int)d, (int)d, (int)d));

                    }
                }
            }
            return Medianimage;

        }
        public Bitmap Lowpass(Bitmap image, int size)
        {
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap Medianimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] mask = new int[size * size];
                int tmp = size / 2;
                int index = 0;
                for (int y = size / 2; y < grayimage.Height - size / 2; y++)
                {
                    for (int x = size / 2; x < grayimage.Width - size / 2; x++)
                    {
                        for (int j = y - tmp; j <= y + tmp; j++)
                        {
                            for (int i = x - tmp; i <= x + tmp; i++)
                            {
                                if (index >= size * size) { index = 0; }
                                else
                                {
                                    mask[index++] = grayimage.GetPixel(i, j).R;
                                }
                            }
                        }
                        int sum = 0;
                        for(int i = 0; i<mask.Length;i++)
                        {
                            sum += mask[i];
                        }
                        int g = sum / mask.Length;
                        Medianimage.SetPixel(x, y, Color.FromArgb(g,g,g));

                    }
                }
            }
            return Medianimage;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Outlier(image, Convert.ToInt32(textBox1.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = SquareMedian(image,Convert.ToInt32(textBox1.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = CrossMedian(image, Convert.ToInt32(textBox1.Text));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "1" || textBox1.Text == "0") { pictureBox2.Image = image2Graylevel(image); }
            else { pictureBox2.Image = PseudoMedian(image, Convert.ToInt32(textBox1.Text)); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Highpass(image, Convert.ToInt32(textBox1.Text));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Lowpass(image, Convert.ToInt32(textBox1.Text));
        }
    }
}
