using ScoreboardController.Commands;
using System;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace ScoreboardController.Views
{
    public class PromptService
    {
        private readonly TextBlock _promptTextBlock;
        private readonly TextBlock _userInputTextBlock;
        private readonly StackPanel _promptPanel;
        private string? _currentContext = null;
        private CommandType _currentCommand = CommandType.None;

        public string? CurrentContext => _currentContext;

        // 1) Define an event so others (like MainWindow) can receive the built command
        public event Action<ScoreboardCommand> OnCommandReady;

        public PromptService(StackPanel promptPanel, TextBlock promptTextBlock, TextBlock userInputTextBlock)
        {
            _promptPanel = promptPanel;
            _promptTextBlock = promptTextBlock;
            _userInputTextBlock = userInputTextBlock;
        }

        private void ClearContext()
        {
            _currentContext = null;
        }

        public void SetPrompt(string prompt, string pattern, string element, CommandType command)
        {
            _promptPanel.Visibility = System.Windows.Visibility.Visible;
            _promptTextBlock.Text = prompt;
            _userInputTextBlock.Text = pattern.Replace('#', '_');
            _currentContext = element;
            _currentCommand = command;
        }

        public void HandleKeyPress(Keys key)
        {
            if (_currentContext is null || _currentCommand is CommandType.None) return;  // do nothing if we have no active context

            // Map the Keys enum to a single char digit, or some sentinel if not a digit
            char digitChar = '\0';

            switch (key)
            {
                case Keys.Key0: digitChar = '0'; break;
                case Keys.Key1: digitChar = '1'; break;
                case Keys.Key2: digitChar = '2'; break;
                case Keys.Key3: digitChar = '3'; break;
                case Keys.Key4: digitChar = '4'; break;
                case Keys.Key5: digitChar = '5'; break;
                case Keys.Key6: digitChar = '6'; break;
                case Keys.Key7: digitChar = '7'; break;
                case Keys.Key8: digitChar = '8'; break;
                case Keys.Key9: digitChar = '9'; break;

                case Keys.KeyBk:
                    // If you want Backspace to revert the last digit to underscore
                    // or remove it, depends on your design.  Here's an example
                    // that reverts the rightmost digit to '_'.
                    int lastDigitIndex = _userInputTextBlock.Text.LastIndexOfAny("0123456789".ToCharArray());
                    if (lastDigitIndex >= 0)
                    {
                        var arr = _userInputTextBlock.Text.ToCharArray();
                        arr[lastDigitIndex] = '_';   // revert it to underscore
                        _userInputTextBlock.Text = new string(arr);
                    }
                    return;

                case Keys.Enter:
                    // Hide prompt
                    _promptPanel.Visibility = System.Windows.Visibility.Collapsed;

                    // Parse user input
                    string input = _userInputTextBlock.Text.Trim().Replace(" ", "");

                    var cmd = new ScoreboardCommand
                    {
                        ElementName = _currentContext,  // or whichever element you're targeting
                        CommandType = _currentCommand,
                        Value = input
                    };

                    OnCommandReady?.Invoke(cmd);

                    _userInputTextBlock.Text = "";
                    ClearContext();
                    return;

                case Keys.Clear:
                    _userInputTextBlock.Text = "";
                    ClearContext();
                    return;

                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }

            // If digitChar != '\0', we want to fill in the first underscore.
            if (digitChar != '\0')
            {
                int underscoreIndex = _userInputTextBlock.Text.IndexOf('_');
                if (underscoreIndex >= 0)
                {
                    // Replace just that one underscore with digitChar
                    var chars = _userInputTextBlock.Text.ToCharArray();
                    chars[underscoreIndex] = digitChar;
                    _userInputTextBlock.Text = new string(chars);
                }
            }
        }

    }

    public enum Keys
    {
        Key0,
        Key1,
        Key2,
        Key3,
        Key4,
        Key5,
        Key6,
        Key7,
        Key8,
        Key9,
        KeyBk,
        Enter,
        Clear
    }
}
