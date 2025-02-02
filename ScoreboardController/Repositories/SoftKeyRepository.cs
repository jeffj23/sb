using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScoreboardController.Data;


namespace ScoreboardController.Repositories
{
    public interface ISoftKeyRepository
    {
        List<SoftKey> GetSoftKeysBySet(int scoreboardSetId);
    }

    public class SoftKeyRepository : ISoftKeyRepository
    {
        private ScoreDbContext _context;
        public SoftKeyRepository(ScoreDbContext context)
        {
            _context = context;
        }

        public List<SoftKey> GetSoftKeysBySet(int setId)
        {
            return _context.SoftKeys.Where(s => s.SetId == setId).ToList();

            var list = new List<SoftKey>();
            for (int i = 1; i <= 40; i++)
            {
                list.Add(new SoftKey
                {
                    Id = i,
                    SetId = setId,
                    Text = $"Key {i}",
                    Tag = $"Key{i:00}",
                    TextColor = "#FFFFFF",
                    BackgroundColor = "#3C0000"
                });
            }
            return list;
        }
    }
}
