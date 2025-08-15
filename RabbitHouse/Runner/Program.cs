namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var arrangements = new RabbitHouseParser().Parse(input);

            foreach (var arrangement in arrangements)
            {
                bool isSafe = arrangement.IsSafe();
                while (!isSafe)
                {
                    RunCellArranger(arrangement);
                    isSafe = arrangement.IsSafe();
                }
                
            }
        }

        static void RunCellArranger(RabbitHouseArrangement arrangement)
        {
            var blocksAddedToArrangement = 0;

            var heighestCellHeight = arrangement.GetHeighestCellHeight();
            var cellsToFocus = arrangement.FindAllCellsOfHeight(heighestCellHeight).ToList();
            for(int x = 0; x < cellsToFocus.Count(); x++)
            {
                var location = new int[]{cellsToFocus[x][0,0], cellsToFocus[x][0,1]};
                var safeCellOutput = arrangement.SetSurroundingCellsToSafe(heighestCellHeight, location);
                blocksAddedToArrangement += safeCellOutput;
            }
                
            arrangement.Visualise(blocksAddedToArrangement);
        }
    }
}
