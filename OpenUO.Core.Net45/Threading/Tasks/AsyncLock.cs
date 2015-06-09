using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenUO.Core.Threading.Tasks
{
    public class AsyncLock
    {
        private readonly Task<Releaser> _releaser;
        private readonly AsyncSemaphore _semaphore;

        public AsyncLock()
        {
            _semaphore = new AsyncSemaphore(1);
            _releaser = Task.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            var wait = _semaphore.WaitAsync();

            return wait.IsCompleted ?
                _releaser :
                wait.ContinueWith(
                    (_, state) => new Releaser((AsyncLock) state),
                    this,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _lock;

            internal Releaser(AsyncLock @lock)
            {
                _lock = @lock;
            }

            public void Dispose()
            {
                _lock?._semaphore.Release();
            }
        }
    }
}