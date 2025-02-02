using ScoreboardController.Views.Data;

namespace ScoreboardController.Services
{
    /// <summary>
    /// Interface for services that provide action-command mappings.
    /// </summary>
    public interface ICommandMappingService
    {
        /// <summary>
        /// Retrieves the mapping for a given action name.
        /// </summary>
        /// <param name="actionName">The identifier for the action.</param>
        /// <returns>The corresponding ActionCommandMapping.</returns>
        ActionCommandMapping GetMapping(string actionName, bool refresh);
    }
}