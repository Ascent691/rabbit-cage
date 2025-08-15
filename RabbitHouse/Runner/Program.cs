namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("input.txt"));

            foreach (var arrangement in arrangements)
            {
                arrangement.Cells[0, 0] = 5;
            }


            Console.WriteLine("Hello, World!");
        }
    }
}
