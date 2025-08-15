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
                        value = _cells[row, column];
                    }

            return value;
        }

        public void Visualise()
        {
            var longestHeightToOutput = GetHeighestCellHeight().ToString().Length;

            for (int row = 0; row < TotalRows; row++)
            {
                var line = "";

                for (int column = 0; column < TotalColumns; column++)
                    line += _cells[row, column].ToString($"D{longestHeightToOutput}") + " ";
                Console.WriteLine(line);
            }

            Console.WriteLine();
        }
        public bool RaiseNeighborsOfAllHighCellsWithoutCorners()
        {
            var anyChanged = false;
            var highest = GetHeighestCellHeight();

            bool IsCorner(int row, int column) =>
                (row == 0 && column == 0) || (row == 0 && column == TotalColumns - 1) ||
                (row == TotalRows - 1 && column == 0) || (row == TotalRows - 1 && column == TotalColumns - 1);

            (int rowDirection, int columnDirection)[] directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];
            while (highest > 0)
            {
                for (var row = 0; row < TotalRows; row++)
                {
                    for (var column = 0; column < TotalColumns; column++)
                    {
                        if (_cells[row, column] != highest) continue;

                        foreach (var (rowDirection, columnDirection) in directions)
                        {
                            int neighborRow = row + rowDirection, neighborColumn = column + columnDirection;
                            if (neighborRow >= 0 && neighborRow < TotalRows && neighborColumn >= 0 && neighborColumn < TotalColumns && !IsCorner(neighborRow, neighborColumn))
                            {
                                if (_cells[neighborRow, neighborColumn] < highest - 1)
                                {
                                    SetHeightAt(neighborRow, neighborColumn, highest - 1);
                                    anyChanged = true;
                                }
                            }
                        }
                    }
                }
                highest--;
            }

            return anyChanged;
        }

    }
}
