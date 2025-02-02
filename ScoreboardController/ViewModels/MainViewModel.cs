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
using ScoreboardController.Data;
using ScoreboardController.Repositories;

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
        private readonly ISoftKeyService _softKeyService;

        private ObservableCollection<SoftKey> _softKeys;
        public ObservableCollection<SoftKey> SoftKeys
        {
            get => _softKeys;
            set
            {
                _softKeys = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(TupleConverter converter, ICommandMappingService commandMappingService, ISoftKeyService softKeyService)
        {
            CommandButtonClick = new RelayCommand(HandleCommandButtonClick);
            _converter = converter;
            _commandMappingService = commandMappingService;
            _controllers = new Dictionary<string, IScoreboardElementController>();
            _softKeyService = softKeyService;

            _softKeyService.LoadSoftKeysForSet(1);
            SoftKeys = _softKeyService.SoftKeys;
            

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
            //TODO: set up keys
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

        public void HandleSoftKeyClick(string key)
        {
            var softKey = SoftKeys.FirstOrDefault(s => s.Tag == key);
            // if the softkey value is one character, add two spaces before it, if it's two characters, add one space before it.
            // this will justify the letters properly
            


            var command = new ScoreboardCommand
            {
                ElementName = softKey.Element, 
                CommandType = Enum.Parse<CommandType>(softKey.CommandType.PadLeft(3)),
                Value = softKey.Value
            };
            if (_controllers.TryGetValue(softKey.Element, out var controller))
            {
                controller.ProcessCommand(command);
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
                },
                new TextBlockDefinition
                {
                    Name = "HomeTeamNameTextBlock",
                    BindingProperty = "HomeTeamName",
                    FontSize = 36,
                    HorizontalAlignment = "Center",
                    VerticalAlignment = "Center",
                    PanelName = "HomeStatsPanel",
                    Row = 0,
                    Col = 0,
                    BackgroundColor = "#FF000000",
                    FontWeight = "Bold",
                    RowSpan = 1,
                    ColSpan = 6,
                    ValueForegroundColor = "#FFFFFFFF",
                    LabelForegroundColor = "#FF008000",
                    TextAlignment = "Center",
                    DefaultText = "HOME TEAM",
                    LabelOrientation = "Horizontal"
                },
                new TextBlockDefinition
                {
                    Name = "GuestTeamNameTextBlock",
                    BindingProperty = "GuestTeamName",
                    FontSize = 36,
                    HorizontalAlignment = "Center",
                    VerticalAlignment = "Center",
                    PanelName = "GuestStatsPanel",
                    Row = 0,
                    Col = 0,
                    BackgroundColor = "#FF000000",
                    FontWeight = "Bold",
                    RowSpan = 1,
                    ColSpan = 6,
                    ValueForegroundColor = "#FFFFFFFF",
                    LabelForegroundColor = "#FF008000",
                    TextAlignment = "Center",
                    DefaultText = "GUEST TEAM",
                    LabelOrientation = "Horizontal"
                },
                new TextBlockDefinition
                {
                    Name = "HomeScoreTextBlock",
                    BindingProperty = "HomeScore",
                    FontSize = 48,
                    HorizontalAlignment = "Center",
                    VerticalAlignment = "Center",
                    PanelName = "HomeStatsPanel",
                    Row = 1,
                    Col = 0,
                    BackgroundColor = "#FF000000",
                    FontWeight = "Bold",
                    RowSpan = 1,
                    ColSpan = 6,
                    ValueForegroundColor = "#FF0000",
                    LabelForegroundColor = "#FF008000",
                    TextAlignment = "Center",
                    DefaultText = "0",
                    LabelOrientation = "Horizontal"
                },
                new TextBlockDefinition
                {
                    Name = "GuestScoreTextBlock",
                    BindingProperty = "GuestScore",
                    FontSize = 48,
                    HorizontalAlignment = "Center",
                    VerticalAlignment = "Center",
                    PanelName = "GuestStatsPanel",
                    Row = 1,
                    Col = 0,
                    BackgroundColor = "#FF000000",
                    FontWeight = "Bold",
                    RowSpan = 1,
                    ColSpan = 6,
                    ValueForegroundColor = "#FF0000",
                    LabelForegroundColor = "#FF008000",
                    TextAlignment = "Center",
                    DefaultText = "0",
                    LabelOrientation = "Horizontal"
                },
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
}
