using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner
{
    public class RabbitHouseParser
    {
        public RabbitHouseArrangement[] Parse(ReadOnlySpan<char> input)
        {
            var lines = 0;
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == '\n') lines++;
            }

            var lineRanges = new Range[lines];
            input.Split(lineRanges, "\r\n", StringSplitOptions.TrimEntries);

            int lineIndex = 0;
            var arrangementLineRange = lineRanges[lineIndex++];
            var totalArrangements = FastReadInt32(input, arrangementLineRange);
            var result = new RabbitHouseArrangement[totalArrangements];
            
            var sizeLineValueRanges = new Range[2];
            
            for (int i = 0; i < totalArrangements; i++) {
                var sizeLine = SplitLineIntoValueRanges(input, lineRanges[lineIndex++], sizeLineValueRanges);

                var numRows = FastReadInt32(sizeLine, sizeLineValueRanges[0]);
                var numColumns = FastReadInt32(sizeLine, sizeLineValueRanges[1]);
                
                Cell[,] cells = new Cell[numRows, numColumns];
                
                var dataLineValueRanges = new Range[numColumns];
                
                for (int k = 0; k < numRows; k++)
                {
                    var dataLine = SplitLineIntoValueRanges(input, lineRanges[lineIndex++], dataLineValueRanges);
                    for (int j = 0; j < numColumns; j++)
                    {
                        var v = FastReadInt32(dataLine, dataLineValueRanges[j]);
                        cells[k, j] =  new Cell(k, j, v);
                    }
                }

                result[i] = new RabbitHouseArrangement(cells, numRows, numColumns);
            }
            
            return result;
        }

        
        private static int FastReadInt32(ReadOnlySpan<char> sizeLine, Range sizeLineValueRange)
        {
            var span = sizeLine.Slice(sizeLineValueRange.Start.Value, sizeLineValueRange.End.Value - sizeLineValueRange.Start.Value);
            int parsedValue = 0;
            for (int i = 0; i < span.Length; i++)
            {
                parsedValue = parsedValue * 10 + (span[i] - '0');
            }

            return parsedValue;
        }
        
        private static ReadOnlySpan<char> SplitLineIntoValueRanges(
            ReadOnlySpan<char> input, Range lineRange, Span<Range> ranges)
        {
            var line = input.Slice(lineRange.Start.Value, lineRange.End.Value - lineRange.Start.Value);
            line.Split(ranges, ' ', StringSplitOptions.TrimEntries);
            
            return line;
        }
    }

    public class RabbitHouseArrangement(Cell[,] cells, int totalRows, int totalColumns)
    {
        public readonly Cell[,] Cells = cells;
        public readonly int TotalRows = totalRows;
        public readonly int TotalColumns = totalColumns;
    }
}
