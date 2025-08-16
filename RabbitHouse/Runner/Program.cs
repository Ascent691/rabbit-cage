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

            Parallel.For(0, arrangements.Length, caseNumber =>
            {
                var addedTotalForArrangement = new RefCount();
                addedTotalForAllArrangements[caseNumber] = addedTotalForArrangement;
                var arrangement = arrangements[caseNumber];
                
                var cells = arrangement.Cells;
                
                var unsafeCellQueue = new PriorityQueue<Cell, int>();
                foreach (var cell in cells)
                {
                    cell.ReferenceNeighbours(cells, arrangement.TotalRows, arrangement.TotalColumns);
                    unsafeCellQueue.Enqueue(cell, 0 - cell.Height);
                }

                while (unsafeCellQueue.Count > 0)
                {
                    var queueSnapshot = unsafeCellQueue;
                    unsafeCellQueue = new PriorityQueue<Cell, int>();

                    while (queueSnapshot.TryDequeue(out var cell, out _))
                    {
                        addedTotalForArrangement.Count += cell.MakeSafe();
                    }
                    
                    foreach (var cell in cells)
                    {
                        if (!cell.IsSafe())
                        {
                            unsafeCellQueue.Enqueue(cell, 0 - cell.Height);
                        }
                    }
                }
            });

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