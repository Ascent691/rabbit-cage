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
                
                int offset = 0;

                var totalArrangementsLineValues = new int[1];
                CopyNumbersOnLineTo(data, ref offset, totalArrangementsLineValues);
                var totalArrangements = totalArrangementsLineValues[0];
                
                rabbitHouseArrangements.Data = new RabbitHouseArrangement[totalArrangements];
                amountOfArrangementsRead.Release(1);
                
                var sizeLineValues = new int[2];
                
                for (int arrangementNumber = 0; arrangementNumber < totalArrangements; arrangementNumber++)
                {
                    CopyNumbersOnLineTo(data, ref offset, sizeLineValues);
                    
                    var numRows = sizeLineValues[0];
                    var numColumns = sizeLineValues[1];

                    Cell[,] cells = new Cell[numRows, numColumns];
                    var queue = new CellQueue();
                    
                    var dataLineValues = new int[numColumns];

                    for (int row = 0; row < numRows; row++)
                    {
                        CopyNumbersOnLineTo(data, ref offset, dataLineValues);
                        for (int column = 0; column < numColumns; column++)
                        {
                            var height = dataLineValues[column];
                            var cell = new Cell(row, column, height, cells);
                            cells[row, column] =  cell;

                            if (height > 1)
                            {
                                queue.Enqueue(cell);    
                            }
                        }
                    }
                    
                    rabbitHouseArrangements.Data[arrangementNumber] = new RabbitHouseArrangement(queue);
                    arrangementDataRead.Release(1);
                }
            });

            return rabbitHouseArrangements;
        }
        
        private static void CopyNumbersOnLineTo(ReadOnlySpan<byte> source, ref int offset, int[] destination)
        {
            for (var i = 0; i < destination.Length; i++)
            {
                int parsedValue = 0;
                while (source[offset] != Space && source[offset] != CarriageReturn  && source[offset] != NewLine)
                {
                    var digit = source[offset] - 48;
                    parsedValue = parsedValue * 10 + digit;
                    offset++;
                }
                destination[i] = parsedValue;
                
                while (source[offset] == Space)
                {
                    offset++;
                }
            }
            
            while (offset < source.Length && (source[offset] == CarriageReturn || source[offset] == NewLine))
            {
                offset++;
            }
        }
    }
}
