namespace CCMS.Application.Interfaces;

public interface IBatchService
{
    Task RunAsync(bool isManualRun);
}