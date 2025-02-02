using ScoreboardController.Repositories;
using System.Collections.ObjectModel;
using Scoreboard.Data;
using ScoreboardController.Commands;

namespace ScoreboardController.Services
{
    public interface ISoftKeyService
    {
        void LoadSoftKeysForSet(int setId);
        void UnloadSoftKeySet();
        void HandleSoftKey(string key);
        ObservableCollection<SoftKey> SoftKeys { get; }
        int? LoadedSoftKeySet { get; }
    }

    public class SoftKeyService : ISoftKeyService
    {
        private readonly ISoftKeyRepository _softKeyRepository;
        private ObservableCollection<SoftKey> _softKeys;
        public int? LoadedSoftKeySet { get; private set; }
        public ObservableCollection<SoftKey> SoftKeys => _softKeys;
        private readonly IMessageDispatcher _messageDispatcher;

        public SoftKeyService(ISoftKeyRepository softKeyRepository, IMessageDispatcher messageDispatcher)
        {
            _softKeyRepository = softKeyRepository;
            _softKeys = new ObservableCollection<SoftKey>();
            _messageDispatcher = messageDispatcher;
        }

        public void LoadSoftKeysForSet(int setId)
        {
            LoadedSoftKeySet = setId;
            _softKeys = new ObservableCollection<SoftKey>(_softKeyRepository.GetSoftKeysBySet(setId));
        }

        public void UnloadSoftKeySet()
        {
            LoadedSoftKeySet = null;
        }

        public void HandleSoftKey(string key)
        {
            var softKey = _softKeys.FirstOrDefault(s => s.Tag == key);
            if (softKey != null)
            {
                var command = new ScoreboardCommand
                {
                    ElementName = softKey.Element ?? "",
                    CommandType = Enum.Parse<CommandType>(softKey.CommandType ?? ""), Value = softKey.Value
                };
                //_messageDispatcher.DispatchMessage();
            }
        }
            
    }

}

