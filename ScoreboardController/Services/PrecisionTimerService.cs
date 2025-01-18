using System.Diagnostics;

namespace ScoreboardController.Services
{
    public interface ITimerService
    {
        event EventHandler<TimeSpan> OnTick;
        void StartCountdown(TimeSpan duration);
        void Stop();
        TimeSpan GetRemainingTime();
    }

    public class PrecisionTimerService : ITimerService, IDisposable
    {
        private Stopwatch _stopwatch;
        private TimeSpan _duration;
        private bool _running;
        private TimeSpan _timeLeft;
        private CancellationTokenSource _cts;
        private Task _task;

        public event EventHandler<TimeSpan> OnTick;

        public PrecisionTimerService()
        {
            _stopwatch = new Stopwatch();
        }

        public void StartCountdown(TimeSpan duration)
        {
            _duration = duration;
            _timeLeft = duration;
            _stopwatch.Reset();
            _stopwatch.Start();
            _running = true;

            // Start background update loop if not started
            if (_task == null || _task.IsCompleted)
            {
                _cts = new CancellationTokenSource();
                _task = Task.Run(() => UpdateLoop(_cts.Token));
            }
        }

        public void Stop()
        {
            if (_running)
            {
                _stopwatch.Stop();
                _timeLeft = GetRemainingTime();
                _running = false;
            }
        }

        public TimeSpan GetRemainingTime()
        {
            if (_running)
            {
                var elapsed = _stopwatch.Elapsed;
                var left = _duration - elapsed;
                if (left < TimeSpan.Zero) left = TimeSpan.Zero;
                return left;
            }
            return _timeLeft;
        }

        private async Task UpdateLoop(CancellationToken token)
        {
            var interval = TimeSpan.FromMilliseconds(100);
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_running)
                    {
                        var current = GetRemainingTime();
                        OnTick?.Invoke(this, current);
                        if (current <= TimeSpan.Zero)
                        {
                            _running = false;
                        }
                    }
                    await Task.Delay(interval, token);
                }
            }
            catch (TaskCanceledException) { /* normal shutdown */ }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _stopwatch?.Stop();
        }
    }
}
