namespace Runner;

public class RabbitHouseArrangements(SemaphoreSlim totalNumberOfArrangementsRead, SemaphoreSlim arrangementDataRead)
{
    public RabbitHouseArrangement[]? Data;

    public void WaitForTotalNumberOfArrangementsToBeRead()
    {
        totalNumberOfArrangementsRead.Wait();
    }

    public void WaitArrangementDataToBeRead()
    {
        arrangementDataRead.Wait();
    }
}