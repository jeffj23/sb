using System.Windows.Controls;
using ScoreboardController.Controllers;
using ScoreboardController.Data;
using ScoreboardController.Elements;
using ScoreboardController.Services;

namespace ScoreboardController.Factories
{
    public static class ScoreboardControllerFactory
    {
        public static IScoreboardElementController CreateController(
            string elementName,
            string elementType,
            ITimerService? timerService, IJsonMessenger messenger)
        {
            switch (elementType)
            {
                case "Clock":
                    if (timerService == null)
                        throw new ArgumentNullException(nameof(timerService));
                    return new MainClockController(elementName, timerService, messenger);

                case "Counter":
                    return new CounterController(elementName, messenger);

                default:
                    throw new NotSupportedException(
                        $"Element type '{elementType}' is not supported.");
            }
        }
    }
}