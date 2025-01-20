using System.Windows;
using System.Windows.Controls;
using ScoreboardController.Commands;
using ScoreboardController.Data;
using ScoreboardController.Factories;
using ScoreboardController.Services;
using ScoreboardController.Controllers;
using System.Windows.Data;
using Microsoft.IdentityModel.Tokens;
using ScoreboardController.Helpers;
using ScoreboardController.ViewModels;

namespace ScoreboardController.Views
{
    public partial class MainWindow : Window
    {
        // Store controllers keyed by element name


        private readonly PromptService _promptService;
        private readonly ITimerService _sharedTimerService;
        private readonly IJsonMessenger _messenger;
        private readonly ICommandMappingService _commandMappingService;
        private readonly MainViewModel _viewModel;

        public MainWindow(ITimerService timerService, IJsonMessenger messenger, MainViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;
            
            //InitializeSoftKeys();
            _messenger = messenger;
            _sharedTimerService = timerService;
            _promptService = new PromptService(InputPromptPanel, PromptTextBlock, UserInputTextBlock);
            _promptService.OnCommandReady += PromptService_HandleCommand;

            _viewModel.SetPromptService(_promptService);

            // 1) Load or stub scoreboard elements (Name, Type). 
            //    e.g., from DB => ScoreboardElements table
            var elements = _viewModel.LoadScoreboardElements();

            // 2) Create controllers via factory and store in _controllers
            foreach (var elem in elements)
            {
                IScoreboardElementController controller = ScoreboardControllerFactory.CreateController(
                    elem.Name,
                    elem.Type,
                    elem.Type == "Clock" ? _sharedTimerService : null,
                    messenger);

                controller.OnMessage += (json) =>
                {
                    // Log or send JSON out to scoreboard (via socket, etc.)
                    // For demonstration, let's just debug-print:
                    System.Diagnostics.Debug.WriteLine($"SEND JSON => {json}");
                };

                // Add to dictionary
                _viewModel.Controllers[elem.Name] = controller;
            }

            InitializeDynamicElements();
            InitializeCommandButtons();
            Task.Run(InitializeMessenger);
        }

        private async Task InitializeMessenger()
        {
            await _messenger.ConnectAsync("ws://localhost:5000");
            await _messenger.SendMessageAsync(new { Element = "GameClock", Value = "20:00" });
        }

        /// <summary>
        /// Initializes buttons that are mapped to scoreboard commands with the generic handler.
        /// </summary>
        private void InitializeCommandButtons()
        {
            // Example: Assuming you have buttons defined in XAML with names like SetTimeButton, ResetTimeButton, etc.
            // Assign their Tag to match ActionName in the mapping and assign the generic handler.

            var commandButtons = new List<Button>
            {
                SetTimeButton,
                ResetTimeButton,
                TimeInButton,
                TimeOutButton,
                SetDefaultTimeButton,
                SetPeriodButton
                // Add more buttons as needed
            };

            foreach (var btn in commandButtons)
            {
                // Ensure the Tag matches the ActionName in the mapping
                // e.g., btn.Tag = "SetTime", "ResetTime", etc.
                //btn.Click -= SpecificEventHandlers; // Remove existing handlers if any
                btn.Click += CommandButton_Click; // Assign the generic handler
            }
        }

        /// <summary>
        /// Generic event handler for command buttons.
        /// </summary>
        private void CommandButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.HandleCommandButtonClick(new Tuple<object, RoutedEventArgs>(sender,e));
        }

        private void SoftKey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var tag = btn.Tag?.ToString(); // e.g., "SoftKey01"
                MessageBox.Show($"You pressed {tag}");
            }
        }

        private void InitializeDynamicElements()
        {
            var definitions = _viewModel.LoadTextBlockDefinitions(); // Fetch from DB or a mock method

            foreach (var def in definitions)
            {
                // Determine the target panel
                Grid? targetPanel = def.PanelName switch
                {
                    "HomeMisc" => HomeMiscGrid,
                    "HomeStatsPanel" => HomeStatsGrid,
                    "GameStatsPanel" => GameStatsGrid,
                    "GuestStatsPanel" => GuestStatsGrid,
                    "GuestMisc" => GuestMiscGrid,
                    _ => null
                };

                if (targetPanel == null)
                    continue;

                // Check if label properties are provided
                bool hasLabel = !string.IsNullOrWhiteSpace(def.LabelText) &&
                                !string.IsNullOrWhiteSpace(def.LabelForegroundColor);

                if (hasLabel)
                {
                    // Determine the orientation based on LabelOrientation property
                    Orientation stackOrientation = Orientation.Vertical; // Default

                    if (!string.IsNullOrWhiteSpace(def.LabelOrientation))
                    {
                        if (def.LabelOrientation.Equals("Horizontal", StringComparison.OrdinalIgnoreCase))
                        {
                            stackOrientation = Orientation.Horizontal;
                        }
                        // Else default to Vertical
                    }

                    // Create a StackPanel to hold the label and value
                    var stackPanel = new StackPanel
                    {
                        Orientation = stackOrientation,
                        HorizontalAlignment = ParseEnum<HorizontalAlignment>(def.HorizontalAlignment, HorizontalAlignment.Center),
                        VerticalAlignment = ParseEnum<VerticalAlignment>(def.VerticalAlignment, VerticalAlignment.Center)
                    };

                    if (stackOrientation == Orientation.Vertical)
                    {
                        // Create the Label TextBlock
                        var labelTextBlock = new TextBlock
                        {
                            Text = def.LabelText,
                            Foreground = ConversionHelpers.ConvertToBrush(def.LabelForegroundColor),
                            Background = ConversionHelpers.ConvertToBrush(def.BackgroundColor),
                            FontSize = def.FontSize * 0.8, // Optional: Smaller font for label
                            FontWeight = ConversionHelpers.ConvertToFontWeight(def.FontWeight),
                            TextAlignment = ParseEnum<TextAlignment>(def.TextAlignment, TextAlignment.Center),
                            HorizontalAlignment = HorizontalAlignment.Center, // Center the label
                            Margin = new Thickness(0, 0, 0, 2) // Optional: Space between label and value
                        };

                        // Create the Value TextBlock
                        var valueTextBlock = new TextBlock
                        {
                            Text = def.DefaultText,
                            Foreground = ConversionHelpers.ConvertToBrush(def.ValueForegroundColor),
                            Background = ConversionHelpers.ConvertToBrush(def.BackgroundColor),
                            FontSize = def.FontSize,
                            FontWeight = ConversionHelpers.ConvertToFontWeight(def.FontWeight),
                            TextAlignment = ParseEnum<TextAlignment>(def.TextAlignment, TextAlignment.Center),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        // Bind the Value TextBlock to the controller's property
                        if (_viewModel.Controllers.TryGetValue(def.BindingProperty, out var controller))
                        {
                            valueTextBlock.DataContext = controller;
                            valueTextBlock.SetBinding(TextBlock.TextProperty, new Binding("ElementValue"));
                        }

                        // Add label and value to the StackPanel
                        stackPanel.Children.Add(labelTextBlock);
                        stackPanel.Children.Add(valueTextBlock);
                    }
                    else // Orientation.Horizontal
                    {
                        // Create the Label TextBlock
                        var labelTextBlock = new TextBlock
                        {
                            Text = def.LabelText,
                            Foreground = ConversionHelpers.ConvertToBrush(def.LabelForegroundColor),
                            Background = ConversionHelpers.ConvertToBrush(def.BackgroundColor),
                            FontSize = def.FontSize * 0.8, // Optional: Smaller font for label
                            FontWeight = ConversionHelpers.ConvertToFontWeight(def.FontWeight),
                            TextAlignment = ParseEnum<TextAlignment>(def.TextAlignment, TextAlignment.Center),
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0, 0, 5, 0) // Optional: Space between label and value
                        };

                        // Create the Value TextBlock
                        var valueTextBlock = new TextBlock
                        {
                            Text = def.DefaultText,
                            Foreground = ConversionHelpers.ConvertToBrush(def.ValueForegroundColor),
                            Background = ConversionHelpers.ConvertToBrush(def.BackgroundColor),
                            FontSize = def.FontSize,
                            FontWeight = ConversionHelpers.ConvertToFontWeight(def.FontWeight),
                            TextAlignment = ParseEnum<TextAlignment>(def.TextAlignment, TextAlignment.Center),
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        // Bind the Value TextBlock to the controller's property
                        if (_viewModel.Controllers.TryGetValue(def.BindingProperty, out var controller))
                        {
                            valueTextBlock.DataContext = controller;
                            valueTextBlock.SetBinding(TextBlock.TextProperty, new Binding("ElementValue"));
                        }

                        // Add label and value to the StackPanel
                        stackPanel.Children.Add(labelTextBlock);
                        stackPanel.Children.Add(valueTextBlock);
                    }

                    // Position the StackPanel in the grid
                    Grid.SetRow(stackPanel, def.Row);
                    Grid.SetColumn(stackPanel, def.Col);
                    Grid.SetRowSpan(stackPanel, def.RowSpan);
                    Grid.SetColumnSpan(stackPanel, def.ColSpan);

                    // Add the StackPanel to the target panel
                    targetPanel.Children.Add(stackPanel);
                }
                else
                {
                    // Create a TextBlock for the value only
                    var valueTextBlock = new TextBlock
                    {
                        Text = def.DefaultText,
                        Foreground = ConversionHelpers.ConvertToBrush(def.ValueForegroundColor),
                        Background = ConversionHelpers.ConvertToBrush(def.BackgroundColor),
                        FontSize = def.FontSize,
                        FontWeight = ConversionHelpers.ConvertToFontWeight(def.FontWeight),
                        TextAlignment = ParseEnum<TextAlignment>(def.TextAlignment, TextAlignment.Center),
                        HorizontalAlignment = ParseEnum<HorizontalAlignment>(def.HorizontalAlignment, HorizontalAlignment.Center),
                        VerticalAlignment = ParseEnum<VerticalAlignment>(def.VerticalAlignment, VerticalAlignment.Center)
                    };

                    // Bind the TextBlock to the controller's property
                    if (_viewModel.Controllers.TryGetValue(def.BindingProperty, out var controller))
                    {
                        valueTextBlock.DataContext = controller;
                        valueTextBlock.SetBinding(TextBlock.TextProperty, new Binding("ElementValue"));
                    }

                    // Position the TextBlock in the grid
                    Grid.SetRow(valueTextBlock, def.Row);
                    Grid.SetColumn(valueTextBlock, def.Col);
                    Grid.SetRowSpan(valueTextBlock, def.RowSpan);
                    Grid.SetColumnSpan(valueTextBlock, def.ColSpan);

                    // Add the TextBlock to the target panel
                    targetPanel.Children.Add(valueTextBlock);
                }
            }
        }

            /// <summary>
            /// Helper method to safely parse enum values with a default fallback.
            /// </summary>
            private T ParseEnum<T>(string value, T defaultValue) where T : struct
            {
                if (Enum.TryParse<T>(value, true, out var result))
                {
                    return result;
                }
                return defaultValue;
            }



            /// <summary>
            /// Handles commands from the PromptService.
            /// </summary>
            /// <param name="obj">The scoreboard command to process.</param>
            private void PromptService_HandleCommand(ScoreboardCommand obj)
            {
                if (string.IsNullOrEmpty(obj.ElementName))
                {
                    MessageBox.Show("Element name is missing in the command.");
                    return;
                }

                if (_viewModel.Controllers.TryGetValue(obj.ElementName, out var controller))
                {
                    controller.ProcessCommand(obj);
                }
                else
                {
                    MessageBox.Show($"Controller for element '{obj.ElementName}' not found.");
                }
            }

            private void NumPad_Click(object sender, RoutedEventArgs e)
            {
                if (sender is Button btn && btn.Tag is string keyString)
                {
                    // Convert string like "Key0" -> Keys.Key0
                    if (Enum.TryParse(typeof(Keys), keyString, out var enumValue))
                    {
                        _promptService.HandleKeyPress((Keys)enumValue);
                    }
                }
            }
    }


}
