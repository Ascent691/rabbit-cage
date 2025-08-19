using System.Diagnostics;

// TODO get rid of need for an additional loop to reference neighbors by giving the cell a reference to all cells
//      and the dimensions of the grid.
// TODO move the creation and population of the queue into the parser, which should put it onto RabbitHouseArrangement,
//      the Cells property can then be removed.
// TODO draw algorithm diagram.
// TODO try to make code more readable, and where that is not possible document performance choices
//      and give "should consider using in production" rating.

namespace Runner;

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
            arrangements.WaitForTotalNumberOfArrangementsToBeRead();
                
            if(arrangements.Data == null) throw new Exception("Oh noes you flew to close to the sun!!!");

            var answers = new Answer[arrangements.Data.Length];
                
            for (var i = 0; i < arrangements.Data.Length; i++)
            {
                arrangements.WaitArrangementDataToBeRead();
                var arrangement = arrangements.Data[i];

                var answer = new Answer(i);
                answers[i] = answer;

                var cells = arrangement.Cells;
                var queue = new CellQueue();
                    
                for (var cellRow = 0; cellRow < cells.GetLength(0); cellRow++)
                for (var cellColumn = 0; cellColumn < cells.GetLength(1); cellColumn++)
                {
                    var cell = cells[cellRow, cellColumn];
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