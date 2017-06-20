namespace KeyboardSplitter.Helpers
{
    using System;

    public class DelayedTask
    {
        private System.Timers.Timer timer;

        public DelayedTask(Action actionToDelay, TimeSpan delay)
        {
            this.timer = new System.Timers.Timer(delay.TotalMilliseconds);
            this.timer.Elapsed += (sender, e) =>
            {
                this.timer.Stop();
                this.timer.Dispose();
                this.timer = null;
                actionToDelay.Invoke();
            };

            this.timer.AutoReset = false;
        }

        public void Run()
        {
            if (this.timer == null)
            {
                return;
            }
            this.timer.Start();
        }
    }
}
