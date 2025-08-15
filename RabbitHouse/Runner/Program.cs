namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("input.txt"));

            var arrangement = arrangements[0];

            MakeArrangementSafe(arrangement);

            arrangement.Visualise();

            Console.WriteLine($"Total blocks added is: {arrangement.GetTotalAddedBlocks()}");
        }

        private static void MakeArrangementSafe(RabbitHouseArrangement arrangement)
        {
            var hasChanges = false;

            for (var row = 0; row < arrangement.TotalRows; row++)
            {
                for (var col = 0; col < arrangement.TotalColumns; col++)
                {
                    hasChanges |= AdjustCellWithNeighbors(arrangement, row, col);
                }
            }

            if (hasChanges)
            {
                MakeArrangementSafe(arrangement);
            }
        }

        private static bool AdjustCellWithNeighbors(RabbitHouseArrangement arrangement, int currentRow, int currentCol)
        {
            var currentCellHeight = arrangement.GetHeightAt(currentRow, currentCol);
            var hasAdjusted = false;

            for (var rowOffset = -1; rowOffset <= 1; rowOffset++)
            {
                var neighborRow = currentRow + rowOffset;
                if (!IsValidRow(arrangement, neighborRow)) continue;

                for (var colOffset = -1; colOffset <= 1; colOffset++)
                {
                    var neighborCol = currentCol + colOffset;
                    if (!IsValidColumn(arrangement, neighborCol)) continue;

                    if (neighborRow == currentRow && neighborCol == currentCol) continue;

                    var neighborHeight = arrangement.GetHeightAt(neighborRow, neighborCol);

                    if (Math.Abs(currentCellHeight - neighborHeight) <= 1) continue;
                    AdjustSmallerCell(arrangement, currentCellHeight, neighborHeight, currentRow, currentCol, neighborRow, neighborCol);
                    hasAdjusted = true;
                }
            }

            return hasAdjusted;
        }

        private static void AdjustSmallerCell(RabbitHouseArrangement arrangement, int currentCellHeight, int neighborHeight,
            int currentRow, int currentCol, int neighborRow, int neighborCol)
        {
            if (currentCellHeight > neighborHeight)
            {
                arrangement.SetHeightAt(neighborRow, neighborCol, currentCellHeight - 1);
            }
            else
            {
                arrangement.SetHeightAt(currentRow, currentCol, neighborHeight - 1);
            }
        }

        private static bool IsValidRow(RabbitHouseArrangement arrangement, int row)
        {
            return row >= 0 && row < arrangement.TotalRows;
        }

        private static bool IsValidColumn(RabbitHouseArrangement arrangement, int col)
        {
            return col >= 0 && col < arrangement.TotalColumns;
        }
    }
}
