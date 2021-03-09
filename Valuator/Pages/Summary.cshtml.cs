﻿using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

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

        private async Task<string> GetRankAsync(string id)
        {
            const int tryCount = 1000;
            for (var i = 0; i < tryCount; i++)
            {
                var rank = _storage.Load(Constants.RankKeyPrefix + id);
                if (rank != null)
                    return rank;

                await Task.Delay(10);
            }

            return null;
        }

        public async Task OnGetAsync(string id)
        {
            _logger.LogDebug(id);

            string rank;
            if ((rank = await GetRankAsync(id)) != null)
                Rank = double.Parse(rank, CultureInfo.InvariantCulture);
            else
                _logger.LogWarning("Could not get rank value for id: " + id);
            Similarity = double.Parse(_storage.Load(Constants.SimilarityKeyPrefix + id), CultureInfo.InvariantCulture);
        }
    }
}