namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("1.in"));

            foreach (var arrangement in arrangements)
            {
                var rows = arrangement.TotalRows;
                var cols = arrangement.TotalColumns;
                //var checkedJumps = new HashSet<Jump>();

                var unsafeJumps = new HashSet<Jump>();
                var uncheckedJumps = new HashSet<Jump>();

                do
                {
                    unsafeJumps.Clear();

                    //check all cells
                    for (var y = 0; y < rows; y++)
                    {
                        for (var x = 0; x < cols; x++)
                        {
                            if (x != cols - 1)
                            {
                                var h = Math.Abs(arrangement[y, x] - arrangement[y, x + 1]);
                                var jump = new Jump(new Cell(y, x), new Cell(y, x + 1));
                                //checkedJumps.Add(jump);
                                if (h > 1)
                                {
                                    unsafeJumps.Add(jump);
                                }
                            }

                            if (y != rows - 1)
                            {
                                var h = Math.Abs(arrangement[y, x] - arrangement[y + 1, x]);
                                var jump = new Jump(new Cell(y, x), new Cell(y + 1, x));
                                //checkedJumps.Add(jump);
                                if (h > 1)
                                {
                                    unsafeJumps.Add(jump);
                                }
                            }
                        }
                    }

                    //make unsafe jumps safe
                    foreach (var unsafeJump in unsafeJumps)
                    {
                        var hA = arrangement[unsafeJump.A.Row, unsafeJump.A.Col];
                        var hB = arrangement[unsafeJump.B.Row, unsafeJump.B.Col];
                        if (hA > hB)
                        {
                            arrangement.SetHeightAt(unsafeJump.B.Row, unsafeJump.B.Col, hA - 1);
                        }
                        else
                        {
                            arrangement.SetHeightAt(unsafeJump.A.Row, unsafeJump.A.Col, hB - 1);
                        }
                    }
                } while (unsafeJumps.Count != 0);
                

                //Console.WriteLine(checkedJumps.Count);
                //Console.WriteLine(unsafeJumps.Count);

                //arrangement.Visualise();
                //Console.WriteLine(arrangement.IsSafe());
                Console.WriteLine(arrangement.GetTotalAddedBlocks());
            }
        }

        private record Cell(int Row, int Col);

        private record Jump(Cell A, Cell B);

        static void CheckAdjacent()
        {

        }
    }
}