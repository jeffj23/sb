using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using ScoreboardController.Controllers;
using ScoreboardController.Commands;
using ScoreboardController.Services;
using Newtonsoft.Json;
using ScoreboardController.Data;

namespace ScoreboardController.Elements
{
    public class MainClockController : IScoreboardElementController
    {
        public string ElementName { get; private set; }
        public string ElementType { get; private set; } = "Clock";

        public string ElementValue
        {
            get => _currentClock;
            private set
            {
                if (_currentClock != value)
                {
                    _currentClock = value;
                    OnPropertyChanged();
                }
            }
        }

        public event Action<string, string> OnStateChanged;
        public event Action<string> OnMessage;

        private readonly ITimerService _timerService;

        private string _currentClock = "00:00.0";
        private string _defaultClock = "00:00.0";
        private bool _isRunning = false;
        private IJsonMessenger _messenger;

        public MainClockController(string elementName, ITimerService timerService, IJsonMessenger messenger)
        {
            ElementName = elementName;
            _timerService = timerService;
            _timerService.OnTick += TimerService_OnTick;
            _messenger = messenger;
        }

        public void ProcessCommand(ScoreboardCommand command)
        {
            if (command == null || command.CommandType == CommandType.None)
                return;

            switch (command.CommandType)
            {
                case CommandType.Start:
                    TimeIn();
                    break;

                case CommandType.Stop:
                    TimeOut();
                    break;

                case CommandType.Set:
                    SetTime(command.Value);
                    break;

                case CommandType.SetDefault:
                    SetDefaultTime(command.Value);
                    break;

                case CommandType.Reset:
                    ResetTime();
                    break;

                // For a clock, we typically ignore Increment/Decrement 
                // or handle them if you want to allow +/- 30 seconds, etc.
                case CommandType.Increment:
                    // optional if we want a "bump up" logic
                    break;
                case CommandType.Decrement:
                    // optional if we want a "bump down" logic
                    break;
            }
        }

        private void SetTime(string input)
        {
            var duration = ParseClockInput(input);
            ElementValue = FormatClock(duration);
            OnStateChanged?.Invoke("GameClock", _currentClock);
            PublishMessage("SetTime", input);
        }

        private void SetDefaultTime(string input)
        {
            _defaultClock = FormatClock(ParseClockInput(input));
            PublishMessage("SetDefaultTime", input);
        }

        private void ResetTime()
        {
            var duration = ParseClockInput(_defaultClock);
            ElementValue = FormatClock(duration);
            PublishMessage("ResetTime", _defaultClock);
        }

        private void TimeIn()
        {
            // resume from _currentClock if we have one
            var dur = ParseClockInput(_currentClock);
            _timerService.StartCountdown(dur);
            _isRunning = true;
            PublishMessage("TimeIn", null);
        }

        private void TimeOut()
        {
            _timerService.Stop();
            _isRunning = false;
            PublishMessage("TimeOut", null);
        }

        // Timer => update clock
        private void TimerService_OnTick(object sender, TimeSpan timeLeft)
        {
            ElementValue = FormatClock(timeLeft);
            OnStateChanged?.Invoke("GameClock", _currentClock);
            if (timeLeft <= TimeSpan.Zero && _isRunning)
            {
                // clock reached zero
                _isRunning = false;
                // optional message
            }
        }

        // Helpers
        private void StartCountdown(TimeSpan duration)
        {
            _timerService.Stop();
            _timerService.StartCountdown(duration);
            _isRunning = true;
        }

        private void PublishMessage(string command, string val)
        {
            var msg = new
            {
                Element = ElementName,
                Command = command,
                Value = val
            };
            OnMessage?.Invoke(JsonConvert.SerializeObject(msg));

        }

        private TimeSpan ParseClockInput(string input)
        {
            input = input.Replace("_", "0");
            if (input.Length == 6) input = "0" + input;

            var minutes = int.Parse(input.Substring(0, 2));
            var seconds = int.Parse(input.Substring(3, 2));
            var tenths = int.Parse(input.Substring(6, 1));
            return new TimeSpan(0, minutes, seconds).Add(TimeSpan.FromMilliseconds(tenths * 100));
        }

        private string FormatClock(TimeSpan ts)
        {
            int m = ts.Minutes;
            int s = ts.Seconds;
            int t = ts.Milliseconds / 100;
            return $"{m}:{s:D2}.{t}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _messenger.SendMessageAsync(new { Element = ElementName, Value = ElementValue });
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
