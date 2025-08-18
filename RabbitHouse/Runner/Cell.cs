namespace Runner;

public class Cell(int row, int column, int height)
{
    private Cell? _north;
    private Cell? _east;
    private Cell? _south;
    private Cell? _west;

    public int Height = height;
    public Cell? Next;

    private bool IsSafe()
    {
        return Height == 0 ||
               (_north == null ||  Height - _north.Height <= 1) && 
               (_east == null || Height - _east.Height <= 1) &&  
               (_south == null || Height - _south.Height <= 1) && 
               (_west == null || Height - _west.Height <= 1);
    }

    public int MakeSafe(CellQueue queue)
    {
        var totalAdded = 0;

        if (_north is not null)
        {
            var additionalHeightNeededToMakeCellSafe = Height - _north.Height - 1;
            if (additionalHeightNeededToMakeCellSafe >= 1)
            {
                _north.Height += additionalHeightNeededToMakeCellSafe;
                totalAdded += additionalHeightNeededToMakeCellSafe;
            }

            if (_north.Next is null && _north.IsSafe() is false)
            {
                queue.Enqueue(_north);
            }
        }
        
        if (_east is not null)
        {
            var additionalHeightNeededToMakeCellSafe = Height - _east.Height - 1;
            if (additionalHeightNeededToMakeCellSafe >= 1)
            {
                _east.Height += additionalHeightNeededToMakeCellSafe;
                totalAdded += additionalHeightNeededToMakeCellSafe;
            }

            if (_east.Next is null && _east.IsSafe() is false)
            {
                queue.Enqueue(_east);
            }
        }
        
        if (_south is not null)
        {
            var additionalHeightNeededToMakeCellSafe = Height - _south.Height - 1;
            if (additionalHeightNeededToMakeCellSafe >= 1)
            {
                _south.Height += additionalHeightNeededToMakeCellSafe;
                totalAdded += additionalHeightNeededToMakeCellSafe;
            }

            if (_south.Next is null && _south.IsSafe() is false)
            {
                queue.Enqueue(_south);
            }
        }
        
        if (_west is not null)
        {
            var additionalHeightNeededToMakeCellSafe = Height - _west.Height - 1;
            if (additionalHeightNeededToMakeCellSafe >= 1)
            {
                _west.Height += additionalHeightNeededToMakeCellSafe;
                totalAdded += additionalHeightNeededToMakeCellSafe;
            }

            if (_west.Next is null && _west.IsSafe() is false)
            {
                queue.Enqueue(_west);
            }
        }
        
        return totalAdded;
    }

    public void ReferenceNeighbours(Cell[,] allCells, int numberOfRows, int numberOfColumns)
    {
        if (row > 0 && row <= numberOfRows - 1)
        {
            _north = allCells[row - 1, column];
        }
        
        if (row < numberOfRows - 1)
        {
            _south = allCells[row + 1, column];
        }

        if (column > 0 && column <= numberOfColumns - 1)
        {
            _west = allCells[row, column - 1];
        }

        if (column < numberOfColumns - 1)
        {
            _east = allCells[row, column + 1];
        }
    }
}