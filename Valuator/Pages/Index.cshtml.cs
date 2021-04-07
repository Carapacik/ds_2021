using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SharedLibrary;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMessageBroker _messageBroker;
        private readonly IStorage _storage;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage, IMessageBroker messageBroker)
        {
            _storage = storage;
            _messageBroker = messageBroker;
            _logger = logger;
        }

        public IActionResult OnPost(string text, string country)
        {
            _logger.LogDebug(text);
            if (string.IsNullOrEmpty(text)) Redirect("/");

            var id = Guid.NewGuid().ToString();
            _logger.LogInformation($"{country} : {id} - OnPost");

            var similarity = GetSimilarity(text);
            _storage.StoreShard(id, country);
            _storage.Store(country, Constants.SimilarityKeyPrefix + id, similarity.ToString());
            _storage.Store(country, Constants.TextKeyPrefix + id, text);

            _messageBroker.Publish(Constants.SimilarityKeyCalculated,
                JsonSerializer.Serialize(new SimilarityObject {Id = id, Value = similarity}));
            _messageBroker.Publish(Constants.RankKeyProcessing, id);

            return Redirect($"summary?id={id}");
        }

        private int GetSimilarity(string text)
        {
            return _storage.HasTextDuplicates(text) ? 1 : 0;
        }
    }
}