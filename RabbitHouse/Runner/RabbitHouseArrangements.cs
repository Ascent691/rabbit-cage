namespace Runner;

public class RabbitHouseArrangements(SemaphoreSlim amountOfCasesRead, SemaphoreSlim caseDataRead)
{
    public RabbitHouseArrangement[]? Data;

    public void WaitForAmountOfCasesToBeRead()
    {
        amountOfCasesRead.Wait();
    }

    public void WaitCaseDataToBeRead()
    {
        caseDataRead.Wait();
    }
}