namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("input.txt"));

            for (var i = 0; i < arrangements.Length; i++)
            {
                var arrangement = arrangements[i];

                Console.WriteLine($"Original arrangement:");
                arrangement.Visualise();

                var heightOfCell = arrangement.GetHeighestCellHeight();

                arrangement.RaiseNeighborsOfAllHighCellsWithoutCorners();

                var changed = arrangement.GetTotalAddedBlocks();
                var isSafe = arrangement.IsSafe();

                Console.WriteLine("Final arrangement:");

                Console.WriteLine($"Case #{i + 1}: Added Blocks = {changed}, Safe = {isSafe}");
                arrangement.Visualise();
            }

        }
    }
}
