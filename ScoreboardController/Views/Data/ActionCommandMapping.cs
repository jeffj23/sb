using ScoreboardController.Commands;

namespace ScoreboardController.Views.Data
{
    /// <summary>
    /// Represents the mapping between a UI action and a scoreboard command.
    /// </summary>
    public class ActionCommandMapping
    {
        /// <summary>
        /// Identifier for the action, typically matching the Button's Tag.
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Name of the scoreboard element to target (e.g., "GameClock").
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// Type of command to execute.
        /// </summary>
        public CommandType CommandType { get; set; }

        public string CommandValue { get; set; }

        /// <summary>
        /// Indicates whether the command requires an input value (e.g., setting a time).
        /// </summary>
        public bool RequiresValue { get; set; }

        /// <summary>
        /// Prompt text to display if an input value is required.
        /// </summary>
        public string PromptText { get; set; }

        /// <summary>
        /// Input mask or format for the prompt (e.g., "# # : # # . #").
        /// </summary>
        public string InputMask { get; set; }
    }
}