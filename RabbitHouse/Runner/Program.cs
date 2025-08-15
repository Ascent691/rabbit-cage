using System.Data.Common;
using System.Diagnostics;

namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            var answers = new RabbitHouseAnswerParser().Parse(File.ReadAllLines("1.ans"));
            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("1.in"));
            var arrangementIndex = 0;
            var totalUnmatched = 0;


            

            foreach (var arrangement in arrangements)
            {
                //arrangement.Visualise();
                var safeCells = new HashSet<int>();

                while (!arrangement.IsSafe())
                {
                    var didSomething = false;
                    var highestPoint = GetHeighestCellCoordinates(arrangement, safeCells);
                    if (highestPoint == null)
                    {
                        break;
                    }
                    var height = Math.Max(arrangement[highestPoint.X, highestPoint.Y] - 1, 0);

                    foreach (var unsafeNeighbour in GetUnsafeNeighbours(arrangement, highestPoint))
                    {
                        arrangement.SetHeightAt(unsafeNeighbour.X, unsafeNeighbour.Y, height);
                        didSomething = true;
                    }
                    safeCells.Add(GetIndexForPoint(arrangement, highestPoint));

                    //if (didSomething)
                    //    arrangement.Visualise();
                }



                var totalAddedBlocks = arrangement.GetTotalAddedBlocks();
                var isMatch = answers[arrangementIndex] == totalAddedBlocks;
                Console.WriteLine($"Case #{arrangementIndex + 1}: {totalAddedBlocks}, Expected: {answers[arrangementIndex]}, Match: {isMatch}");
                arrangementIndex++;

                if (isMatch)
                    totalUnmatched++;
            }
            timer.Stop();
            var elapsed = timer.Elapsed.ToString();
            Console.WriteLine($"Executed in {elapsed}");

            if (totalUnmatched == arrangements.Length)
            {
                Console.WriteLine("All test cases match!");
            }
            else
            {
                Console.WriteLine($"Unmatched test cases: {totalUnmatched}");
            }
            Console.ReadLine();
        }

        public static IEnumerable<Point> GetUnsafeNeighbours(RabbitHouseArrangement arrangement, Point point)
        {
            int fromHeight = arrangement[point.X, point.Y];

            for (int dR = -1; dR <= 1; dR++)
            {
                if (point.X + dR < 0 || point.X + dR >= arrangement.TotalRows) continue;
                for (int dC = -1; dC <= 1; dC++)
                {

                    if (point.Y + dC < 0 || point.Y + dC >= arrangement.TotalColumns) continue;
                    if (Math.Abs(dC) + Math.Abs(dR) > 1) continue;
                    int toHeight = arrangement[point.X + dR, point.Y + dC];
                    if (Math.Abs(toHeight - fromHeight) > 1)
                    {
                        yield return new Point { X = point.X + dR, Y = point.Y + dC };
                    }
                }
            }
        }

        public static Point GetHeighestCellCoordinates(RabbitHouseArrangement arrangement, HashSet<int> excludedPoints)
        {
            var result = new Point();
            int value = -1;

            for (int row = 0; row < arrangement.TotalRows; row++)
                for (int column = 0; column < arrangement.TotalColumns; column++)
                    if (value < arrangement[row, column])
                    {
                        var index = GetIndexForPoint(arrangement, row, column);

                        if (!excludedPoints.Contains(index))
                        {
                            value = arrangement[row, column];
                            result.X = row;
                            result.Y = column;
                        }

                    }

            return value == -1 ? null : result;
        }

        public static int GetIndexForPoint(RabbitHouseArrangement arrangement, Point point)
        {
            return (point.X * arrangement.TotalColumns) + point.Y;
        }

        public static int GetIndexForPoint(RabbitHouseArrangement arrangement, int x, int y)
        {
            return (x * arrangement.TotalColumns) + y;
        }
    }

    public record Point
    {
        public int X { get; set; } 
        public int Y { get; set; }
    }
}
