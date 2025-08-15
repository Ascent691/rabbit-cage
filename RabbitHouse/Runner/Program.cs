using System.Diagnostics;

namespace Runner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("1.in"));
            // var arrangements = new RabbitHouseParser().Parse(File.ReadAllLines("input.txt"));
            var parsingTime = stopwatch.ElapsedMilliseconds;

            for (int i = 0; i < arrangements.Length; i++)
            {
                var arrangement = arrangements[i];
                var cells = arrangement.MapInputCells((row, column, height) =>
                    new Cell(row: row, column: column, height: height));

                
                var nextPriorityQueue = new PriorityQueue<Cell, int>();
                foreach (var cell in cells)
                {
                    cell.ReferenceNeighbours(cells);
                    nextPriorityQueue.Enqueue(cell, 0 - cell.Height);
                }

                while (!arrangement.IsSafe())
                {
                    var priorityQueue = nextPriorityQueue;
                    nextPriorityQueue = new PriorityQueue<Cell, int>();

                    while (priorityQueue.TryDequeue(out var cell, out _))
                    {
                        foreach (var cellNeighbour in cell.Neighbours)
                        {
                            if (cell.Height - cellNeighbour.Height > 1)
                            {
                                cellNeighbour.Height += cell.Height - cellNeighbour.Height - 1;    
                            }
                            
                            arrangement.SetHeightAt(cellNeighbour.Row, cellNeighbour.Column, cellNeighbour.Height);
                        }

                        nextPriorityQueue.Enqueue(cell, 0 - cell.Height);    
                    }
                }

                Console.WriteLine($"Case {i+1}#: {arrangement.GetTotalAddedBlocks()}");
            }
            
            stopwatch.Stop();

            Console.WriteLine($"Parsing Time: {parsingTime}ms");
            Console.WriteLine($"Total Time: {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}

public class Cell(int row, int column, int height)
{
    public int Height { get; set; } = height;
    public int Column { get; } = column;
    public int Row { get; } = row;
    public List<Cell> Neighbours { get; } = [];

    public void ReferenceNeighbours(Cell[,] allCells)
    {
        if (Row > 0 && Row <= allCells.GetLength(0) - 1)
        {
            var north = allCells[Row - 1, Column];
            Neighbours.Add(north);
        }
        
        if (Row < allCells.GetLength(0) - 1)
        {
            var south = allCells[Row + 1, Column];
            Neighbours.Add(south);
        }

        if (Column > 0 && Column <= allCells.GetLength(1) - 1)
        {
            var west = allCells[Row, Column - 1];
            Neighbours.Add(west);
        }

        if (Column < allCells.GetLength(1) - 1)
        {
            var east = allCells[Row, Column + 1];
            Neighbours.Add(east);
        }
    }

    public void Deconstruct(out int Row, out int Column, out int Height)
    {
        Row = this.Row;
        Column = this.Column;
        Height = this.Height;
    }
}
