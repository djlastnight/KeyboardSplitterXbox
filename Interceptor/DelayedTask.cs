namespace Interceptor
{
    using System;
    using System.Timers;

    public sealed class DelayedTask : IDisposable
    {
        private Timer timer;

        public DelayedTask(Action actionToDelay, TimeSpan delay)
        {
            this.timer = new Timer(delay.TotalMilliseconds);
            this.timer.Elapsed += (sender, e) =>
            {
                this.timer.Stop();
                this.timer.Dispose();
                this.timer = null;
                actionToDelay.Invoke();
            };

            this.timer.AutoReset = false;
        }

        public void Dispose()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
            }
        }

        public void Run()
        {
            if (this.timer != null)
            {
                this.timer.Start();
            }
        }
    }
}