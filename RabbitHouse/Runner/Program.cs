using System.Diagnostics;
using System.Text;

namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("1.in"));
            var parsingTimeStamp = stopwatch.Elapsed;

            var stringBuilder = new StringBuilder(arrangements.Length);
            
            for (int i = 0; i < arrangements.Length; i++)
            {
                var totalAdded = 0L;
                var cells = arrangements[i].MapInputCells((row, column, height) =>
                    new Cell(row: row, column: column, height: height));

                
                var nextPriorityQueue = new PriorityQueue<Cell, int>();
                foreach (var cell in cells)
                {
                    cell.ReferenceNeighbours(cells);
                    nextPriorityQueue.Enqueue(cell, 0 - cell.Height);
                }

                while (nextPriorityQueue.Count > 0)
                {
                    var priorityQueue = nextPriorityQueue;
                    nextPriorityQueue = new PriorityQueue<Cell, int>();

                    while (priorityQueue.TryDequeue(out var cell, out _))
                    {
                        totalAdded += cell.BalanceNeighbours();
                    }
                    
                    foreach (var cell in cells)
                    {
                        if (!cell.IsSafe())
                        {
                            nextPriorityQueue.Enqueue(cell, 0 - cell.Height);
                        }
                    }
                }

                stringBuilder.AppendLine($"Case #{i+1}: {totalAdded}");
            }

            var output = stringBuilder.ToString();
            Console.Write(output);
            
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