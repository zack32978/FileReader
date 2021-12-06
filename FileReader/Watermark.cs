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
    public partial class Watermark : Form
    {
        public Watermark(Bitmap image1,Bitmap image2,Bitmap image3)
        {
            InitializeComponent();
            pictureBox1.Image = image1;
            pictureBox2.Image = image2;
            pictureBox3.Image = image3;
        }

        private void Watermark_Load(object sender, EventArgs e)
        {

        }
    }
}
