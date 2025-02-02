using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using ScoreboardController.Commands;
using ScoreboardController.Models;
using ScoreboardController.Repositories;
using ScoreboardController.Views.Data;

namespace ScoreboardController.Services
{
    /// <summary>
    /// Mock implementation of ICommandMappingService for demonstration purposes.
    /// </summary>
    public class CommandMappingService : ICommandMappingService
    {
        private Dictionary<string, ActionCommandMapping> _mappings;
        private readonly ICommandMappingRepository _repository;

        public CommandMappingService(ICommandMappingRepository repository)
        {
            _repository = repository;
        }

        public ActionCommandMapping GetMapping(string actionName, bool refresh = false)
        {
            if (_mappings == null || refresh)
            {
                _mappings = _repository.GetActionCommandMappings();
            }

            if (_mappings.TryGetValue(actionName, out var mapping))
            {
                return mapping;
            }
            throw new KeyNotFoundException($"No mapping found for action '{actionName}'.");
        }
    }
}
