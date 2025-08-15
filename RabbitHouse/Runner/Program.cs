using System.Collections;
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

            foreach (var arrangement in arrangements)
            {
                var rows = arrangement.TotalRows;
                var cols = arrangement.TotalColumns;

                var unsafeJumps = new HashSet<Jump>();
                var changedJumps = new HashSet<Jump>();

                //check all cells
                for (var y = 0; y < rows; y++)
                {
                    for (var x = 0; x < cols; x++)
                    {
                        if (x != cols - 1)
                        {
                            var h = Math.Abs(arrangement[y, x] - arrangement[y, x + 1]);
                            var jump = new Jump(new Cell(y, x), new Cell(y, x + 1));
                            if (h > 1)
                            {
                                unsafeJumps.Add(jump);
                            }
                        }

                        if (y != rows - 1)
                        {
                            var h = Math.Abs(arrangement[y, x] - arrangement[y + 1, x]);
                            var jump = new Jump(new Cell(y, x), new Cell(y + 1, x));
                            if (h > 1)
                            {
                                unsafeJumps.Add(jump);
                            }
                        }
                    }
                }

                do
                {
                    //check changed jumps
                    foreach (var changedJump in changedJumps)
                    {
                        var h = Math.Abs(arrangement[changedJump.A.Row, changedJump.A.Col] -
                                         arrangement[changedJump.B.Row, changedJump.B.Col]);
                        if (h > 1)
                        {
                            unsafeJumps.Add(changedJump);
                        }
                    }

                    changedJumps.Clear();

                    //make unsafe jumps safe
                    foreach (var unsafeJump in unsafeJumps)
                    {
                        var hA = arrangement[unsafeJump.A.Row, unsafeJump.A.Col];
                        var hB = arrangement[unsafeJump.B.Row, unsafeJump.B.Col];
                        if (hA > hB)
                        {
                            arrangement.SetHeightAt(unsafeJump.B.Row, unsafeJump.B.Col, hA - 1);

                            foreach (var jump in GetAffectedJumps(unsafeJump.B, rows, cols))
                            {
                                changedJumps.Add(jump);
                            }
                        }
                        else
                        {
                            arrangement.SetHeightAt(unsafeJump.A.Row, unsafeJump.A.Col, hB - 1);

                            foreach (var jump in GetAffectedJumps(unsafeJump.A, rows, cols))
                            {
                                changedJumps.Add(jump);
                            }
                        }
                    }
                    unsafeJumps.Clear();

                } while (changedJumps.Count != 0);

                answers[currCase++] = arrangement.GetTotalAddedBlocks();
            }
            
            var counter = 1;
            var ans = answers.Select(a => $"Case #{counter++}: {a}");
            File.WriteAllLines("answers.txt", ans);

            sw.Stop();
            Console.WriteLine("Done in: " + sw.ElapsedMilliseconds);
        }

        private record Cell(int Row, int Col);

        private record Jump(Cell A, Cell B);

        private static IEnumerable<Jump> GetAffectedJumps(Cell cell, int rows, int cols)
        {
            if (cell.Row + 1 < rows)
                yield return new Jump(cell, cell with { Row = cell.Row + 1 });
            if (cell.Row - 1 >= 0)
                yield return new Jump(cell with { Row = cell.Row - 1 }, cell);
            if (cell.Col + 1 < cols)
                yield return new Jump(cell, cell with { Col = cell.Col + 1 });
            if (cell.Col - 1 >= 0)
                yield return new Jump(cell with { Col = cell.Col - 1 }, cell);
        }
    }
}