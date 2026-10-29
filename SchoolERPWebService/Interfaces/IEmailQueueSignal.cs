namespace SchoolERPWebService.Interfaces
{
    public interface IEmailQueueSignal
    {
        void Notify();
        Task WaitAsync(TimeSpan timeout, CancellationToken ct);
    }
}
