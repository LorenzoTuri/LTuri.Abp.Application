using System;

namespace LTuri.Abp.Application.Events.EventBus
{
    /// <summary>
    /// Implementation for a multi-disposable disposable.
    /// It's recognized as a normal disposable, but contains other disposables
    /// </summary>
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
