using System.ComponentModel;
using ScoreboardController.Commands;

namespace ScoreboardController.Controllers
{
    public interface IScoreboardElementController : INotifyPropertyChanged
    {
        /// <summary>
        /// The name of this element (e.g. "MainClock", "HomeScore")
        /// </summary>
        string ElementName { get; }

        /// <summary>
        /// e.g. "Clock", "Counter", etc.
        /// </summary>
        string ElementType { get; }

        string ElementValue { get; }

        /// <summary>
        /// Process a generic command: Start, Stop, Set, etc.
        /// The controller decides how (or if) to respond.
        /// </summary>
        void ProcessCommand(ScoreboardCommand command);

        /// <summary>
        /// Notifies subscribers that some internal state changed.
        /// For instance, "TimeString" => "20:30.5"
        /// </summary>
        event System.Action<string, string> OnStateChanged;

        /// <summary>
        /// Notifies subscribers that the controller wants 
        /// to publish an external message (e.g. JSON).
        /// </summary>
        event System.Action<string> OnMessage;
    }
}