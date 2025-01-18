using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using ScoreboardController.Commands;
using ScoreboardController.Controllers;
using ScoreboardController.Data;
using ScoreboardController.Services;

namespace ScoreboardController.Elements
{
    public class CounterController : IScoreboardElementController
    {
        public string ElementName { get; private set; }
        public string ElementType { get; private set; } = "Counter";

        public event Action<string, string> OnStateChanged;
        public event Action<string> OnMessage;

        private int _count;

        public string ElementValue => _count.ToString();
        private readonly IJsonMessenger _messenger;

        public CounterController(string elementName, IJsonMessenger messenger)
        {
            ElementName = elementName;
            _messenger = messenger;
        }

        public void ProcessCommand(ScoreboardCommand command)
        {
            if (command == null || command.CommandType == CommandType.None)
                return;

            switch (command.CommandType)
            {
                case CommandType.Set:
                    SetCounter(command.Value);
                    break;
                case CommandType.Increment:
                    Increment(command.Value);
                    break;
                case CommandType.Decrement:
                    Decrement(command.Value);
                    break;
            }
        }

        private void SetCounter(string val)
        {
            if (int.TryParse(val, out int newVal))
            {
                _count = (newVal < 0) ? 0 : newVal;
                OnPropertyChanged(nameof(ElementValue)); // Notify UI
            }
            OnPropertyChanged(nameof(ElementValue)); // Notify UI
            PublishState();
            PublishMessage("Set", val);
        }

        private void Increment(string val)
        {
            int amt = 1;
            if (int.TryParse(val, out int parsed) && parsed > 0)
                amt = parsed;

            _count += amt;
            OnPropertyChanged(nameof(ElementValue)); // Notify UI
            PublishState();
            PublishMessage("Increment", amt.ToString());
        }

        private void Decrement(string val)
        {
            int amt = 1;
            if (int.TryParse(val, out int parsed) && parsed > 0)
                amt = parsed;

            _count -= amt;
            if (_count < 0) _count = 0;
            OnPropertyChanged(nameof(ElementValue)); // Notify UI
            PublishState();
            PublishMessage("Decrement", amt.ToString());
        }

        private void PublishState()
        {
            OnStateChanged?.Invoke("CounterValue", _count.ToString());
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
