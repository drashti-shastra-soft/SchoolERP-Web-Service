using SchoolERPWebService.Interfaces;

namespace SchoolERPWebService.Services
{
    public class EmailQueueSignal : IEmailQueueSignal
    {
        private readonly SemaphoreSlim _sem = new(0, 1);

        // called after rows are put in EmailQueue -> wakes the worker immediately
        public void Notify()
        {
            try { _sem.Release(); }
            catch (SemaphoreFullException) { /* already signalled, ignore */ }
        }

        // worker waits here; returns early if Notify() is called, else after timeout
        public async Task WaitAsync(TimeSpan timeout, CancellationToken ct)
            => await _sem.WaitAsync(timeout, ct);
    }
}
