using System.Collections.Concurrent;
using System.Diagnostics;

namespace Runner
{
    internal class Program
    {
        static void Main()
        {
            var stopwatch = Stopwatch.StartNew();
            var arrangements = new RabbitHouseParser().Parse("1.in");
            var addedTotalForAllArrangements = new ConcurrentDictionary<int, RefCount>();

            var t = Task.Run(() =>
            {
                arrangements.AmountDataReadSemaphore.Wait();
                
                if(arrangements.Data == null) throw new Exception("Oh noes you flew to close to the sun!!!");
                
                for (int caseNumber = 0; caseNumber < arrangements.Data.Length; caseNumber++)
                {
                    arrangements.DataAvailableSemaphore.Wait();
                    
                    var addedTotalForArrangement = new RefCount();
                    addedTotalForAllArrangements[caseNumber] = addedTotalForArrangement;
                    var arrangement = arrangements.Data[caseNumber];
                
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

                foreach (var indexedAddedTotal in addedTotalForAllArrangements)
                {
                    Console.WriteLine($"Case #{indexedAddedTotal.Key + 1}: {indexedAddedTotal.Value.Count}");
                }
            });

            t.Wait();
            
            stopwatch.Stop();

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