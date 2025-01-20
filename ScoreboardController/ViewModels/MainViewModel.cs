using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using ScoreboardController.Commands;
using ScoreboardController.Controllers;
using ScoreboardController.Helpers;
using ScoreboardController.Models;
using ScoreboardController.Services;
using ScoreboardController.Views;
using System.Collections.ObjectModel;

namespace ScoreboardController.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand CommandButtonClick { get; }
        public ICommand ResetTimeCommand { get; }
        private readonly TupleConverter _converter;
        private readonly ICommandMappingService _commandMappingService;
        private PromptService _promptService;
        private Dictionary<string, IScoreboardElementController> _controllers
            = new Dictionary<string, IScoreboardElementController>();
        private readonly ISoftKeyRepository _softKeyRepository;

        private ObservableCollection<SoftKeyDefinition> _softKeys;
        public ObservableCollection<SoftKeyDefinition> SoftKeys
        {
            get => _softKeys;
            set
            {
                _softKeys = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(TupleConverter converter, ICommandMappingService commandMappingService, ISoftKeyRepository softKeyRepository)
        {
            CommandButtonClick = new RelayCommand(HandleCommandButtonClick);
            _converter = converter;
            _commandMappingService = commandMappingService;
            _controllers = new Dictionary<string, IScoreboardElementController>();
            _softKeyRepository = softKeyRepository;

            SoftKeys = new ObservableCollection<SoftKeyDefinition>(_softKeyRepository.GetSoftKeys());
            Console.WriteLine($"Loaded {SoftKeys.Count} softkeys.");
            InitializeSoftKeys();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Dictionary<string, IScoreboardElementController> Controllers
        {
            get => _controllers;
            set
            {
                _controllers = value;
                OnPropertyChanged();
            }
        }

        public void SetPromptService(PromptService promptService)
        {
            _promptService = promptService;
        }


        private void InitializeSoftKeys()
        {
            // Populate softkey definitions
            SoftKeys = new ObservableCollection<SoftKeyDefinition>();

            for (int i = 0; i < 40; i++)
            {
                SoftKeys.Add(new SoftKeyDefinition
                {
                    Content = $"Key {(i <= 9 ? "0" : "")}{i + 1}",
                    Tag = $"SoftKey{(i <= 9 ? "0" : "")}{i + 1}",
                    Command = new RelayCommand(o => HandleSoftKeyClick(o))
                });
            }
        }

        private void HandleSoftKeyClick(object? parameter)
        {
            if (parameter is string tag)
            {
                // Example logic for softkey click handling
                System.Diagnostics.Debug.WriteLine($"SoftKey clicked: {tag}");
            }
        }

        public void HandleCommandButtonClick(object parameters)
        {
            if (parameters is Tuple<object, RoutedEventArgs> tuple)
            {
                if (tuple.Item1 is Button button)
                {
                    var actionName = button.Tag?.ToString();
                    if (string.IsNullOrEmpty(actionName))
                    {
                        MessageBox.Show("Action name not specified for the button.");
                        return;
                    }

                    try
                    {
                        var mapping = _commandMappingService.GetMapping(actionName, false);

                        if (!mapping.PromptText.IsNullOrEmpty())
                        {
                            _promptService.SetPrompt(
                                mapping.PromptText,
                                mapping.InputMask,
                                mapping.ElementName,
                                mapping.CommandType);
                        }
                        else
                        {
                            var command = new ScoreboardCommand
                            {
                                ElementName = mapping.ElementName,
                                CommandType = mapping.CommandType,
                                Value = mapping.CommandValue
                            };
                            if (_controllers.TryGetValue(mapping.ElementName, out var controller))
                            {
                                controller.ProcessCommand(command);
                            }
                            else
                            {
                                MessageBox.Show($"Controller for element '{mapping.ElementName}' not found.");
                            }
                        }
                    }
                    catch (KeyNotFoundException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error executing command: {ex.Message}");
                    }
                }
            }
        }

        public List<TextBlockDefinition> LoadTextBlockDefinitions()
        {
            return new List<TextBlockDefinition>
            {
                new TextBlockDefinition
                {
                    Name = "ClockTextBlock",
                    BindingProperty = "GameClock",
                    FontSize = 48,
                    HorizontalAlignment = "Center",
                    VerticalAlignment = "Center",
                    PanelName = "GameStatsPanel",
                    Row = 0,
                    Col = 0,
                    BackgroundColor = "#FF000000",
                    FontWeight = "Bold",
                    RowSpan = 1,
                    ColSpan = 6,
                    ValueForegroundColor = "#FFFFFF00",
                    TextAlignment = "Center",
                    DefaultText = "99:99.9"
                },
                new TextBlockDefinition
                {
                    Name = "PeriodTextBlock",
                    BindingProperty = "Period",
                    FontSize = 36,
                    HorizontalAlignment = "Center",
                    VerticalAlignment = "Center",
                    PanelName = "GameStatsPanel",
                    Row = 1,
                    Col = 0,
                    BackgroundColor = "#FF000000",
                    FontWeight = "Bold",
                    RowSpan = 1,
                    ColSpan = 6,
                    ValueForegroundColor = "#FFFFFFFF",
                    LabelForegroundColor = "#FF008000",
                    LabelText = "PERIOD",
                    TextAlignment = "Center",
                    DefaultText = "0",
                    LabelOrientation = "Horizontal"
                }
            };
        }

        /// <summary>
        /// Mock method to load scoreboard elements. Replace with actual DB calls.
        /// </summary>
        /// <returns>List of tuples containing element name and type.</returns>
        public List<(string Name, string Type)> LoadScoreboardElements()
        {
            return new List<(string Name, string Type)>
            {
                ("GameClock", "Clock"),
                ("Period", "Counter"),
                ("HomeScore", "Counter"),
                ("GuestScore", "Counter")
            };
        }

        /// <summary>
        /// Generic event handler for command buttons.
        /// </summary>
        private void CommandButton_Click(object sender, RoutedEventArgs e)
        {
            HandleCommandButtonClick(new Tuple<object, RoutedEventArgs>(sender, e));
        }

    }





    public interface ISoftKeyRepository
    {
        List<SoftKeyDefinition> GetSoftKeys();
    }


    public class MockSoftKeyRepository : ISoftKeyRepository
    {
        public List<SoftKeyDefinition> GetSoftKeys()
        {
            var softKeys = new List<SoftKeyDefinition>();

            for (int i = 0; i < 40; i++)
            {
                softKeys.Add(new SoftKeyDefinition
                {
                    Content = $"SoftKey {i}",
                    Tag = $"SoftKey{i}",
                    Command = new RelayCommand(parameter => HandleSoftKeyClick(parameter))
                });
            }

            return softKeys;
        }

        private void HandleSoftKeyClick(object? parameter)
        {
            if (parameter is string tag)
            {
                Console.WriteLine($"SoftKey clicked: {tag}");
            }
        }
    }

}
