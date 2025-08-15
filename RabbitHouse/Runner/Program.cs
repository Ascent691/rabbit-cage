using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace Runner
{
    internal class Program
    {
        private static object _lock = new object();
        
        static void Main()
        {
            var stopwatch = Stopwatch.StartNew();
            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("1.in"));
            var parsingTimeStamp = stopwatch.Elapsed;

            // var stringBuilder = new StringBuilder(arrangements.Length);
            var concurrentDictionary = new ConcurrentDictionary<int, long>();

            Parallel.For(0, arrangements.Length, i =>
            {
                concurrentDictionary[i] = 0L;
                var arrangement = arrangements[i];
                var cells = arrangement.MapInputCells((row, column, height) =>
                    new Cell(row: row, column: column, height: height));
                
                var nextPriorityQueue = new PriorityQueue<Cell, int>();
                foreach (var cell in cells)
                {
                    cell.ReferenceNeighbours(cells, arrangement.TotalRows, arrangement.TotalColumns);
                    nextPriorityQueue.Enqueue(cell, 0 - cell.Height);
                }

                while (nextPriorityQueue.Count > 0)
                {
                    var priorityQueue = nextPriorityQueue;
                    nextPriorityQueue = new PriorityQueue<Cell, int>();

                    while (priorityQueue.TryDequeue(out var cell, out _))
                    {
                        concurrentDictionary[i] += cell.MakeSafe();
                    }
                    
                    foreach (var cell in cells)
                    {
                        if (!cell.IsSafe())
                        {
                            nextPriorityQueue.Enqueue(cell, 0 - cell.Height);
                        }
                    }
                }
            });

            var stringBuilder = new StringBuilder(concurrentDictionary.Count);
            foreach (var l in concurrentDictionary)
            {
                stringBuilder.AppendLine($"Case #{l.Key + 1}: {l.Value}");
            }
            Console.Write(stringBuilder);
            
            stopwatch.Stop();

            Console.WriteLine($"Parsing Timestamp: {parsingTimeStamp.ToString()}");
            Console.WriteLine($"Total Timestamp: {stopwatch.Elapsed.ToString()}");

            var calculatedAnswers = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
            var actualAnswers = File.ReadAllLines("1.ans");
            for (int i = 0; i < calculatedAnswers.Count; i++)
            {
                if (calculatedAnswers[i] != actualAnswers[i])
                {
                    Console.WriteLine($"Difference detected, calculated: '{calculatedAnswers[i]}', answer: '{actualAnswers[i]}'");
                }
            }
        }
    }
}