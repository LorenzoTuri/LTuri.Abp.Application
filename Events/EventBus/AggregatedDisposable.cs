using System;

namespace LTuri.Abp.Application.Events.EventBus
{
    public class AggregatedDisposable : IDisposable
    {
        protected IDisposable[] disposables;
        public AggregatedDisposable(IDisposable[] disposables)
        {
            this.disposables = disposables;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
