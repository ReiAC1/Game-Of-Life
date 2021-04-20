using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public static class GOL
    {
        static bool[,] buffer = null;

        // Gets whether the cell is true or false, follows wrap rules
        static bool GetNeighbor(bool[,] grid, int x, int y, bool wrap)
        {

            // Handle out of bounds when NOT wrapping, we return false
            if ((x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1)) && !wrap)
                return false;

            // If we're wrapping, check for out of bounds and if we are, set the positions to the opposite side of the grid
            if (x < 0 || x >= grid.GetLength(0))
            {
                x = grid.GetLength(0) - Math.Abs(x);
            }

            if (y < 0 || y >= grid.GetLength(1))
            {
                y = grid.GetLength(1) - Math.Abs(y);
            }

            // return the grid position at x/y
            return grid[x, y];
        }

        // Returns the amount of neighbors the currents cell has
        public static int GetNeighborCount(bool[,] grid, int x, int y, bool wrap)
        {
            int count = 0;

            // loop through -1, 0, and 1 offsets for the cell's x/y
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {

                    // the offset 0,0 is our current cell, ignore it
                    if (i == 0 && j == 0)
                        continue;

                    if (GetNeighbor(grid, x + i, y + j, wrap))
                        count++;
                }
            }

            return count;
        }

        // Updates the entire world, uses a grid as a reference, and returns the total number of living cells
        public static int FrameUpdate(ref bool[,] grid, bool wrap)
        {
            // create a new buffer for us to make the frame modification if it's null or if the size of the grid has changed
            if (buffer == null || buffer.GetLength(0) != grid.GetLength(0) || buffer.GetLength(1) != grid.GetLength(1))
                buffer = new bool[grid.GetLength(0), grid.GetLength(1)];

            int alive = 0;

            // loop through each cell
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // get number of neighbors
                    int count = GetNeighborCount(grid, i, j, wrap);

                    // set cell to false
                    buffer[i, j] = false;

                    // run through game rules
                    if ((count == 2 || count == 3) && grid[i, j]) 
                        buffer[i, j] = true;

                    if (count == 3)
                        buffer[i, j] = true;

                    // increment alive counter if cell is on
                    if (buffer[i, j])
                        alive++;
                }
            }

            // swap the buffer and grid
            bool[,] temp = grid;
            grid = buffer;
            buffer = temp;

            return alive;
        }

        // quick function to get number of alive cells in a grid
        public static int GetAliveCells(bool[,] grid)
        {
            int a = 0;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j])
                        a++;
                }
            }

            return a;
        }

        // Quick and simple grid randomizer, loops through every grid cell and randomly sets it
        // returns alive cells
        public static int RandomizeGrid(ref bool[,] grid, Random r)
        {
            int c = 0;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = r.Next(2) == 1;

                    // increment alive cells if grid cell is on
                    if (grid[i, j])
                        c++;
                }
            }

            return c;
        }
    }
}
