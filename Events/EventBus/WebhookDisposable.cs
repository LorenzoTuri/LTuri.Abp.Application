using System;

namespace LTuri.Abp.Application.Events.EventBus
{
    /// <summary>
    /// Empty implementation of a disposable. 
    /// Needed only for code compatibility reasons
    /// </summary>
    public class WebhookDisposable : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
