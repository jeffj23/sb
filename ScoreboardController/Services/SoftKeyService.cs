using ScoreboardController.Repositories;

namespace ScoreboardController.Services
{
    public interface ISoftKeyService
    {
        List<SoftKey> LoadSoftKeysForSet(int setId);
    }

    public class SoftKeyService : ISoftKeyService
    {
        private readonly ISoftKeyRepository _softKeyRepository;
        public SoftKeyService(ISoftKeyRepository softKeyRepository)
        {
            _softKeyRepository = softKeyRepository;
        }

        public List<SoftKey> LoadSoftKeysForSet(int setId)
        {
            return _softKeyRepository.GetSoftKeysBySet(setId);
        }
    }
}
