using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreboardController.Commands;
using ScoreboardController.Views.Data;

namespace ScoreboardController.Repositories
{
    public interface ICommandMappingRepository
    {
        Dictionary<string, ActionCommandMapping> GetActionCommandMappings();
    }

    public class MockCommandMappingRepository : ICommandMappingRepository
    {
        public Dictionary<string, ActionCommandMapping> GetActionCommandMappings()
        {
            return new Dictionary<string, ActionCommandMapping>
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
    }
}
