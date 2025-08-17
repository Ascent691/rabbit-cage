using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace Runner
{
    internal class Program
    {
        static void Main()
        {
            var stopwatch = Stopwatch.StartNew();
            var data = File.ReadAllText("1.in");
            var fileReadTimeStamp = stopwatch.Elapsed;
            var arrangements = new RabbitHouseParser().Parse(data);
            var parsingTimeStamp = stopwatch.Elapsed;

            var addedTotalForAllArrangements = new ConcurrentDictionary<int, RefCount>();

            for (int caseNumber = 0; caseNumber < arrangements.Length; caseNumber++)
            {
                var addedTotalForArrangement = new RefCount();
                addedTotalForAllArrangements[caseNumber] = addedTotalForArrangement;
                var arrangement = arrangements[caseNumber];
                
                var cells = arrangement.Cells;
                var queue = new CellQueue();
                foreach (var cell in cells)
                {
                    cell.ReferenceNeighbours(cells, arrangement.TotalRows, arrangement.TotalColumns);
                    queue.EnqueueNonZero(cell);
                }

                while (queue.Head is not null)
                {
                    var cell = queue.Dequeue();
                    if (cell is not null)
                    {
                        addedTotalForArrangement.Count += cell.MakeSafe(queue);    
                    }
                }
            }

            var consoleOutput = new StringBuilder(addedTotalForAllArrangements.Count);
            foreach (var indexedAddedTotal in addedTotalForAllArrangements)
            {
                consoleOutput.AppendLine($"Case #{indexedAddedTotal.Key + 1}: {indexedAddedTotal.Value.Count}");
            }
            Console.Write(consoleOutput);
            
            stopwatch.Stop();

            Console.WriteLine($"File Read Timestamp: {fileReadTimeStamp.ToString()}");
            Console.WriteLine($"Parsing Timestamp: {parsingTimeStamp.ToString()}");
            Console.WriteLine($"Total Timestamp: {stopwatch.Elapsed.ToString()}");

            var actualAnswers = File.ReadAllLines("1.ans");
            for (int i = 0; i < addedTotalForAllArrangements.Count; i++)
            {
                var calculatedAnswer = $"Case #{i + 1}: {addedTotalForAllArrangements[i].Count}";
                if (calculatedAnswer != actualAnswers[i])
                {
                    Console.WriteLine($"Difference detected, calculated: '{calculatedAnswer}', answer: '{actualAnswers[i]}'");
                }
            }
        }
    }
}