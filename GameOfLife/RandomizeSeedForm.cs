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
    public partial class RandomizeSeedForm : Form
    {
        public static bool Randomize = false;
        public static int Seed = 0;

        static Random random = new Random();

        public RandomizeSeedForm()
        {
            InitializeComponent();

            numericUpDown1.Maximum = int.MaxValue;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Randomize = true;
            Seed = (int)numericUpDown1.Value;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = random.Next(int.MaxValue);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Randomize = false;
            Close();
        }
    }
}
