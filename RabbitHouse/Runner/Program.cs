using System.Collections.Concurrent;
using System.Diagnostics;

namespace Runner
{
    internal class Program
    {
        private static readonly string InputPath = "1.in";
        private static readonly string AnswerPath = "1.ans";

        static async Task Main()
        {
            var solutionStopWatch = Stopwatch.StartNew();
            var arrangements = new RabbitHouseParser().Parse(InputPath);

            await using var output = new StreamWriter(Console.OpenStandardOutput());

            var answers = await CalculateAnswers(arrangements, output);

            solutionStopWatch.Stop();
            output.WriteLine($"Solution Time (Including Console Output) : {solutionStopWatch.Elapsed.ToString()}");

            var answerVerificationStopWatch = VerifyAnswers(answers, output);
            output.WriteLine($"Answer Verification Time                 : {answerVerificationStopWatch.Elapsed.ToString()}");
            
            var totalTime = answerVerificationStopWatch.Elapsed + solutionStopWatch.Elapsed;
            output.WriteLine($"Total Time                               : {totalTime.ToString()}");
        }

        private static Task<Answer[]> CalculateAnswers(RabbitHouseArrangements arrangements, StreamWriter output)
        {
            return Task.Run(() =>
            {
                arrangements.AmountDataReadSemaphore.Wait();
                
                if(arrangements.Data == null) throw new Exception("Oh noes you flew to close to the sun!!!");

                var answers = new Answer[arrangements.Data.Length];
                
                for (int i = 0; i < arrangements.Data.Length; i++)
                {
                    arrangements.DataAvailableSemaphore.Wait();
                    var arrangement = arrangements.Data[i];

                    var answer = new Answer(i);
                    answers[i] = answer;

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
                            answer.Count += cell.MakeSafe(queue);    
                        }
                    }
                    
                    output.WriteLine(answer.ToString());
                }

                return answers;
            });
        }

        private static Stopwatch VerifyAnswers(Answer[] answers, StreamWriter o)
        {
            var answerVerificationStopWatch = Stopwatch.StartNew();
            
            var actualAnswers = File.ReadAllLines(AnswerPath);
            for (int i = 0; i < answers.Length; i++)
            {
                if (answers[i].ToString() != actualAnswers[i])
                {
                    o.WriteLine($"Difference detected, calculated: '{answers[i]}', answer: '{actualAnswers[i]}'");
                }
            }
            answerVerificationStopWatch.Stop();
            
            return answerVerificationStopWatch;
        }
    }
}