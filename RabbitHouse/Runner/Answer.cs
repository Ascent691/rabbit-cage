namespace Runner;

public class Answer(int caseNumber)
{
    public long Count;

    public override string ToString()
    {
        return $"Case #{caseNumber + 1}: {Count}";
    }
}