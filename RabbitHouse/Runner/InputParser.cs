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
                    for (int j = 0; j < numRows; j++)
                    {
                        cells[k, j] = Int32.Parse(parts[j]);
                    }
                }

                result[i] = new RabbitHouseArrangement(cells);
            }

            return result;
        }
    }

    public class RabbitHouseArrangement
    {
        private readonly int[,] _cells;

        public RabbitHouseArrangement(int[,] cells)
        {
            this._cells = cells;
        }
    }
}
