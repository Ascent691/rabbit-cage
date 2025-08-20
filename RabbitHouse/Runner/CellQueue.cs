namespace Runner;

public class CellQueue
{
    public Cell? Head;
    private Cell? _tail;

    public void Enqueue(Cell? cell)
    {
        if (cell == Head || cell == _tail || cell == null)
        {
            return;
        }
        
        if (cell.Next is not null) throw new InvalidOperationException("Cell already in a queue");

        if (Head is null && _tail is null)
        {
            Head = cell;
            _tail = cell;
            return;
        }
        
        if (Head is null || _tail is null) throw new InvalidOperationException("Queue in invalid state");
        
        if (Head == _tail)
        {
            Head.Next = cell;
            _tail = cell;
            return;
        }
        
        _tail.Next = cell;
        _tail = cell;
    }

    public Cell? Dequeue()
    {
        if (Head is null)
        {
            return null;
        }
        
        var popped = Head;

        if (Head == _tail)
        {
            Head = null;
            _tail = null;
        }
        else
        {
            Head = popped.Next;    
        }
        
        popped.Next = null;
        
        return popped;
    }
}