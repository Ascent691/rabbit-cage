namespace Runner;

public class Cell(
    int row,
    int column,
    int height,
    Cell[,] cells)
{
    private Cell? _north;
    private Cell? _east;
    private Cell? _south;
    private Cell? _west;
    private bool _neighboursHaveBeenReferenced;

    private int _height = height;
    public Cell? Next;

    private bool IsSafe()
    {
        if (!_neighboursHaveBeenReferenced)
        {
            ReferenceNeighbours();
        }

        return _height <= 1 ||
               IsNeighbourSafeToJumpTo(_north) &&
               IsNeighbourSafeToJumpTo(_east) &&
               IsNeighbourSafeToJumpTo(_south) &&
               IsNeighbourSafeToJumpTo(_west);
    }

    private bool IsNeighbourSafeToJumpTo(Cell? neighbour)
    {
        return neighbour == null || _height - neighbour._height <= 1;
    }

    public int MakeSafe(CellQueue queue)
    {
        if (!_neighboursHaveBeenReferenced)
        {
            ReferenceNeighbours();
        }

        return MakeNeighbourSafeToJumpTo(queue, _north) +
               MakeNeighbourSafeToJumpTo(queue, _east) +
               MakeNeighbourSafeToJumpTo(queue, _south) +
               MakeNeighbourSafeToJumpTo(queue, _west);
    }

    private int MakeNeighbourSafeToJumpTo(CellQueue queue, Cell? neighbour)
    {
        if (neighbour is null) return 0;
        
        var amountAddedToNeighbour = 0;
        
        var additionalHeightNeededToMakeNeighbourSafeToJumpTo = _height - neighbour._height - 1;
        if (additionalHeightNeededToMakeNeighbourSafeToJumpTo >= 1)
        {
            neighbour._height += additionalHeightNeededToMakeNeighbourSafeToJumpTo;
            amountAddedToNeighbour = additionalHeightNeededToMakeNeighbourSafeToJumpTo;
        }

        if (neighbour.Next is null && !neighbour.IsSafe())
        {
            queue.Enqueue(neighbour);
        }

        return amountAddedToNeighbour;
    }

    private void ReferenceNeighbours()
    {
        _neighboursHaveBeenReferenced = true;

        var totalRows = cells.GetLength(0);
        var totalColumns = cells.GetLength(1);

        if (row > 0 && row <= totalRows - 1)
        {
            _north = cells[row - 1, column];
        }

        if (row < totalRows - 1)
        {
            _south = cells[row + 1, column];
        }

        if (column > 0 && column <= totalColumns - 1)
        {
            _west = cells[row, column - 1];
        }

        if (column < totalColumns - 1)
        {
            _east = cells[row, column + 1];
        }
    }
}