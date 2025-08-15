namespace Runner;

public class Cell(int row, int column, int height)
{
    private Cell? _north;
    private Cell? _east;
    private Cell? _south;
    private Cell? _west;

    public int Height = height;
    
    public bool IsSafe()
    {
        return (_north == null ||  Height - _north.Height <= 1) && 
               (_east == null || Height - _east.Height <= 1) &&  
               (_south == null || Height - _south.Height <= 1) && 
               (_west == null || Height - _west.Height <= 1);
    }

    public int BalanceNeighbours()
    {
        var totalAdded = 0;

        if (_north is not null && Height - _north.Height > 1)
        {
            var amountToAdd = Height - _north.Height - 1;
            _north.Height += amountToAdd;
            totalAdded += amountToAdd;
        }
        
        if (_east is not null && Height - _east.Height > 1)
        {
            var amountToAdd = Height - _east.Height - 1;
            _east.Height += amountToAdd;
            totalAdded += amountToAdd;
        }
        
        if (_south is not null && Height - _south.Height > 1)
        {
            var amountToAdd = Height - _south.Height - 1;
            _south.Height += amountToAdd;
            totalAdded += amountToAdd;
        }
        
        if (_west is not null && Height - _west.Height > 1)
        {
            var amountToAdd = Height - _west.Height - 1;
            _west.Height += amountToAdd;
            totalAdded += amountToAdd;
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