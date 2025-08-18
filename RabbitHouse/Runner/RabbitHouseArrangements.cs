namespace Runner;

public class RabbitHouseArrangements(SemaphoreSlim amountDataReadSemaphore, SemaphoreSlim dataAvailableSemaphore)
{
    public RabbitHouseArrangement[]? Data;
        
    public readonly SemaphoreSlim AmountDataReadSemaphore = amountDataReadSemaphore;
    public readonly SemaphoreSlim DataAvailableSemaphore = dataAvailableSemaphore;
}