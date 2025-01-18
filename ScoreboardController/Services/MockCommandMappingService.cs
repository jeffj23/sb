using System;
using System.Collections.Generic;
using ScoreboardController.Commands;
using ScoreboardController.Models;
using ScoreboardController.Views.Data;

namespace ScoreboardController.Services
{
    /// <summary>
    /// Mock implementation of ICommandMappingService for demonstration purposes.
    /// </summary>
    public class MockCommandMappingService : ICommandMappingService
    {
        private readonly Dictionary<string, ActionCommandMapping> _mappings;

        public MockCommandMappingService()
        {
            // Initialize with mock data. Replace this with database loading in the future.
            _mappings = new Dictionary<string, ActionCommandMapping>
            {
                {
                    "SetTime", new ActionCommandMapping
                    {
                        ActionName = "SetTime",
                        ElementName = "GameClock",
                        CommandType = CommandType.Set,
                        RequiresValue = true,
                        PromptText = "Set Time:",
                        InputMask = "# # : # # . #"
                    }
                },
                {
                    "ResetTime", new ActionCommandMapping
                    {
                        ActionName = "ResetTime",
                        ElementName = "GameClock",
                        CommandType = CommandType.Reset,
                        RequiresValue = false
                    }
                },
                {
                    "TimeIn", new ActionCommandMapping
                    {
                        ActionName = "TimeIn",
                        ElementName = "GameClock",
                        CommandType = CommandType.Start,
                        RequiresValue = false
                    }
                },
                {
                    "TimeOut", new ActionCommandMapping
                    {
                        ActionName = "TimeOut",
                        ElementName = "GameClock",
                        CommandType = CommandType.Stop,
                        RequiresValue = false
                    }
                },
                {
                    "SetDefaultTime", new ActionCommandMapping
                    {
                        ActionName = "SetDefaultTime",
                        ElementName = "GameClock",
                        CommandType = CommandType.SetDefault,
                        RequiresValue = true,
                        PromptText = "Set Default Period Length:",
                        InputMask = "# # : # # . #"
                    }

                },
                {
                    "Period", new ActionCommandMapping
                    {
                        ActionName = "Period",
                        ElementName = "Period",
                        CommandType = CommandType.Increment,
                        RequiresValue = true,
                        CommandValue = "1"
                    }

                }
                // Add more mappings as needed
            };  
        }

        public ActionCommandMapping GetMapping(string actionName)
        {
            if (_mappings.TryGetValue(actionName, out var mapping))
            {
                return mapping;
            }
            throw new KeyNotFoundException($"No mapping found for action '{actionName}'.");
        }
    }
}
