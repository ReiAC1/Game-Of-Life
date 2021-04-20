using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class OptionsForm : Form
    {
        public static int CellWidth = -1;
        public static int CellHeight = -1;
        public static int Interval = -1;


        public OptionsForm(int width, int height, int interval)
        {
            InitializeComponent();

            numericUpDown3.Maximum = decimal.MaxValue;

            numericUpDown1.Value = height;
            numericUpDown2.Value = width;
            numericUpDown3.Value = interval;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CellWidth = (int)numericUpDown2.Value;
            CellHeight = (int)numericUpDown1.Value;
            Interval = (int)numericUpDown3.Value;

            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
