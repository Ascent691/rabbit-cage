namespace Runner
{
    public class RabbitHouseParser
    {
        private static readonly byte CarriageReturn = 13;
        private static readonly byte NewLine = 10;
        private static readonly byte Space = 32;

        public RabbitHouseArrangements Parse(string path)
        {
            var amountOfArrangementsRead = new SemaphoreSlim(0, 1);
            var arrangementDataRead = new SemaphoreSlim(0);
            var rabbitHouseArrangements = new RabbitHouseArrangements(amountOfArrangementsRead, arrangementDataRead);

            Task.Run(() =>
            {
                ReadOnlySpan<byte> data = File.ReadAllBytes(path);
                
                int position = 0;

                var totalArrangementsLineValues = new int[1];
                CopyNumbersOnLineTo(data, ref position, totalArrangementsLineValues);
                var totalArrangements = totalArrangementsLineValues[0];
                
                rabbitHouseArrangements.Data = new RabbitHouseArrangement[totalArrangements];
                amountOfArrangementsRead.Release(1);
                
                var sizeLineValues = new int[2];
                
                for (int i = 0; i < totalArrangements; i++)
                {
                    CopyNumbersOnLineTo(data, ref position, sizeLineValues);
                    
                    var numRows = sizeLineValues[0];
                    var numColumns = sizeLineValues[1];
                    
                    Cell[,] cells = new Cell[numRows, numColumns];
                    var queue = new CellQueue();
                    
                    var dataLineValues = new int[numColumns];

                    for (int row = 0; row < numRows; row++)
                    {
                        CopyNumbersOnLineTo(data, ref position, dataLineValues);
                        for (int column = 0; column < numColumns; column++)
                        {
                            var cell = new Cell(row, column, dataLineValues[column], cells);
                            cells[row, column] =  cell;
                            if (cell.Height > 1)
                            {
                                queue.Enqueue(cell);    
                            }
                        }
                    }
                    
                    rabbitHouseArrangements.Data[i] = new RabbitHouseArrangement(queue);
                    arrangementDataRead.Release(1);
                }
            });

            return rabbitHouseArrangements;
        }
        
        private static void CopyNumbersOnLineTo(ReadOnlySpan<byte> span, ref int position, int[] destination)
        {
            for (var i = 0; i < destination.Length; i++)
            {
                int parsedValue = 0;
                while (span[position] != Space && span[position] != CarriageReturn  && span[position] != NewLine)
                {
                    var digit = span[position] - 48;
                    parsedValue = parsedValue * 10 + digit;
                    position++;
                }
                destination[i] = parsedValue;
                
                while (span[position] == Space)
                {
                    position++;
                }
            }
            
            while (position < span.Length && (span[position] == CarriageReturn || span[position] == NewLine))
            {
                position++;
            }
        }
    }
}
