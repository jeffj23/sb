namespace ScoreboardController.Data
{
    public interface ISoftKeyService
    {
        List<SoftKeyModel> LoadSoftKeysForSet(int setId);
    }

    public class SoftKeyService : ISoftKeyService
    {
        private readonly ScoreDbContext _dbContext;

        public SoftKeyService(ScoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<SoftKeyModel> LoadSoftKeysForSet(int setId)
        {
            // Real code would load from DB:
            // return _dbContext.SoftKeys
            //     .Where(k => k.SetId == setId)
            //     .OrderBy(k => k.Id)
            //     .ToList();

            // For now, let's return a stub list of 40 keys
            var list = new List<SoftKeyModel>();
            for (int i = 1; i <= 40; i++)
            {
                list.Add(new SoftKeyModel
                {
                    Id = i,
                    SetId = setId,
                    DisplayText = $"SK{i}",
                    CommandText = $"Command{i}"
                });
            }
            return list;
        }
    }
}
