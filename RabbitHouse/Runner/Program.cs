namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("input.txt"));

            foreach (var arrangement in arrangements)
            {
                var heightOfCell = arrangement[0, 0];
                arrangement.SetHeightAt(0,0, 5);

                arrangement.SetHeightAt(1, 0, 5);
                bool isSafe = arrangement.IsSafe();
                var changed = arrangement.GetTotalAddedBlocks();

            }


            Console.WriteLine("Hello, World!");
        }
    }
}
