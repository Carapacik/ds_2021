using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SharedLibrary;

namespace Valuator.Pages
{
    public class Summary : PageModel
    {
        private readonly ILogger<Summary> _logger;
        private readonly IStorage _storage;

        public Summary(IStorage storage, ILogger<Summary> logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        private async Task<string> GetRankAsync(string id, string shard)
        {
            const int tryCount = 1000;
            for (var i = 0; i < tryCount; i++)
            {
                var rank = _storage.Load(shard, Constants.RankKeyPrefix + id);
                if (rank != null)
                    return rank;

                await Task.Delay(10);
            }

            return null;
        }

        public async Task OnGetAsync(string id)
        {
            var shard = _storage.LoadShard(id);
            _logger.LogInformation($"{shard} : {id} - OnGetAsync");

            string rank;
            if ((rank = await GetRankAsync(id, shard)) != null)
            {
                _logger.LogInformation($"{rank} - Rank");
                Rank = double.Parse(rank);
            }
            else
            {
                _logger.LogWarning($"Could not get rank value on shard [{shard}] for id: {id}");
            }

            Similarity = int.Parse(_storage.Load(shard, Constants.SimilarityKeyPrefix + id));
        }
    }
}