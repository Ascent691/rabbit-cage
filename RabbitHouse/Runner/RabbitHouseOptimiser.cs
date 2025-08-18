namespace Runner;

public class RabbitHouseOptimiser
{
    private RabbitHouseArrangement _arrangement = null!;
    private int _rows;
    private int _cols;
    private IEnumerable<Jump> _unsafeJumps = [];
    private HashSet<Jump> _changedJumps = [];

    public void Optimise(RabbitHouseArrangement arr)
    {
        _arrangement = arr;
        _rows = _arrangement.TotalRows;
        _cols = _arrangement.TotalColumns;

        _unsafeJumps = GetAllUnsafeJumps();

        do
        {
            _changedJumps = [];

            foreach (var unsafeJump in _unsafeJumps)
            {
                MakeJumpSafe(unsafeJump);
            }

            _unsafeJumps = _changedJumps.Where(JumpIsUnsafe);
        } while (_changedJumps.Count > 0);
    }

    private record Cell(int Row, int Col);

    private record Jump(Cell A, Cell B);

    private IEnumerable<Jump> GetAllUnsafeJumps()
    {
        for (var y = 0; y < _rows; y++)
        {
            for (var x = 0; x < _cols; x++)
            {
                if (x != _cols - 1)
                {
                    var h = Math.Abs(_arrangement[y, x] - _arrangement[y, x + 1]);
                    if (h > 1)
                        yield return new Jump(new Cell(y, x), new Cell(y, x + 1));
                }

                if (y != _rows - 1)
                {
                    var h = Math.Abs(_arrangement[y, x] - _arrangement[y + 1, x]);
                    if (h > 1)
                        yield return new Jump(new Cell(y, x), new Cell(y + 1, x));
                }
            }
        }
    }

    private bool JumpIsUnsafe(Jump jump)
    {
        var h = Math.Abs(_arrangement[jump.A.Row, jump.A.Col] -
                         _arrangement[jump.B.Row, jump.B.Col]);
        return h > 1;
    }

    private void MakeJumpSafe(Jump unsafeJump)
    {
        var hA = _arrangement[unsafeJump.A.Row, unsafeJump.A.Col];
        var hB = _arrangement[unsafeJump.B.Row, unsafeJump.B.Col];
        if (hA > hB)
        {
            _arrangement.SetHeightAt(unsafeJump.B.Row, unsafeJump.B.Col, hA - 1);
            _changedJumps.UnionWith(GetAffectedJumps(unsafeJump.B));
        }
        else
        {
            _arrangement.SetHeightAt(unsafeJump.A.Row, unsafeJump.A.Col, hB - 1);
            _changedJumps.UnionWith(GetAffectedJumps(unsafeJump.A));
        }
    }

    private IEnumerable<Jump> GetAffectedJumps(Cell cell)
    {
        if (cell.Row + 1 < _rows)
            yield return new Jump(cell, cell with { Row = cell.Row + 1 });
        if (cell.Row - 1 >= 0)
            yield return new Jump(cell with { Row = cell.Row - 1 }, cell);
        if (cell.Col + 1 < _cols)
            yield return new Jump(cell, cell with { Col = cell.Col + 1 });
        if (cell.Col - 1 >= 0)
            yield return new Jump(cell with { Col = cell.Col - 1 }, cell);
    }
}