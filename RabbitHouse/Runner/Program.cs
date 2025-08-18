using System.Diagnostics;

namespace Runner
{
    internal static class Program
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
            await output.WriteLineAsync($"Solution Time (Including Console Output) : {solutionStopWatch.Elapsed.ToString()}");

            var answerVerificationStopWatch = VerifyAnswers(answers, output);
            await output.WriteLineAsync($"Answer Verification Time                 : {answerVerificationStopWatch.Elapsed.ToString()}");
            
            var totalTime = answerVerificationStopWatch.Elapsed + solutionStopWatch.Elapsed;
            await output.WriteLineAsync($"Total Time                               : {totalTime.ToString()}");
        }

        private static Task<Answer[]> CalculateAnswers(RabbitHouseArrangements arrangements, StreamWriter output)
        {
            return Task.Run(() =>
            {
                arrangements.WaitForAmountOfCasesToBeRead();
                
                if(arrangements.Data == null) throw new Exception("Oh noes you flew to close to the sun!!!");

                var answers = new Answer[arrangements.Data.Length];
                
                for (var i = 0; i < arrangements.Data.Length; i++)
                {
                    arrangements.WaitCaseDataToBeRead();
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

        private static Stopwatch VerifyAnswers(Answer[] calculatedAnswers, StreamWriter o)
        {
            var answerVerificationStopWatch = Stopwatch.StartNew();
            
            var actualAnswers = File.ReadAllLines(AnswerPath);
            for (var i = 0; i < calculatedAnswers.Length; i++)
            {
                if (calculatedAnswers[i].ToString() != actualAnswers[i])
                {
                    o.WriteLine($"Difference detected, calculated: '{calculatedAnswers[i]}', answer: '{actualAnswers[i]}'");
                }
            }
            answerVerificationStopWatch.Stop();
            
            return answerVerificationStopWatch;
        }
    }
}