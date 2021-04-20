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

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[20, 20];

        // boolean values that handle visual or function settings for the universe
        bool running = false;
        bool wrapAound = false;
        bool showNeighborCount = true;
        bool showHud = true;
        bool showGrid = true;

        // amount of cells on
        int cellsOn = 0;

        // current seed and random variable
        int seed = 0;
        Random random = null;

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 20; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = true; // start timer running

            seed = new Random().Next(int.MaxValue);
            random = new Random(seed);

            toolStripStatusLabel3.Text = "Seed: " + seed;
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // Increment generation count
            generations++;

            // frame update our world
            cellsOn = GOL.FrameUpdate(ref universe, wrapAound);

            // invalidate the graphics since we may have altered the grid with the update
            graphicsPanel1.Invalidate();

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            toolStripStatusLabel2.Text = "Alive = " + cellsOn.ToString();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (running)
                NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            Brush textBrush = new SolidBrush(Color.FromArgb(128, 255, 0, 0));

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    if (showGrid)
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    int count = GOL.GetNeighborCount(universe, x, y, wrapAound);

                    if (!universe[x, y] && count != 0 && showNeighborCount)
                        e.Graphics.DrawString(count.ToString(), DefaultFont, textBrush, cellRect.X, cellRect.Y + cellRect.Height - 12);
                }
            }

            Font hudFont = new Font(DefaultFont.FontFamily, 12, FontStyle.Bold);

            // Drawing HUD
            if (showHud)
            {
                string text = $"Generations: {{Width = {universe.GetLength(0)}, Height = {universe.GetLength(1)}}}";
                var size = TextRenderer.MeasureText(text, hudFont);
                int yDraw = e.ClipRectangle.Height - size.Height - 2;

                e.Graphics.DrawString(text, hudFont, textBrush, 2, yDraw);

                text = String.Format("Boundary Type: {0}", wrapAound ? "Torodial" : "Finite");
                size = TextRenderer.MeasureText(text, hudFont);
                yDraw -= 2 + size.Height;

                e.Graphics.DrawString(text, hudFont, textBrush, 2, yDraw);

                text = $"Cell Count: {cellsOn}";
                size = TextRenderer.MeasureText(text, hudFont);
                yDraw -= 2 + size.Height;

                e.Graphics.DrawString(text, hudFont, textBrush, 2, yDraw);

                text = $"Generations: {generations}";
                size = TextRenderer.MeasureText(text, hudFont);
                yDraw -= 2 + size.Height;

                e.Graphics.DrawString(text, hudFont, textBrush, 2, yDraw);
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
            textBrush.Dispose();
            hudFont.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                cellsOn += universe[x, y] ? 1 : -1;

                toolStripStatusLabel2.Text = "Alive = " + cellsOn.ToString();

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showHud = !showHud;

            hUDToolStripMenuItem.Checked = showHud;

            graphicsPanel1.Invalidate();
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showNeighborCount = !showNeighborCount;

            neighborCountToolStripMenuItem.Checked = showNeighborCount;

            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showGrid = !showGrid;

            gridToolStripMenuItem.Checked = showGrid;

            graphicsPanel1.Invalidate();
        }

        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wrapAound = false;

            finiteToolStripMenuItem.Checked = true;
            torodialToolStripMenuItem.Checked = false;

            graphicsPanel1.Invalidate();
        }

        private void torodialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wrapAound = true;

            finiteToolStripMenuItem.Checked = false;
            torodialToolStripMenuItem.Checked = true;

            graphicsPanel1.Invalidate();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // set all cells to false
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    universe[i, j] = false;
                }
            }

            // reset variables
            generations = 0;
            cellsOn = 0;
            running = false;
            startToolStripMenuItem.Enabled = true;
            nexToolStripMenuItem.Enabled = true;
            pauseToolStripMenuItem.Enabled = false;

            cutToolStripButton.Enabled = true;
            pasteToolStripButton.Enabled = true;
            copyToolStripButton.Enabled = false;

            // redraw
            graphicsPanel1.Invalidate();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // check for comment, if true go to next line
                    if (row.Trim()[0] == '!')
                        continue;

                    // set max width if needed and increment height
                    if (row.Length > maxWidth)
                        maxWidth = row.Length;

                    maxHeight++;
                }

                universe = new bool[maxWidth, maxHeight];

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                int y = 0;

                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // check for comment, if true go to next line
                    if (row.Trim()[0] == '!')
                        continue;

                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        universe[xPos, y] = row[xPos] == 'O';
                    }

                    y++;
                }

                // Close the file.
                reader.Close();

                cellsOn = GOL.GetAliveCells(universe);
                running = false;
                startToolStripMenuItem.Enabled = true;
                nexToolStripMenuItem.Enabled = true;
                pauseToolStripMenuItem.Enabled = false;

                cutToolStripButton.Enabled = true;
                pasteToolStripButton.Enabled = true;
                copyToolStripButton.Enabled = false;

                toolStripStatusLabel2.Text = "Alive = " + cellsOn.ToString();

                graphicsPanel1.Invalidate();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!Cell file created by Johannes Cronje's game of life application");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // set O or . depending if the grid cell is on or off
                        currentRow += universe[x, y] ? 'O' : '.';
                    }

                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                int y = 0;

                // Iterate through the file breaking at the end of file or when the y value is too large for the universe
                while (!reader.EndOfStream && y < universe.GetLength(1))
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // check for comment, if true go to next line
                    if (row.Trim()[0] == '!')
                        continue;

                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        // if we exceed the universe bounds, break
                        if (xPos >= universe.GetLength(0))
                            break;

                        universe[xPos, y] = row[xPos] == 'O';
                    }

                    y++;
                }

                // Close the file.
                reader.Close();

                cellsOn = GOL.GetAliveCells(universe);
                running = false;
                startToolStripMenuItem.Enabled = true;
                nexToolStripMenuItem.Enabled = true;
                pauseToolStripMenuItem.Enabled = false;

                cutToolStripButton.Enabled = true;
                pasteToolStripButton.Enabled = true;
                copyToolStripButton.Enabled = false;

                toolStripStatusLabel2.Text = "Alive = " + cellsOn.ToString();

                graphicsPanel1.Invalidate();


            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            running = true;
            startToolStripMenuItem.Enabled = false;
            nexToolStripMenuItem.Enabled = false;
            pauseToolStripMenuItem.Enabled = true;

            cutToolStripButton.Enabled = false;
            pasteToolStripButton.Enabled = false;
            copyToolStripButton.Enabled = true;
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            running = false;
            startToolStripMenuItem.Enabled = true;
            nexToolStripMenuItem.Enabled = true;
            pauseToolStripMenuItem.Enabled = false;

            cutToolStripButton.Enabled = true;
            pasteToolStripButton.Enabled = true;
            copyToolStripButton.Enabled = false;
        }

        private void nexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void toToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextForm.Generation = generations + 1;
            var form = new NextForm();

            form.FormClosed += (object s, FormClosedEventArgs f) =>
            {
                for (int i = 0; i <= NextForm.Generation; i++)
                {
                    NextGeneration();
                }
            };

            form.ShowDialog();
        }

        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create random based off current seed
            random = new Random(seed);

            // randomize grid and set cells on
            cellsOn = GOL.RandomizeGrid(ref universe, random);

            // update toolstrip and graphics
            toolStripStatusLabel2.Text = "Alive = " + cellsOn.ToString();
            toolStripStatusLabel3.Text = "Seed = " + seed;

            graphicsPanel1.Invalidate();
        }

        private void fromCurrentTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create random based on current time
            random = new Random();

            // randomize grid and set cells on
            cellsOn = GOL.RandomizeGrid(ref universe, random);

            // update toolstrip and graphics
            toolStripStatusLabel2.Text = "Alive = " + cellsOn.ToString();
            toolStripStatusLabel3.Text = "Seed = " + seed;

            graphicsPanel1.Invalidate();
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var rand = new RandomizeSeedForm();
            rand.FormClosed += (object s, FormClosedEventArgs f) =>
            {
                if (!RandomizeSeedForm.Randomize)
                    return;

                seed = RandomizeSeedForm.Seed;
                random = new Random(seed);

                // randomize grid and set cells on
                cellsOn = GOL.RandomizeGrid(ref universe, random);

                // update toolstrip and graphics
                toolStripStatusLabel2.Text = "Alive = " + cellsOn.ToString();
                toolStripStatusLabel3.Text = "Seed = " + seed;

                graphicsPanel1.Invalidate();
            };

            rand.ShowDialog();
        }

        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                graphicsPanel1.BackColor = colorDialog.Color;
            }

            graphicsPanel1.Invalidate();
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                cellColor = colorDialog.Color;
            }

            graphicsPanel1.Invalidate();
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                gridColor = colorDialog.Color;
            }

            graphicsPanel1.Invalidate();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsForm options = new OptionsForm(universe.GetLength(0), universe.GetLength(1), timer.Interval);

            options.FormClosed += (object s, FormClosedEventArgs f) =>
            {
                if (OptionsForm.CellWidth > 0 && OptionsForm.CellHeight > 0)
                {
                    universe = new bool[OptionsForm.CellWidth, OptionsForm.CellHeight];
                }
                   
                if (OptionsForm.Interval > 0)
                {
                    timer.Interval = OptionsForm.Interval;

                    toolStripStatusLabel1.Text = "Interval = " + timer.Interval;
                }

                graphicsPanel1.Invalidate();
            };

            options.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var writer = new StreamWriter(File.OpenWrite(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "GameOfLifeSettings.sav"));

            writer.Write((char)timer.Interval);
            writer.Write((char)graphicsPanel1.BackColor.R);
            writer.Write((char)graphicsPanel1.BackColor.G);
            writer.Write((char)graphicsPanel1.BackColor.B);

            writer.Write((char)cellColor.R);
            writer.Write((char)cellColor.G);
            writer.Write((char)cellColor.B);

            writer.Write((char)gridColor.R);
            writer.Write((char)gridColor.G);
            writer.Write((char)gridColor.B);

            writer.Write((char)universe.GetLength(0));
            writer.Write((char)universe.GetLength(1));

            writer.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "GameOfLifeSettings.sav"))
            {
                var reader = new StreamReader(File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "GameOfLifeSettings.sav"));

                timer.Interval = reader.Read();

                graphicsPanel1.BackColor = Color.FromArgb(reader.Read(), reader.Read(), reader.Read());

                cellColor = Color.FromArgb(reader.Read(), reader.Read(), reader.Read());
                gridColor = Color.FromArgb(reader.Read(), reader.Read(), reader.Read());

                universe = new bool[reader.Read(), reader.Read()];

                reader.Close();

                toolStripStatusLabel1.Text = "Interval = " + timer.Interval;

                graphicsPanel1.Invalidate();
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.White;
            gridColor = Color.Black;
            cellColor = Color.Gray;
            timer.Interval = 20;

            universe = new bool[20, 20];

            toolStripStatusLabel1.Text = "Interval = " + timer.Interval;

            graphicsPanel1.Invalidate();
        }
    }
}
