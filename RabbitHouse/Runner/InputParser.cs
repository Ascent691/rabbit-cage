using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner
{
    public class RabbitHouseParser
    {
        public RabbitHouseParser() { }

        public RabbitHouseArrangement[] Parse(string[] lines)
        {
            int lineIndex = 0;
            var totalArrangements = Int32.Parse(lines[lineIndex++].Trim());
            RabbitHouseArrangement[] result = new RabbitHouseArrangement[totalArrangements];
            for (int i = 0; i < totalArrangements; i++) {
                string[] parts = lines[lineIndex++].Split(' ');
                int numRows = Int32.Parse(parts[0]);
                int numColumns = Int32.Parse(parts[1]);
                int[,] cells = new int[numRows, numColumns];

                for (int k = 0; k < numRows; k++)
                {
                    parts = lines[lineIndex++].Split(' ');
                    for (int j = 0; j < numColumns; j++)
                    {
                        cells[k, j] = Int32.Parse(parts[j]);
                    }
                }

                result[i] = new RabbitHouseArrangement(cells, numRows, numColumns);
            }

            return result;
        }
    }

    public class RabbitHouseArrangement
    {

        private readonly int[,] _immutableCells;
        private readonly int[,] _cells;

        private int lastRegisteredMaxHeight = 0;

        public int TotalRows { get; }
        public int TotalColumns { get; }

        public int this[int row, int column] { get { return _cells[row,column]; } }



        public RabbitHouseArrangement(int[,] cells, int totalRows, int totalColumns)
        {
            this._immutableCells = cells;
            this._cells = new int[totalRows, totalColumns];
            Array.Copy(cells, _cells, cells.Length);
            TotalRows = totalRows;
            TotalColumns = totalColumns;
        }


        public int GetHeightAt(int row, int column)
        {
            return _cells[row, column];
        }

        public void SetHeightAt(int row, int column, int value)
        {
            if (value < _immutableCells[row, column])
                throw new ArgumentOutOfRangeException("value", value, $"Cannot make cell [${row},${column}] have a height of ${value}, as that would be lower than its original height of ${_immutableCells[row, column]}.");
            _cells[row, column] = value;
        }

        public bool IsSafe()
        {
            for (int row = 0; row < TotalRows; row++)
                for (int column = 0; column < TotalColumns; column++)
                {
                    int fromHeight = _cells[row, column];

                    for (int dR = -1; dR <= 1; dR++)
                    {
                        if (row + dR < 0 || row + dR >= TotalRows) continue;
                        for (int dC = -1; dC <= 1; dC++)
                        {
                            if (column + dC < 0 || column + dC >= TotalColumns) continue;
                            int toHeight = _cells[row + dR, column + dC];
                            if (Math.Abs(toHeight - fromHeight) > 1)
                            {
                                return false;
                            }
                        }
                    }
                }

            return true;

        }

        public int GetTotalAddedBlocks()
        {
            var result = 0;

            for (int row = 0; row < TotalRows; row++)
                for (int column = 0; column < TotalColumns; column++)
                    result += _cells[row, column] - _immutableCells[row, column];

            return result;
        }

        public int GetHeighestCellHeight()
        {
            int value = 0;

            for (int row = 0; row < TotalRows; row++)
                for (int column = 0; column < TotalColumns; column++)
                    if (value < _cells[row, column])
                    {
                        if(lastRegisteredMaxHeight != 0 && _cells[row, column] >= lastRegisteredMaxHeight) continue;
                        value = _cells[row, column];
                    }
            lastRegisteredMaxHeight = value;

            return lastRegisteredMaxHeight;
        }

        public void Visualise(int blocksAdded)
        {
            var longestHeightToOutput = GetHeighestCellHeight().ToString().Length;

            for (int row = 0; row < TotalRows; row++)
            {
                var line = "";

                for (int column = 0; column < TotalColumns; column++)
                    line += _cells[row, column].ToString($"D{longestHeightToOutput}") + " ";
                Console.WriteLine(line);
            }

            Console.WriteLine($"{blocksAdded}\n");
        }

        public IEnumerable<int[,]> FindAllCellsOfHeight(int height)
        {
            var cellsOfHeight = new int[,]{};

            for (int x = 0; x < _cells.GetLength(0); x++)
            {
                for (int y = 0; y < _cells.GetLength(1); y++)
                {
                    if (_cells[x, y] == height)
                    {
                        var newCells = new int[cellsOfHeight.GetLength(0) + 1, cellsOfHeight.GetLength(1) + 1];
                        Array.Copy(cellsOfHeight, newCells, cellsOfHeight.Length);
                        yield return new int[,]{ {x ,y }};
                    }
                }
            }
        }
        
        public List<(int,int[])> GetAdjacentValues(int x, int y, bool includeDiagonals = false)
        {
            List<(int,int[])> adjacentValues = new List<(int,int[])>();
            int rows = _cells.GetLength(0);
            int cols = _cells.GetLength(1);

            // Iterate through possible offsets for adjacent cells
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    // Skip the center cell itself
                    if (dx == 0 && dy == 0)
                        continue;

                    // Skip diagonal cells if not included
                    if (!includeDiagonals && (dx != 0 && dy != 0))
                        continue;

                    int newX = x + dx;
                    int newY = y + dy;

                    // Check if the new coordinates are within array bounds
                    if (newX >= 0 && newX < rows && newY >= 0 && newY < cols)
                    {
                        adjacentValues.Add((_cells[newX, newY], [newX, newY]));
                    }
                }
            }
            return adjacentValues;
        }

        public int SetSurroundingCellsToSafe(int cellHeight, int[] cellLocation)
        {
            var blocksAdded = 0;
            var adjacentCells = GetAdjacentValues(cellLocation[0], cellLocation[1]);
            for (int i = 0; i < adjacentCells.Count; i++)
            {
                int newCellHeight = adjacentCells[i].Item1;
                if (newCellHeight >= cellHeight) continue;
                while (newCellHeight < cellHeight - 1)
                {
                    blocksAdded++;
                    newCellHeight++;
                }
                _cells[adjacentCells[i].Item2[0], adjacentCells[i].Item2[1]] = newCellHeight;
            }
            return blocksAdded;
        }
    }
}
