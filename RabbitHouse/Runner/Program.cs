using System.Diagnostics;

namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Uncomment to check against the answers
            // var answers = File.ReadAllLines("1.ans");
            
            var stopwatch = Stopwatch.StartNew();
            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("1.in"));
            var parsingTimeStamp = stopwatch.Elapsed;

            for (int i = 0; i < arrangements.Length; i++)
            {
                var totalAdded = 0L;
                var cells = arrangements[i].MapInputCells((row, column, height) =>
                    new Cell(row: row, column: column, height: height));

                
                var nextPriorityQueue = new PriorityQueue<Cell, int>();
                foreach (var cell in cells)
                {
                    cell.ReferenceNeighbours(cells);
                    nextPriorityQueue.Enqueue(cell, 0 - cell.Height);
                }

                while (nextPriorityQueue.Count > 0)
                {
                    var priorityQueue = nextPriorityQueue;
                    nextPriorityQueue = new PriorityQueue<Cell, int>();

                    while (priorityQueue.TryDequeue(out var cell, out _))
                    {
                        totalAdded += cell.BalanceNeighbours();
                    }
                    
                    foreach (var cell in cells)
                    {
                        if (!cell.IsSafe())
                        {
                            nextPriorityQueue.Enqueue(cell, 0 - cell.Height);
                        }
                    }
                }

                var output = $"Case #{i+1}: {totalAdded}";
                Console.WriteLine(output);

                // Uncomment to check against the answers
                // if (output.Trim() != answers[i].Trim())
                // {
                //     Console.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!'{output}' different to '{answers[i]}'");
                // }
            }
            
            stopwatch.Stop();

            Console.WriteLine($"Parsing Timestamp: {parsingTimeStamp.ToString()}");
            Console.WriteLine($"Total Timestamp: {stopwatch.Elapsed.ToString()}");
        }
    }
}

public class Cell(int row, int column, int height)
{
    public int Height { get; private set; } = height;
    private int Column { get; } = column;
    private int Row { get; } = row;
    private Cell? North { get; set; }
    private Cell? East { get; set; }
    private Cell? South { get; set; }
    private Cell? West { get; set; }

    public bool IsSafe()
    {
        var northSafe = North == null ||  Height - North.Height <= 1;
        var eastSafe = East == null || Height - East.Height <= 1;
        var southSafe = South == null || Height - South.Height <= 1;
        var westSafe = West == null || Height - West.Height <= 1;
        
        return northSafe && eastSafe &&  southSafe && westSafe;
    }

    public int BalanceNeighbours()
    {
        var totalAdded = 0;

        if (North is not null && Height - North.Height > 1)
        {
            var amountToAdd = Height - North.Height - 1;
            North.Height += amountToAdd;
            totalAdded += amountToAdd;
        }
        
        if (East is not null && Height - East.Height > 1)
        {
            var amountToAdd = Height - East.Height - 1;
            East.Height += amountToAdd;
            totalAdded += amountToAdd;
        }
        
        if (South is not null && Height - South.Height > 1)
        {
            var amountToAdd = Height - South.Height - 1;
            South.Height += amountToAdd;
            totalAdded += amountToAdd;
        }
        
        if (West is not null && Height - West.Height > 1)
        {
            var amountToAdd = Height - West.Height - 1;
            West.Height += amountToAdd;
            totalAdded += amountToAdd;
        }
        
        return totalAdded;
    }

    public void ReferenceNeighbours(Cell[,] allCells)
    {
        if (Row > 0 && Row <= allCells.GetLength(0) - 1)
        {
            North = allCells[Row - 1, Column];
        }
        
        if (Row < allCells.GetLength(0) - 1)
        {
            South = allCells[Row + 1, Column];
        }

        if (Column > 0 && Column <= allCells.GetLength(1) - 1)
        {
            West = allCells[Row, Column - 1];
        }

        if (Column < allCells.GetLength(1) - 1)
        {
            East = allCells[Row, Column + 1];
        }
    }
}
