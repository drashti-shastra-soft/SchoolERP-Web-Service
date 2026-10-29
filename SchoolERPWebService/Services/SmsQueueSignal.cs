using SchoolERPWebService.Interfaces;

namespace SchoolERPWebService.Services
{
    public class SmsQueueSignal: ISmsQueueSignal
    {
        private readonly SemaphoreSlim _sem = new(0, 1);

        public void Notify()
        {
            try { _sem.Release(); }
            catch (SemaphoreFullException) { /* already signalled */ }
        }

        public async Task WaitAsync(TimeSpan timeout, CancellationToken ct)
            => await _sem.WaitAsync(timeout, ct);
    }
}
