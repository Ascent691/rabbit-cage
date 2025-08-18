using System.Diagnostics;

namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("1.in"));
            var answers = new long[arrangements.Length];
            var currCase = 0;

            var optimiser = new RabbitHouseOptimiser();

            foreach (var arrangement in arrangements)
            {
                optimiser.Optimise(arrangement);
                answers[currCase++] = arrangement.GetTotalAddedBlocks();
            }

            CheckAnswers(answers);

            sw.Stop();
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds} ms");
        }

        private static void CheckAnswers(long[] answers)
        {
            var correctAnswers = new RabbitHouseAnswerParser().Parse(File.ReadAllLines("1.ans"));

            var incorrectAnswerCount = 0;
            foreach (var (answer, correct) in answers.Zip(correctAnswers))
            {
                if (answer == correct) continue;

                Console.WriteLine($"Incorrect. Expected: {correct} - Actual: {answer}");
                incorrectAnswerCount++;
            }

            Console.WriteLine(incorrectAnswerCount == 0 
                ? $"All {answers.Length} answers were correct" 
                : $"{incorrectAnswerCount} answers were incorrect");
        }
    }
}