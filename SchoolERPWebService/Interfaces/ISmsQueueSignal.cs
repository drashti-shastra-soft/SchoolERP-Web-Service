namespace SchoolERPWebService.Interfaces
{
    public interface ISmsQueueSignal
    {
        void Notify();
        Task WaitAsync(TimeSpan timeout, CancellationToken ct);
    }
}
