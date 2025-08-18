using System.Collections.Concurrent;
using System.Diagnostics;

namespace Runner
{
    internal class Program
    {
        static void Main()
        {
            var solutionStopWatch = Stopwatch.StartNew();
            var arrangements = new RabbitHouseParser().Parse("1.in");
            var addedTotalForAllArrangements = new ConcurrentDictionary<int, RefCount>();

            using var o = new StreamWriter(Console.OpenStandardOutput());

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
                    o.WriteLine($"Case #{indexedAddedTotal.Key + 1}: {indexedAddedTotal.Value.Count}");
                }
            });

            t.Wait();
            
            solutionStopWatch.Stop();
            o.WriteLine($"Solution Time (Including Console Output) : {solutionStopWatch.Elapsed.ToString()}");

            var answerVerificationStopWatch = Stopwatch.StartNew();
            var actualAnswers = File.ReadAllLines("1.ans");
            for (int i = 0; i < addedTotalForAllArrangements.Count; i++)
            {
                var calculatedAnswer = $"Case #{i + 1}: {addedTotalForAllArrangements[i].Count}";
                if (calculatedAnswer != actualAnswers[i])
                {
                    o.WriteLine($"Difference detected, calculated: '{calculatedAnswer}', answer: '{actualAnswers[i]}'");
                }
            }
            answerVerificationStopWatch.Stop();
            o.WriteLine($"Answer Verification Time                 : {answerVerificationStopWatch.Elapsed.ToString()}");
            var totalTime = answerVerificationStopWatch.Elapsed + solutionStopWatch.Elapsed;
            o.WriteLine($"Total Time                               : {totalTime.ToString()}");
        }
    }
}